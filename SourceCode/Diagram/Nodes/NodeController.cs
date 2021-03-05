using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class NodeController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
	public NodeData nodeAttached;
	public DiagramController diagramController;

	[SerializeField]
	private Sprite nodeSprite, nodeSpriteDoub;
	[SerializeField]
	protected TextMeshProUGUI textComp;
	[SerializeField]
	protected Image imageComp;

	protected ERDiagram diagram;
	private RectTransform rct;
	private Vector2 prevPos;
	private Vector2 startMouse;

	protected bool beingDragged;

	public virtual void Initialize(NodeData node, DiagramController controller, ERDiagram currDiag)
	{
		nodeAttached = node;
		diagramController = controller;
		textComp.text = node.nodeName;

		diagram = currDiag;
		beingDragged = false;

		UpdateSprite();
	}
	public void SwitchContour()
	{
		nodeAttached.doubleLine = !nodeAttached.doubleLine;
		UpdateSprite();
	}
	public virtual void UpdateSprite()
	{
		if (nodeAttached.doubleLine)
		{
			imageComp.sprite = nodeSpriteDoub;
		}
		else
		{
			imageComp.sprite = nodeSprite;
		}
	}
	void Start()
	{
		rct = GetComponent<RectTransform>();
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		prevPos = rct.anchoredPosition;
		startMouse = eventData.position;
		beingDragged = true;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left)
			return;
		rct.anchoredPosition = prevPos + eventData.position - startMouse;
		Debug.Log("Increment");
	}
	public void OnEndDrag(PointerEventData eventData)
	{
		diagram.objToNode[gameObject].position = rct.anchoredPosition;
		beingDragged = false;
	}


	public virtual void OnPointerClick(PointerEventData eventData)
	{
		if (!beingDragged)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				diagramController.NodeLeftClicked(this);
			}
			else if (eventData.button == PointerEventData.InputButton.Right)
			{
				diagramController.OpenNodeEdition(eventData);
			}
		}
	}
}
