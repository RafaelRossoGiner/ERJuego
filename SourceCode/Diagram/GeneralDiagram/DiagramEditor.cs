using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Rename pair of GameObjects for the nodes
using NodePair = System.Collections.Generic.KeyValuePair<UnityEngine.GameObject, UnityEngine.GameObject>;
public class DiagramEditor : MonoBehaviour
{
	[SerializeField]
	private GameObject erEntityPrefab, erRelationPrefab, erAttributePrefab, erGeneralizationPrefab;
	[SerializeField]
	private GameObject partPrefab, cardPrefab, refHandPrefab, genLinkPrefab;
	[SerializeField]
	private Transform nodesContent, linksContent;
	[SerializeField]
	private Sprite entSprite, relSprite, attSprite, genSprite;
	[SerializeField]
	private Sprite entSpriteDoub, relSpriteDoub, attSpriteDoub;
	[SerializeField]
	private ScrollRect scroll;
	//The controller used to handle all the menus and calls to the other components on the Diagram scene
	private DiagramMenuController diagramController;
	//The dictionary links each GameObject with the NodeData it represents
	private Dictionary<GameObject, NodeData> objToNodeData;
	//The dictionary links each GameObject with the LinkData it represents
	private Dictionary<GameObject, LinkData> objToLinkData;
	//The dictionary links each pair of Node GameObjects with the GameObject of their link
	private Dictionary<NodePair, GameObject> nodesToLinkObj;

	// Zooming
	private float minScale = 0.5f;
	private float maxScale = 2f;
	private float scaleFactor = 30f;

	private float currScale = 1f;

	// Minimal movement vector
	Vector3 MinMovement3 = new Vector3(0, 0.00001f, 0f);
	// Properties
	public float CurrentScale { get { return currScale; } }
	public void Awake()
	{
		diagramController = GetComponent<DiagramMenuController>();
		objToNodeData = new Dictionary<GameObject, NodeData>();
		objToLinkData = new Dictionary<GameObject, LinkData>();
		nodesToLinkObj = new Dictionary<NodePair, GameObject>();
	}
	//========================================= [ Dictionary Access ] ======================================================
	public NodeData ObjToNode(GameObject obj)
	{
		return objToNodeData[obj];
	}

	public LinkData ObjToLink(GameObject obj)
	{
		return objToLinkData[obj];
	}

	//========================================= [ Node Update ] ======================================================
	public NodeData Rename(GameObject nodeObj, out string oldName)
	{
		NodeData nodeData = objToNodeData[nodeObj];
		oldName = nodeData.nodeName;
		nodeData.Rename(nodeObj.name);
		nodeObj.GetComponentInChildren<TextMeshProUGUI>().text = nodeObj.name;
		return nodeData;
	}
	public void UnderlineName(GameObject nodeObj, bool underline)
	{
		if (underline) { 
			nodeObj.GetComponentInChildren<TextMeshProUGUI>().fontStyle = TMPro.FontStyles.Underline; 
		} else {
			nodeObj.GetComponentInChildren<TextMeshProUGUI>().fontStyle = TMPro.FontStyles.Normal;
		}
	}
	//========================================= [ Node Manipulation ] ======================================================
	public void DrawNode(NodeData nodeData)
	{
		//Set default name
		GameObject newNode = null;
		switch (nodeData.type) {
			case NodeData.NodeType.Entity:
				newNode = Instantiate(erEntityPrefab, nodesContent.transform, false);
				newNode.GetComponent<NodeController>().Initialize(nodeData, GetComponent<DiagramMenuController>(), this);
				break;
			case NodeData.NodeType.Relation:
				newNode = Instantiate(erRelationPrefab, nodesContent.transform, false);
				newNode.GetComponent<NodeController>().Initialize(nodeData, GetComponent<DiagramMenuController>(), this);
				break;
			case NodeData.NodeType.Attribute:
				newNode = Instantiate(erAttributePrefab, nodesContent.transform, false);
				newNode.GetComponent<NodeController>().Initialize(nodeData, GetComponent<DiagramMenuController>(), this);
				break;
			case NodeData.NodeType.Generalization:
				newNode = Instantiate(erGeneralizationPrefab, nodesContent.transform, false);
				newNode.GetComponent<GeneralizationController>().Initialize(nodeData, GetComponent<DiagramMenuController>(), this);
				break;
			default:
				Debug.LogError("Se ha intentado instanciar un nodo de tipo desconocido");
				break;
		}
		//Set as first sibling of the content panel, this ensures that is drawn on top of the other nodes
		newNode.transform.SetAsFirstSibling();
		newNode.GetComponent<RectTransform>().anchoredPosition = nodeData.position;
		//Initialize the Highlighter for the UI elements
		newNode.GetComponent<NodeHighlighter>().controller = diagramController;
		//Attach gameObject to the nodeData it is representing
		objToNodeData.Add(newNode, nodeData);
	}

	public NodeData RemNode(GameObject nodeObj, out List<LinkData> linksToBeRemoved)
	{
		linksToBeRemoved = RemoveAttachedLinks(nodeObj);
		NodeData nodeDataAttached = objToNodeData[nodeObj];
		objToNodeData.Remove(nodeObj);
		Destroy(nodeObj);
		return nodeDataAttached;
	}

	//============================================[ Link Drawing Functions ]================================================

