using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(ERDiagram))]

public class DiagramController : MonoBehaviour, IPointerClickHandler, IPointerExitHandler{
	[SerializeField]
	private GameObject createNodeMenu, editNodeMenu, editAttMenu, editGenMenu, nameMenu, nameMenuAtr, background;
	[SerializeField]
	private ScrollRect scroll;
	[SerializeField]
	private TextMeshProUGUI pathDisplay;

	private Camera mainCamera;
	public enum Modes { selecting, linking, unLinking };
	public Modes mode = Modes.selecting;

	private Canvas canvas_;

	private ERData erData_;
	private ERDiagram erDiagram_;
	private Dropdown nameList, nameListAttr;

	private GameObject selectedNode;
	private Vector2 selectedPos;

	void Start(){
		Cursor.lockState = CursorLockMode.None;
		//Get Canvas Scaler component
		canvas_ = GetComponent<Canvas>();
		//Get Graphic and Data components of the ERDiagram
		erData_ = DiagramKeeper.GetCurrDiagram();
		erDiagram_ = GetComponent<ERDiagram>();
		//Setup path display
		pathDisplay.text = GetPathToDiagram();
		//Load all names on the nameDropdown Menu
		List<string> dropdownOptions = GlobalController.config.nodeNames;
		//Apply the names to the DropDown Component
		nameList = nameMenu.GetComponent<Dropdown>();
		nameListAttr = nameMenuAtr.GetComponent<Dropdown>();
		nameList.AddOptions(dropdownOptions);
		nameListAttr.AddOptions(dropdownOptions);
		//Create diagram form diagramData
		erDiagram_.DrawDiagram(erData_);
	}

	//Contextual Menu Behaviour
	public void OnPointerClick(PointerEventData eventData){
		if (eventData.button == PointerEventData.InputButton.Right){
			//Right click
			if (mode != Modes.selecting){
				//If there is an edition happening, close it
				ExitEdition();
			}
			CloseAllMenus();
			if (eventData.pointerCurrentRaycast.gameObject == background){
				//The user is not selecting anything, the create Node menu must be shown
				OpenNodeCreation(eventData);
			}
			else{
				//The user is selecting a node, the Node edition menu must be shown
				OpenNodeEdition(eventData);
			}
		}
		else{
			//Just in case the Input detects another mouse button, close everything and do nothing
			ExitEdition();
			CloseAllMenus();
		}
	}
	public void OnPointerExit(PointerEventData eventData){
		CloseAllMenus();
		ExitEdition();
	}

