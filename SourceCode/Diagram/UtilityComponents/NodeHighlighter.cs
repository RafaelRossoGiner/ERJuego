using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NodeHighlighter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public DiagramMenuController controller;
	private Image mySprite;
	private static List<NodeHighlighter> highlights = new List<NodeHighlighter>();

	public static NodeHighlighter first;
	public void Start() {
		mySprite = GetComponent<Image>();
		highlights.Add(this);
	}

	public static void CancelAllHighlights()
	{
		foreach(NodeHighlighter hl in highlights)
		{
			hl.mySprite.color = Color.white;
		}
	}
	public static void SetFirst(GameObject firstNode, DiagramMenuController.Modes mode)
	{
		first = firstNode.GetComponent<NodeHighlighter>();
		foreach (NodeHighlighter hl in highlights)
		{
			hl.mySprite.color = Color.white;
		}
		switch (mode)
		{
			case DiagramMenuController.Modes.selecting:
				first.mySprite.color = Color.yellow;
				break;
			case DiagramMenuController.Modes.linking:
				first.mySprite.color = Color.green;
				break;
			case DiagramMenuController.Modes.unLinking:
				first.mySprite.color = Color.red;
				break;
		}
	}

	//Mouse Hover
	public void OnPointerEnter(PointerEventData eventData){
		switch (controller.mode)
		{
			case DiagramMenuController.Modes.selecting:
				mySprite.color = Color.yellow;
				break;
			case DiagramMenuController.Modes.linking:
				mySprite.color = Color.green;
				break;
			case DiagramMenuController.Modes.unLinking:
				mySprite.color = Color.red;
				break;
		}
	}
	public void OnPointerExit(PointerEventData eventData){
		if (this != first)
			mySprite.color = Color.white;
	}
}
