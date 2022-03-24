using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

public class ParticipationIndicator : IndicatorController, IPointerClickHandler
{
	private Centralize myCent;

	protected override void Awake()
	{
		base.Awake();
		myCent = GetComponent<Centralize>();
	}

	public override void Initialize(RectTransform[] rectTransforms, Transform linksContent, LinkData link)
	{
		linkAttached = link;
		GameObject line = Instantiate(linePrefab, linksContent);
		line.GetComponent<UILineConnector>().transforms = rectTransforms;
		lines.Add(line);

		myCent.rcts = rectTransforms;
		UpdateLineSprite();
	}
	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left || eventData.button == PointerEventData.InputButton.Right)
		{
			linkAttached.participationIsTotal = !linkAttached.participationIsTotal;
			UpdateLineSprite();
		}
	}
}

