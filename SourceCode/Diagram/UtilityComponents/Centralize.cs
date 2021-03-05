using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Centralize : MonoBehaviour, IDragHandler
{
	public RectTransform[] rcts;
	RectTransform myTransform;

	public void OnDrag(PointerEventData eventData)
	{
		//Prevents Dragging of the scroll
	}

	private void Start(){
		myTransform = GetComponent<RectTransform>();
	}

	private void Update(){
		myTransform.anchoredPosition = rcts[1].anchoredPosition + (rcts[0].anchoredPosition - rcts[1].anchoredPosition) / 2;
	}
}