	//============================[ Methods ]===================================================
	//Menu Handling
	public Modes NodeLeftClicked(NodeController node)
	{
		//The mode will always return to selecting after the click is handled so we want to store
		//it in a local variable to be able to return the mode when the event was called and not after
		Modes inputMode = mode;
		switch (inputMode)
		{
			case Modes.selecting:
				switch (node.nodeAttached.type)
				{
					case NodeData.NodeType.Entity:
					case NodeData.NodeType.Relation:
					case NodeData.NodeType.Attribute:
						node.SwitchContour();
						break;
					default:
						//For clarity the Generalization state change is handled on the GeneralizationNode.cs script
						//before calling this method so it's not necessary to update it here. Still we need to keep
						//calling the method for it be consistent with the linking system
						break;
				}
				break;
			case Modes.linking:
				{
					//We want to link the nodes, selectedNode is still the previous node
					AddLink(erDiagram_.objToNode[selectedNode], erDiagram_.objToNode[node.gameObject]);
					break;
				}
			case Modes.unLinking:
				{
					//We want to unlink the nodes, selectedNode is still the previous node
					RemLink(selectedNode, node.gameObject);
					break;
				}
		}
		ExitEdition();
		CloseAllMenus();
		//Some methods may use this value to modify it's functionality
		return inputMode;
	}
	public void OpenNodeCreation(PointerEventData eventData)
	{
		selectedPos = eventData.position;
		createNodeMenu.SetActive(true);
		RectTransform menuRect_ = createNodeMenu.GetComponent<RectTransform>();
		menuRect_.anchoredPosition = eventData.position;
	}
	public void OpenNodeEdition(PointerEventData eventData)
	{
		selectedNode = eventData.pointerCurrentRaycast.gameObject;
		editNodeMenu.SetActive(true);
		RectTransform menuRect_ = editNodeMenu.GetComponent<RectTransform>();
		menuRect_.anchoredPosition = eventData.position;
	}
	public void OpenAttributeMenu(PointerEventData eventData)
	{
		selectedNode = eventData.pointerCurrentRaycast.gameObject;
		editAttMenu.SetActive(true);
		RectTransform menuRect_ = editAttMenu.GetComponent<RectTransform>();
		menuRect_.anchoredPosition = eventData.position;
	}
	public void OpenGeneralizationMenu(PointerEventData eventData)
	{
		selectedNode = eventData.pointerCurrentRaycast.gameObject;
		editGenMenu.SetActive(true);
		RectTransform menuRect_ = editGenMenu.GetComponent<RectTransform>();
		menuRect_.anchoredPosition = eventData.position;
	}
	public void CloseAllMenus(){
		createNodeMenu.SetActive(false);
		editNodeMenu.SetActive(false);
		editAttMenu.SetActive(false);
		editGenMenu.SetActive(false);
		nameMenu.SetActive(false);
	}
	public string GetPathToDiagram()
	{
		return "Se guarda en: " + Path.Combine(Application.persistentDataPath,"Saves") + " Codigo: " + DiagramKeeper.GetCurrDiagramCode();
	}
	public void CopyPathToClipboard()
	{
		GUIUtility.systemCopyBuffer = Application.persistentDataPath;
	}
	public void GenerateJson()
	{
		erData_.Save();
	}
	public void DeleteJson()
	{
		erData_.Delete();
	}
	public void CloseDiagramScene()
	{
		SceneHandler.CloseDiagram();
	}
	//Node Edition
	public void ChangeName()
	{
		string oldName;
		string newName = nameList.options[nameList.value].text;
		selectedNode.name = newName;
		erDiagram_.objToNode[selectedNode].Rename(newName);
		NodeData node = erDiagram_.UpdateName(selectedNode, out oldName);
		PlayerEventHandler.ChangeName(node, oldName);
	}
	public void ChangeNameAtr()
	{
		string oldName;
		string newName = nameListAttr.options[nameListAttr.value].text;
		selectedNode.name = newName;
		erDiagram_.objToNode[selectedNode].Rename(newName);
		NodeData node = erDiagram_.UpdateName(selectedNode, out oldName);
		PlayerEventHandler.ChangeName(node, oldName);
	}
	public void SwitchAttributeAsKey()
	{
		bool isKey = erDiagram_.objToNode[selectedNode].SwitchIsKey();
		erDiagram_.UnderlineName(selectedNode, isKey);
	}

	public void AddNode(NodeTypeClass typeClass) {
		Vector2 nodePos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(scroll.content, selectedPos, mainCamera, out nodePos);
		NodeData nodeData = new NodeData(typeClass.nodeType, nodePos, erData_.lastId++);
		erDiagram_.DrawNode(nodeData);
		erData_.AddNodeData(nodeData);
		PlayerEventHandler.AddNode(nodeData);
	}

	public void RemNode(){
		if(selectedNode != null) {
			List<LinkData> linksRemoved = new List<LinkData>();
			NodeData nodeToRem = erDiagram_.RemNode(selectedNode, out linksRemoved);
			//Remove All Links attached
			foreach (LinkData link in linksRemoved) {
				erData_.RemoveLink(link);
			}
			//Remove Node
			PlayerEventHandler.RemNode(nodeToRem); //Logs the action
			erData_.RemoveNodeData(nodeToRem); //Remos the node
		}
	}

