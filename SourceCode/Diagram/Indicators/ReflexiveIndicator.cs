using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ReflexiveIndicator : IndicatorController, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	private DiagramEditor diagram;
	private Vector2 prevPos;
	private Vector2 startMouse;

	protected override void Awake()
	{
		base.Awake();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			linkAttached.participationIsTotal = !linkAttached.participationIsTotal;
			UpdateLineSprite();
		}
	}
	public void Initialize(DiagramEditor currDiag)
	{
		diagram = currDiag;
		UpdateLineSprite();
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		prevPos = myRect.anchoredPosition;
		startMouse = eventData.position;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left)
			return;
		myRect.anchoredPosition = prevPos + eventData.position - startMouse;
	}
	public void OnEndDrag(PointerEventData eventData)
	{
		//Change reflexive postion on node
		linkAttached.auxRectPos = myRect.anchoredPosition;
	}
}
