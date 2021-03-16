using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Rename pair of GameObjects for the nodes
using NodePair = System.Collections.Generic.KeyValuePair<UnityEngine.GameObject, UnityEngine.GameObject>;
[RequireComponent(typeof(DiagramController))]
public class ERDiagram : MonoBehaviour
{
	[SerializeField]
	private GameObject erEntityPrefab, erRelationPrefab, erAttributePrefab, erGeneralizationPrefab;
	[SerializeField]
	private GameObject erNodePrefab, partPrefab, cardPrefab, refHandPrefab, genLinkPrefab;
	[SerializeField]
	private Transform nodesContent, linksContent;
	[SerializeField]
	private Sprite entSprite, relSprite, attSprite, genSprite;
	[SerializeField]
	private Sprite entSpriteDoub, relSpriteDoub, attSpriteDoub;
	[SerializeField]
	private ScrollRect scroll;
	//The controller used to handle all the menus and calls to the other components on the Diagram scene
	private DiagramController diagramController;
	//The dictionary links each GameObject with the NodeData it represents
	public Dictionary<GameObject, NodeData> objToNode;
	//The dictionary links each GameObject with the LinkData it represents
	public Dictionary<GameObject, LinkData> objToLink;
	//The dictionary links each pair of Node GameObjects with the GameObject of their link
	private Dictionary<NodePair, GameObject> nodesToLinkObj;

	public static bool scrolling;

	public void Awake()
	{
		diagramController = GetComponent<DiagramController>();
		objToNode = new Dictionary<GameObject, NodeData>();
		objToLink = new Dictionary<GameObject, LinkData>();
		nodesToLinkObj = new Dictionary<NodePair, GameObject>();
	}

	public NodeData UpdateName(GameObject nodeObj)
	{
		NodeData node = objToNode[nodeObj];
		nodeObj.GetComponentInChildren<TextMeshProUGUI>().text = node.nodeName;
		return node;
	}
	public NodeData UpdateName(GameObject nodeObj, out string oldName)
	{
		NodeData node = objToNode[nodeObj];
		oldName = node.nodeName;
		nodeObj.GetComponentInChildren<TextMeshProUGUI>().text = node.nodeName;
		return node;
	}
	public void UnderlineName(GameObject nodeObj, bool underline)
	{
		if (underline) { 
			nodeObj.GetComponentInChildren<TextMeshProUGUI>().fontStyle = TMPro.FontStyles.Underline; 
		} else {
			nodeObj.GetComponentInChildren<TextMeshProUGUI>().fontStyle = TMPro.FontStyles.Normal;
		}
		Debug.Log("Called" + underline + " " + nodeObj.GetComponentInChildren<TextMeshProUGUI>().fontStyle);
	}

	public void DrawNode(NodeData nodeData)
	{
		//Instantiate Node Object
		GameObject newNode = null;
		switch (nodeData.type) {
			case NodeData.NodeType.Entity:
				newNode = Instantiate(erEntityPrefab, nodesContent.transform, false);
				newNode.GetComponent<NodeController>().Initialize(nodeData, GetComponent<DiagramController>(), this);
				break;
			case NodeData.NodeType.Relation:
				newNode = Instantiate(erRelationPrefab, nodesContent.transform, false);
				newNode.GetComponent<NodeController>().Initialize(nodeData, GetComponent<DiagramController>(), this);
				break;
			case NodeData.NodeType.Attribute:
				newNode = Instantiate(erAttributePrefab, nodesContent.transform, false);
				newNode.GetComponent<NodeController>().Initialize(nodeData, GetComponent<DiagramController>(), this);
				break;
			case NodeData.NodeType.Generalization:
				newNode = Instantiate(erGeneralizationPrefab, nodesContent.transform, false);
				newNode.GetComponent<GeneralizationController>().Initialize(nodeData, GetComponent<DiagramController>(), this);
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
		objToNode.Add(newNode, nodeData);

		UpdateName(newNode);
	}

	public void DrawDiagram(ERData diagram)
	{
		Debug.Log("Code: " + diagram.diagramCode + "Nodes: " + diagram.nodes.Count + "Links: " + diagram.nodes.Count);
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

	public NodeData RemNode(GameObject nodeObj, out List<LinkData> linksRemoved)
	{
		linksRemoved = RemLinkedObjs(nodeObj);
		NodeData nodeDataAttached = objToNode[nodeObj];
		objToNode.Remove(nodeObj);
		Destroy(nodeObj);
		return nodeDataAttached;
	}

	public LinkData RemLink(GameObject node1, GameObject node2)
	{
		NodePair nodeObjs = new NodePair(node1, node2);
		NodePair nodeObjsInv = new NodePair(node2, node1);
		GameObject linkObjAttached;
		if (nodesToLinkObj.TryGetValue(nodeObjs, out linkObjAttached))
			linkObjAttached = nodesToLinkObj[nodeObjs];
		else if (nodesToLinkObj.TryGetValue(nodeObjsInv, out linkObjAttached))
			linkObjAttached = nodesToLinkObj[nodeObjsInv];
		LinkData linkDataAttached = objToLink[linkObjAttached];
		objToLink.Remove(linkObjAttached);
		Destroy(linkObjAttached);
		return linkDataAttached;
	}

	//============================================[ Node Drawing Functions ]================================================

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
		objToLink.Add(mainLinkObj, link);
	}

	private List<LinkData> RemLinkedObjs(GameObject nodeToSearch)
	{
		List<LinkData> links = new List<LinkData>();
		GameObject foundLink;
		foreach (KeyValuePair<GameObject, NodeData> node in objToNode)
		{
			NodePair nodeObjs = new NodePair(nodeToSearch, node.Key);
			NodePair nodeObjsInv = new NodePair(node.Key, nodeToSearch);
			if (nodesToLinkObj.TryGetValue(nodeObjs, out foundLink))
			{
				links.Add(objToLink[foundLink]);
				Destroy(foundLink);
			}
			if (nodesToLinkObj.TryGetValue(nodeObjsInv, out foundLink))
			{
				links.Add(objToLink[foundLink]);
				Destroy(foundLink);
			}
		}
		return links;
	}

	private void SearchNodes(LinkData link, GameObject[] nodeRcts)
	{
		int iter = 0;
		foreach (KeyValuePair<GameObject, NodeData> node in objToNode)
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
}
