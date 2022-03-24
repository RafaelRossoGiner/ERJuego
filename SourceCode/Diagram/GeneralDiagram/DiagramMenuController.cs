using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;
using System.Collections;

[RequireComponent(typeof(DiagramEditor))]

public class DiagramMenuController : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	[SerializeField]
	private GameObject createNodeMenu, editNodeMenu, editAttMenu, editGenMenu, nameMenu, nameMenuAtr, errorPanel, background;
	[SerializeField]
	private ScrollRect scroll;
	[SerializeField]
	private TextMeshProUGUI pathDisplay, errorPanelTittle;
	[SerializeField]
	private GameObject MessagePrefab;
	[SerializeField]
	private Transform ErrorsContent;
	[SerializeField]
	private string unlockingMessage = "Se han desbloquado las habitaciones: ";
	[SerializeField]
	private float displayMessageTimer = 3f;

	private Camera mainCamera;
	public enum Modes { selecting, linking, unLinking };
	public Modes mode = Modes.selecting;

	private bool isUIOpen;

	private ERData erData_;
	private DiagramEditor erDiagram_;
	private Dropdown nameList, nameListAttr;

	private GameObject selectedNode;
	private Vector2 selectedPos;

	// Coloring
	private Color light_green = new Color(0.4302955f, 0.9622642f, 0.4523566f);
	private Color light_red = new Color(1f, 0.6393762f, 0.6358491f);
	private Color light_yellow = new Color(0.983665f, 1f, 0.6352941f);

	// Dragging variables
	private Vector2 prevPos;
	private Vector2 startMouse;
	private Vector2 resolutionSize;

	private static OverlayMenu overlayMenu;

	public void Start() {
		UnityEngine.Cursor.lockState = CursorLockMode.None;
		//Get Graphic and Data components of the ERDiagram
		erData_ = GameDiagramManager.GetCurrDiagram();
		erDiagram_ = GetComponent<DiagramEditor>();
		//Setup path display
		pathDisplay.text = GetPathToDiagram();
		//Load all names on the nameDropdown Menu
		List<string> dropdownOptions = SerializationManager.config.nodeNames;
		foreach(string usedName in erData_.GetUsedNames())
        {
			Debug.Log("Eliminated: " + usedName);
			dropdownOptions.Remove(usedName);
        }
		dropdownOptions.Insert(0, SerializationManager.config.unnamedNode);

		//Apply the names to the DropDown Component
		nameList = nameMenu.GetComponent<Dropdown>();
		nameListAttr = nameMenuAtr.GetComponent<Dropdown>();

		nameList.ClearOptions();
		nameListAttr.ClearOptions();
		nameList.AddOptions(dropdownOptions);
		nameListAttr.AddOptions(dropdownOptions);
		// Setup no-selection option at end
		nameList.AddOptions(new List<string> { "" });
		nameListAttr.AddOptions(new List<string> { "" });
		nameList.SetValueWithoutNotify(nameList.options.Count);
		nameListAttr.SetValueWithoutNotify(nameList.options.Count);

		//Create diagram form diagramData
		erDiagram_.DrawDiagram(erData_);

		//Drag Initialization
		resolutionSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
	}
	public static void SetOverlayMenu(OverlayMenu newMenu)
	{
		overlayMenu = newMenu;
	}
	IEnumerator CloseDiagram()
	{
		yield return new WaitForEndOfFrame();
		var texture = ScreenCapture.CaptureScreenshotAsTexture();

		GameDiagramManager.AddTexture(texture, erData_.diagramCode);

		Destroy(texture);
		SceneHandler.CloseDiagram();
	}
	public void Update()
	{
		float wheelScrollVelocity = Input.GetAxis("Mouse ScrollWheel");
		if (wheelScrollVelocity != 0f) // forward
		{
			// Zooming
			if (!SceneHandler.isPaused() && !isUIOpen)
				erDiagram_.Zoom(wheelScrollVelocity);
		}
		erDiagram_.RedDrawLinks();
	}
	//========================================= [ Diagram UI Manipulation ] ======================================================
	// Drag
	public void OnBeginDrag(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			prevPos = Vector2.one - scroll.normalizedPosition;
			startMouse = eventData.position / resolutionSize;
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			Vector2 finalPos = Vector2.one - (prevPos + eventData.position / resolutionSize - startMouse);
			if (finalPos.x >= 0 && finalPos.x <= 1)
				scroll.horizontalNormalizedPosition = finalPos.x;
			if (finalPos.y >= 0 && finalPos.y <= 1)
				scroll.verticalNormalizedPosition = finalPos.y;
		}
		//erDiagram_.RedDrawLinks();
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		//erDiagram_.RedDrawLinks();
	}

	//========================================= [ Contextual Menu Behaviour ] ======================================================
	public void OnPointerClick(PointerEventData eventData) {
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			//Right click
			if (mode != Modes.selecting)
			{
				//If there is an edition happening, close it
				ExitEdition();
			}
			CloseAllMenus();
			if (eventData.pointerCurrentRaycast.gameObject == background)
			{
				//The user is not selecting anything, the create Node menu must be shown
				OpenNodeCreation(eventData);
			}
		}
		else
		{
			//Just in case the Input detects another mouse button, close everything and do nothing
			ExitEdition();
			CloseAllMenus();
		}
	}
	public void OnPointerExit(PointerEventData eventData) {
		CloseAllMenus();
		ExitEdition();
	}

	//============================[ Node Menu Manipulation ]===================================================
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
					AddLink(erDiagram_.ObjToNode(selectedNode), erDiagram_.ObjToNode(node.gameObject));
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
		isUIOpen = true;
		selectedPos = eventData.position;
		createNodeMenu.SetActive(true);
		RectTransform menuRect_ = createNodeMenu.GetComponent<RectTransform>();
		menuRect_.anchoredPosition = eventData.position;
	}
	public void OpenNodeEdition(PointerEventData eventData)
	{
		isUIOpen = true;
		selectedNode = eventData.pointerCurrentRaycast.gameObject;
		editNodeMenu.SetActive(true);
		RectTransform menuRect_ = editNodeMenu.GetComponent<RectTransform>();
		menuRect_.anchoredPosition = eventData.position;
	}
	public void OpenAttributeMenu(PointerEventData eventData)
	{
		isUIOpen = true;
		selectedNode = eventData.pointerCurrentRaycast.gameObject;
		editAttMenu.SetActive(true);
		RectTransform menuRect_ = editAttMenu.GetComponent<RectTransform>();
		menuRect_.anchoredPosition = eventData.position;
	}
	public void OpenGeneralizationMenu(PointerEventData eventData)
	{
		isUIOpen = true;
		selectedNode = eventData.pointerCurrentRaycast.gameObject;
		editGenMenu.SetActive(true);
		RectTransform menuRect_ = editGenMenu.GetComponent<RectTransform>();
		menuRect_.anchoredPosition = eventData.position;
	}
	public void CloseAllMenus() {
		isUIOpen = false;
		createNodeMenu.SetActive(false);
		editNodeMenu.SetActive(false);
		editAttMenu.SetActive(false);
		editGenMenu.SetActive(false);
		nameMenu.SetActive(false);
		nameMenuAtr.SetActive(false);
	}
	//========================================= [ UI Panel Functions ] ======================================================
	public string GetPathToDiagram()
	{
		return "Guardado en: " + SerializationManager.GeneralDir + " como " + erData_.diagramCode;
	}
	public void CopyPathToClipboard()
	{
		GUIUtility.systemCopyBuffer = SerializationManager.GeneralDir;
		overlayMenu.DisplayMessage("Se ha copiado la ruta de guardado al portapapeles", 2f);
	}
	public void SaveDiagram()
	{
		SerializationManager.SaveER(erData_);
	}
	public void ResetDiagram()
	{
		GameDiagramManager.ResetCurrentDiagram();
		erDiagram_.ClearDiagram();
	}
	public void CloseDiagramScene()
	{
		StartCoroutine(CloseDiagram());
	}
	public void CheckDiagram()
	{
		ERData solDiagram;
		List<(int, string)> errorMessageList;
		List<(int, string)> successMessageList = null;
		int score = 0, maxScore = 0;
		bool isQualifiable = erData_.CheckErrors(out errorMessageList, ref score, ref maxScore);
		if (isQualifiable && GameDiagramManager.solutions.TryGetValue(erData_.diagramCode, out solDiagram))
		{
			// It has a solution, run a similarity report too
			isQualifiable = erData_.CheckSimilarity(solDiagram, out successMessageList, ref score, ref maxScore);
			if (!isQualifiable)
			{
				errorMessageList.Add((0, "La solucion parece contener nombres repetidos, no es posible comparar con el diagrama propuesto"));
			}
		}
		if (isQualifiable && !SerializationManager.IsAlreadyCheckedDiagram(erData_.diagramCode))
		{
			if (score < 0)
            {
				score = 0;
            }
			overlayMenu.AddScorePoints(score);
			overlayMenu.DisplayMessage("Obtuviste " + score + " puntos", displayMessageTimer);
		}
		DisplayDiagramErrors(score, maxScore, isQualifiable, errorMessageList, successMessageList);
	}
	private void DisplayDiagramErrors(int score, int maxScore, bool isQualifiable, List<(int, string)> errorMessageList, List<(int, string)> correctElements = null)
	{
		// Destroy old lists
		foreach (Transform child in ErrorsContent)
		{
			Destroy(child.gameObject);
		}
		if (!isQualifiable)
        {
			// Set title
			errorPanelTittle.text = "Para poder corregir el diagram debes, al menos, <b><color=#FFAAAA>corregir los errores indicados</color></b>!";
			// Indicate errors
			GameObject error;
			foreach ((int, string) errorMessage in errorMessageList)
			{
				error = Instantiate(MessagePrefab, ErrorsContent);
				error.GetComponent<UnityEngine.UI.Image>().color = light_red;
				error.GetComponentInChildren<TextMeshProUGUI>().text = errorMessage.Item2;
			}
		}
        else
        {
			// Set title
			string scoreText;
			if (score > 0)
			{
				scoreText = "<b><color=#AAFFAA>" + score.ToString() + "</color></b>/<b><color=#AAFFAA>" + maxScore.ToString() + "</color></b>";
			}
			else
			{
				scoreText = "<b><color=#FFAAAA>" + score.ToString() + "</color></b>/<b><color=#AAFFAA>" + maxScore.ToString() + "</color></b>";
			}
			if (!SerializationManager.IsAlreadyCheckedDiagram(erData_.diagramCode))
			{
				// First check of the ER, the score should be updated and the ER marked as checked
				SerializationManager.SetAlreadyCheckedDiagram(erData_.diagramCode);
				errorPanelTittle.text = "Comprobar este diagrama por primera vez te ha otorgado " + scoreText + " puntos!";
			}
			else
			{
				errorPanelTittle.text = "Puntuación " + scoreText + " puntos.\nDebido a que este diagrama ya fue calificado, el resultado mostrado es meramente informativo.";
			}
			// Indicate awarded points per element
			if (correctElements != null)
			{
				GameObject correctElement;
				correctElement = Instantiate(MessagePrefab, ErrorsContent);
				correctElement.GetComponent<UnityEngine.UI.Image>().color = light_green;
				correctElement.GetComponentInChildren<TextMeshProUGUI>().text = "<b><color=#AAFFAA>+" + SerializationManager.config.BasePoints + "</color></b> Puntos de base";

				// There are awarded points on the diagram
				foreach ((int, string) correctElementMessage in correctElements)
				{
					correctElement = Instantiate(MessagePrefab, ErrorsContent);
					correctElement.GetComponent<UnityEngine.UI.Image>().color = light_green;
					correctElement.GetComponentInChildren<TextMeshProUGUI>().text = "<b><color=#AAFFAA>+" + correctElementMessage.Item1 + "</color></b> " + correctElementMessage.Item2;
				}
			}
			// Check if there are errors
			if (errorMessageList.Count == 0)
			{
				// There are no errors on the diagram
				GameObject error = Instantiate(MessagePrefab, ErrorsContent);
				error.GetComponent<UnityEngine.UI.Image>().color = light_yellow;

				if (GameDiagramManager.IsSolutionUnlocked(erData_.diagramCode))
				{
					error.GetComponentInChildren<TextMeshProUGUI>().text = "No parece haber errores graves, de todas formas, puedes ver la solución propuesta a este diagrama desde el móvil";
				}
				else
				{
					error.GetComponentInChildren<TextMeshProUGUI>().text = "No parece haber errores graves";
				}
				// Room is completed
				List<string> roomsUnlocked = GameDiagramManager.RoomCompleted(erData_.diagramCode);
				SerializationManager.playerData.completedRooms.Add(erData_.diagramCode);
				if (roomsUnlocked.Count > 0)
				{
					string completeMessage = unlockingMessage;
					foreach (string room in roomsUnlocked)
					{
						completeMessage += "[" + room + "]";
					}
					overlayMenu.DisplayMessage(completeMessage, displayMessageTimer);
				}
			}
			else
			{
				// There are errors on the diagram
				GameObject error;
				foreach ((int, string) errorMessage in errorMessageList)
				{
					error = Instantiate(MessagePrefab, ErrorsContent);
					error.GetComponent<UnityEngine.UI.Image>().color = light_red;
					error.GetComponentInChildren<TextMeshProUGUI>().text = "<b><color=#FFAAAA>" + errorMessage.Item1 + "</color></b>" + errorMessage.Item2;
				}
			}
		}
		errorPanel.SetActive(true);
		EventHandler.EvaluateDiagram(erData_, score, maxScore);
		SceneHandler.Pause(true);
	}
	public void CloseErrorPanel()
	{
		errorPanel.SetActive(false);
		SceneHandler.Pause(false);
	}
	//========================================= [ Node Edition ] ======================================================
	public void RenameGeneralNode()
	{
		RenameNode(nameList.value, nameList.options[nameList.value].text);
	}
	public void RenameAttributeNode()
	{
		RenameNode(nameListAttr.value, nameListAttr.options[nameListAttr.value].text);
	}
	private void RenameNode(int optionIndex, string newName)
    {
		string oldName;
		selectedNode.name = newName;
		NodeData node = erDiagram_.Rename(selectedNode, out oldName);
		// Update name list
		if (optionIndex != 0)
		{
			nameList.options.RemoveAt(optionIndex);
			nameListAttr.options.RemoveAt(optionIndex);
		}
		if (oldName != SerializationManager.config.unnamedNode)
		{
			nameList.options.Insert(1, new Dropdown.OptionData(oldName));
			nameListAttr.options.Insert(1, new Dropdown.OptionData(oldName));
		}
		//Refresh Values
		nameList.SetValueWithoutNotify(nameList.options.Count);
		nameListAttr.SetValueWithoutNotify(nameList.options.Count);
		// Create new action event
		EventHandler.ChangeName(node, oldName);
	}
	public void SwitchAttributeAsKey()
	{
		bool isKey = erDiagram_.ObjToNode(selectedNode).SwitchIsKey();
		erDiagram_.UnderlineName(selectedNode, isKey);
	}

	public void AddNode(NodeTypeClass typeClass) {
		Vector2 nodePos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(scroll.content, selectedPos, mainCamera, out nodePos);
		NodeData nodeData = new NodeData(typeClass.nodeType, nodePos, erData_.lastId++);
		erDiagram_.DrawNode(nodeData);
		erData_.AddNodeData(nodeData);
		EventHandler.AddNode(nodeData);
	}

	public void RemNode(){
		if(selectedNode != null) {
			List<LinkData> linksToBeRemoved = new List<LinkData>();
			NodeData nodeToRem = erDiagram_.RemNode(selectedNode, out linksToBeRemoved);
			// Return eliminated name to name list
			if (nodeToRem.nodeName != SerializationManager.config.unnamedNode)
			{
				nameList.options.Insert(1, new Dropdown.OptionData(nodeToRem.nodeName));
				nameListAttr.options.Insert(1, new Dropdown.OptionData(nodeToRem.nodeName));
			}
			// Remove All Links attached
			foreach (LinkData link in linksToBeRemoved) {
				erData_.RemoveLink(link);
			}
			// Remove Node
			EventHandler.RemNode(nodeToRem); //Logs the action
			erData_.RemoveNodeData(nodeToRem); //Removes the node
		}
	}

	//========================================= [ Link Edition ] ======================================================
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
					EventHandler.AddLink(newLink);
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
					EventHandler.AddLink(newLink);
				}
			}
		}
		else if (node1.type == NodeData.NodeType.Attribute && node2.type == NodeData.NodeType.Attribute)
		{
			//Only allow links between the same node types if they are attributes
			newLink = new LinkData(node1, node2, LinkData.LinkTypes.AttrAttr);
			erData_.CreateLink(newLink);
			erDiagram_.DrawLink(newLink);
			EventHandler.AddLink(newLink);
		}
	}
	private void RemLink(GameObject node1, GameObject node2){
		LinkData link = erDiagram_.RemLink(node1, node2);
		EventHandler.RemLink(link);
		erData_.RemoveLink(link);
	}
	//========================================= [ Click Mode Transitions ] ======================================================
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
