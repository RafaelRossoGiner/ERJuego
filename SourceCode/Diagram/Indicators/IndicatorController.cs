using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

public abstract class IndicatorController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField]
	Sprite simpleLineSpr;
	[SerializeField]
	Sprite doubleLineSpr;
	[SerializeField]
	protected GameObject linePrefab;

	protected Image mySprite;
	protected RectTransform myRect;
	[SerializeField]
	protected LinkData linkAttached;
	protected List<GameObject> lines;

	protected virtual void Awake(){
		mySprite = GetComponent<Image>();
		myRect = GetComponent<RectTransform>();
		lines = new List<GameObject>();
	}

	public virtual void Initialize(RectTransform[] rectTransforms, Transform linksContent, LinkData link) {
		linkAttached = link;
		foreach (RectTransform rt in rectTransforms) {
			GameObject line = Instantiate(linePrefab, linksContent);
			line.GetComponent<UILineConnector>().transforms = new RectTransform[] { rt, myRect };
			lines.Add(line);
		}
		UpdateLineSprite();
	}

	public void OnPointerEnter(PointerEventData eventData){
		mySprite.color = Color.yellow;
	}

	public void OnPointerExit(PointerEventData eventData){
		mySprite.color = Color.white;
	}
	public void UpdateLineSprite()
	{
		if(linkAttached.participationIsTotal)
			foreach (GameObject line in lines)
			{
				line.GetComponent<UILineRenderer>().sprite = doubleLineSpr;
			}
		else
			foreach (GameObject line in lines)
			{
				line.GetComponent<UILineRenderer>().sprite = simpleLineSpr;
			}
	}

	void OnDestroy(){
		foreach (GameObject line in lines) {
			Destroy(line);
		}
	}
}