	//Link Edition
	private void AddLink(NodeData node1, NodeData node2){
		//In this method we have to determine the link type needed based on the nodes selected
		LinkData newLink;
		if (node1.type != node2.type){
			//The links are not of the same type (which has no sense in the ER Diagram model), we can continue
			if (node1.type == NodeData.NodeType.Generalization || node2.type == NodeData.NodeType.Generalization) {
				//We will handle Generalizations apart from the other node types since they are an utility node which works as a
				//complex link between multiple nodes and will be represented as a node inside the game but as a link on the output logs
				if(node1.type == NodeData.NodeType.Entity || node2.type == NodeData.NodeType.Entity)
				{
					//We can only perform generalization links on entities, any other node will have no effect, by default it will be an
					//specialization link so the node will be represented as a subclass related to the Generalization node. The user must
					//make a decision later to determine wich links will be a generalization instead, meaning the node related is a superclass

					//For tracking purposes on the Log file, the first node will always be the generalization node. It is planned
					//to be modify in near futuro to improve this behaviour and allow any order.
					if (node1.type == NodeData.NodeType.Generalization){
						newLink = new LinkData(node1, node2, LinkData.LinkTypes.Specialization);
					}
					else{
						newLink = new LinkData(node2, node1, LinkData.LinkTypes.Specialization);
					}
					erData_.CreateLink(newLink);
					erDiagram_.DrawLink(newLink);
					PlayerEventHandler.AddLink(newLink);
				}
			}
			else {
				//The link does not involve a generalization node
				if (node1.type == NodeData.NodeType.Relation || node2.type == NodeData.NodeType.Relation)
				{
					//There is a relation involved, we may need to initialize a link with Cardinality
					if (node1.type == NodeData.NodeType.Entity || node2.type == NodeData.NodeType.Entity)
					{
						//Relation-Entity link, cardinality needed
						newLink = new LinkData(node1, node2, LinkData.LinkTypes.EntityRel);
					}
					else
					{
						//Attribute-Entity link, no cardinality needed
						newLink = new LinkData(node1, node2, LinkData.LinkTypes.EntityAttr);
					}
				}
				else
				{
					//It is a Entity-Atribute link, no cardinality needed
					newLink = new LinkData(node1, node2, LinkData.LinkTypes.EntityAttr);
				}
				int availableLinks = erData_.FreeLinkSlots(node1, node2, newLink.type);
				if (availableLinks > 0)
				{
					//Only create link if there is available slots for linking those nodes, when there are relations involved
					//there can be reflexive relations, which involve only 1 entity in a relation with itself.
					//Relation links will always start with 2 free linking slots and the others usally with 1.
					if (availableLinks == 1 && newLink.type == LinkData.LinkTypes.EntityRel)
					{
						//The new link is with a relation but we only have 1 slot lef, which means there is already a link between these nodes
						//so we must change the type os this link to a Reflexive link so they do not overlap graphically and the user can move it freely
						newLink.type = LinkData.LinkTypes.EntityRelReflexive;
						newLink.auxRectPos = selectedPos;
					}
					erData_.CreateLink(newLink);
					erDiagram_.DrawLink(newLink);
					PlayerEventHandler.AddLink(newLink);
				}
			}
		}
		else if (node1.type == NodeData.NodeType.Attribute && node2.type == NodeData.NodeType.Attribute)
		{
			//Only allow links between the same node types if they are attributes
			newLink = new LinkData(node1, node2, LinkData.LinkTypes.AttrAttr);
			erData_.CreateLink(newLink);
			erDiagram_.DrawLink(newLink);
			PlayerEventHandler.AddLink(newLink);
		}
	}
	private void RemLink(GameObject node1, GameObject node2){
		LinkData link = erDiagram_.RemLink(node1, node2);
		PlayerEventHandler.RemLink(link);
		erData_.RemoveLink(link);
	}
	//Mode Transitions
	public void EnterLinking(){
		if (selectedNode != null){
			mode = Modes.linking;
			NodeHighlighter.SetFirst(selectedNode, mode);
		}
	}

	public void EnterUnLinking(){
		if (selectedNode != null) {
			mode = Modes.unLinking;
			NodeHighlighter.SetFirst(selectedNode, mode);
		}
	}

	private void ExitEdition(){
		mode = Modes.selecting;
		NodeHighlighter.first = null;
		NodeHighlighter.CancelAllHighlights();
	}
}
