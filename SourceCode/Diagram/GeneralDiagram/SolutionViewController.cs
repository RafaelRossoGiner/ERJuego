using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SolutionViewController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	[SerializeField]
	private DiagramEditor erDiagram_;

	[SerializeField]
	private ScrollRect scroll;

	// Dragging variables
	private Vector2 prevPos;
	private Vector2 startMouse;
	private Vector2 resolutionSize;

	// Zooming
	public void Update()
	{
		float wheelScrollVelocity = Input.GetAxis("Mouse ScrollWheel");
		if (wheelScrollVelocity != 0f) // forward
		{
			erDiagram_.Zoom(wheelScrollVelocity);
			erDiagram_.RedDrawLinks();
		}
		//Drag Initialization
		resolutionSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
	}

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
			scroll.horizontalNormalizedPosition = finalPos.x;
			scroll.verticalNormalizedPosition = finalPos.y;
		}
		erDiagram_.RedDrawLinks();
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		erDiagram_.RedDrawLinks();
	}
}
