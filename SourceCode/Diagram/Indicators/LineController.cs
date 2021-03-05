using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class LineController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
	[SerializeField]
	Sprite simpleLineSpr;
	[SerializeField]
	Sprite doubleLineSpr;

	private UILineRenderer lineRend;
	private LinkData linkAttached;

	void Awake()
    {
		lineRend = GetComponent<UILineRenderer>();
	}
	public void Initialize(RectTransform[] rectTransformst, Sprite simple, Sprite doub, LinkData link)
	{
		linkAttached = link;
		GetComponent<UILineConnector>().transforms = rectTransformst;
		simpleLineSpr = simple;
		doubleLineSpr = doub;
		UpdateSprite();
	}
	public void OnPointerClick(PointerEventData eventData)
	{
		Debug.Log("Click On Line");
		linkAttached.participationIsTotal = !linkAttached.participationIsTotal;
		UpdateSprite();
	}
	public void UpdateSprite()
	{
		Debug.Log("Line Changed");
		if (linkAttached.participationIsTotal) lineRend.sprite = doubleLineSpr;
		else lineRend.sprite = simpleLineSpr;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Debug.Log("Entrando Linea");
	}
}