	public void DrawLink(LinkData link)
	{
		GameObject[] objsLinking = new GameObject[2];
		GameObject mainLinkObj;
		SearchNodes(link, objsLinking);
		RectTransform[] rctsLinking = new RectTransform[2] { objsLinking[0].GetComponent<RectTransform>(), objsLinking[1].GetComponent<RectTransform>() };
		//Node
		NodePair nodes = new NodePair(objsLinking[0], objsLinking[1]);
		switch (link.type)
		{
			case LinkData.LinkTypes.EntityAttr:
			case LinkData.LinkTypes.RelationAtt:
			case LinkData.LinkTypes.AttrAttr:
				mainLinkObj = Instantiate(partPrefab, nodesContent);
				mainLinkObj.GetComponent<ParticipationIndicator>().Initialize(rctsLinking, linksContent, link);
				break;
			case LinkData.LinkTypes.EntityRel:
				mainLinkObj = Instantiate(cardPrefab, nodesContent);
				mainLinkObj.GetComponent<CardinalityIndicator>().Initialize(rctsLinking, linksContent, link);
				break;
			case LinkData.LinkTypes.EntityRelReflexive:
				mainLinkObj = Instantiate(refHandPrefab, nodesContent);
				mainLinkObj.GetComponent<ReflexiveIndicator>().Initialize(rctsLinking, linksContent, link);
				//Invert nodes to avoid a key duplication on the internal Dictionary
				nodes = new NodePair(objsLinking[1], objsLinking[0]);
				break;
			case LinkData.LinkTypes.Generalization:
			case LinkData.LinkTypes.Specialization:
				mainLinkObj = Instantiate(genLinkPrefab, nodesContent);
				mainLinkObj.GetComponent<GeneralizationIndicator>().Initialize(rctsLinking, linksContent, link);
				break;
			default:
				mainLinkObj = null;
				Debug.LogError("Unknown link type encountered");
				break;
		}

		//Add to corresponding dictionaries
		nodesToLinkObj.Add(nodes, mainLinkObj);
		objToLinkData.Add(mainLinkObj, link);
	}
	public LinkData RemLink(GameObject node1, GameObject node2)
	{
		NodePair nodeObjs = new NodePair(node1, node2);
		NodePair nodeObjsInv = new NodePair(node2, node1);
		GameObject linkObjAttached;
		if (!nodesToLinkObj.TryGetValue(nodeObjs, out linkObjAttached))
		{
			nodesToLinkObj.TryGetValue(nodeObjsInv, out linkObjAttached);
		}
		LinkData linkDataAttached = objToLinkData[linkObjAttached];

		//Destroy visual object and remove from dictionary
		objToLinkData.Remove(linkObjAttached);

		Destroy(linkObjAttached);
		return linkDataAttached;
	}
	// This function removes all link objects attached to the node being removed, it also
	// returns a list with their corresponding LinkData objects.
	private List<LinkData> RemoveAttachedLinks(GameObject nodeBeingRemoved)
	{
		List<LinkData> linksToBeRemoved = new List<LinkData>();
		GameObject foundLink;
		LinkData linkDataAttached;
		foreach (KeyValuePair<GameObject, NodeData> node in objToNodeData)
		{
			NodePair nodeObjs = new NodePair(nodeBeingRemoved, node.Key);
			NodePair nodeObjsInv = new NodePair(node.Key, nodeBeingRemoved);
			if (nodesToLinkObj.TryGetValue(nodeObjs, out foundLink) && foundLink != null)
			{
				linkDataAttached = objToLinkData[foundLink];
				linksToBeRemoved.Add(linkDataAttached);
				Destroy(foundLink);
			}
			else if (nodesToLinkObj.TryGetValue(nodeObjsInv, out foundLink) && foundLink != null)
			{
				linkDataAttached = objToLinkData[foundLink];
				linksToBeRemoved.Add(linkDataAttached);
				Destroy(foundLink);
			}
		}
		return linksToBeRemoved;
	}
	//========================================= [ Complete Diagram Manipulation ] ======================================================
	public void DrawDiagram(ERData diagram)
	{
		//Instantiate Nodes
		foreach (NodeData node in diagram.nodes)
		{
			DrawNode(node);
		}
		foreach (LinkData link in diagram.links)
		{
			DrawLink(link);
		}
	}

	public void ClearDiagram()
	{
		//Clear diagram graphics
		foreach (Transform node in nodesContent)
		{
			Destroy(node.gameObject);
		}
		foreach (Transform node in linksContent)
		{
			Destroy(node.gameObject);
		}
		objToNodeData.Clear();
		objToLinkData.Clear();
		nodesToLinkObj.Clear();
	}
	//========================================= [ Auxiliar search function ] ======================================================
	private void SearchNodes(LinkData link, GameObject[] nodeRcts)
	{
		int iter = 0;
		foreach (KeyValuePair<GameObject, NodeData> node in objToNodeData)
		{
			foreach (int nodeId in link.linkedNodeId)
			{
				if (node.Value.id == nodeId)
				{
					//If one node of the diagram is the same as one of the linkedNodes stored on the link,
					//then add the GameObject which represents the node to the nodesToLink list of objects.
					nodeRcts[iter] = node.Key;
					iter++;
				}
			}
			if (iter >= 2)
				//Already found all the nodes stored on the link, no need to keep searching
				break;
		}
	}

	//========================================= [ Zoom Manipulation ] ======================================================
	public void Zoom(float velocity)
	{
		currScale = nodesContent.localScale.x + (Time.deltaTime * scaleFactor * Mathf.Sign(velocity));
		if (currScale > maxScale)
			currScale = maxScale;
		else if (currScale < minScale)
			currScale = minScale;

		nodesContent.localScale = Vector3.one * currScale;
	}

	// ================================== [ Update link rendering ] ================================================
	public void RedDrawLinks()
    {
        foreach (KeyValuePair<GameObject, NodeData> node in objToNodeData)
        {
            node.Key.transform.localPosition += MinMovement3;
        }
	}
}
