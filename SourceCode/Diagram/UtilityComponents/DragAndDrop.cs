using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour
{
	/*
	private ERDiagram diagram;
	private NodeController nodeCont;
	private RectTransform rct;
	private Vector2 prevPos;
	private Vector2 startMouse;

	private float clickInterval_;
	private float clickThreshold_;
	void Start() {
		rct = GetComponent<RectTransform>();
	}

	public void Initialize(ERDiagram currDiag)
	{
		diagram = currDiag;
	}

	public void Initialize(ERDiagram currDiag, NodeController node, float clickThreshold)
	{
		diagram = currDiag;
		nodeCont = node;
		clickThreshold_ = clickThreshold;
	}

	public void OnBeginDrag(PointerEventData eventData){
		prevPos = rct.anchoredPosition;
		startMouse = eventData.position;
		clickInterval_ = Time.deltaTime;
	}

	public void OnDrag(PointerEventData eventData){
		if (eventData.button != PointerEventData.InputButton.Left)
			return;
		rct.anchoredPosition = prevPos + eventData.position - startMouse;
	}
	public void OnEndDrag(PointerEventData eventData){
		diagram.objToNode[gameObject].position = rct.anchoredPosition;
		if (Time.deltaTime - clickInterval_ < clickThreshold_)
		{
			nodeCont.CustomPointerClick(eventData);
		}
	}*/
}
