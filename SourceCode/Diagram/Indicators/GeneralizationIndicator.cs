using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

public class GeneralizationIndicator : IndicatorController, IPointerClickHandler
{
	[SerializeField]
	private List<string> states;
	private int index;

	private Text textComp;
	private Centralize myCent;
	private Image lineImage;

	protected override void Awake()
	{
		base.Awake();
		textComp = transform.GetComponentInChildren<Text>();
		myCent = GetComponent<Centralize>();
	}

	public override void Initialize(RectTransform[] rectTransforms, Transform linksContent, LinkData link)
	{
		linkAttached = link;
		GameObject line = Instantiate(linePrefab, linksContent);
		line.GetComponent<UILineConnector>().transforms = rectTransforms;
		lines.Add(line);

		myCent.rcts = rectTransforms;
		index = states.IndexOf(link.nodeState);
		if (index == -1)
		{
			//Not cardinality found
			index = 0;
		}

		if (linkAttached.type == LinkData.LinkTypes.Specialization)
			linkAttached.type = LinkData.LinkTypes.Generalization;
		else
			linkAttached.type = LinkData.LinkTypes.Specialization;
		
		link.nodeState = states[index];
		textComp.text = states[index];
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			//Go to next state
			if (index == states.Count - 1) index = 0; else index++;
			textComp.text = states[index];
			linkAttached.nodeState = states[index];
			if (linkAttached.type == LinkData.LinkTypes.Specialization)
				linkAttached.type = LinkData.LinkTypes.Generalization;
			else
				linkAttached.type = LinkData.LinkTypes.Specialization;
		}
		else if (eventData.button == PointerEventData.InputButton.Right)
		{
			linkAttached.participationIsTotal = !linkAttached.participationIsTotal;
			UpdateLineSprite();
		}
	}
}
