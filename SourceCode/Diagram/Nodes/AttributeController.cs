using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttributeController : NodeController
{
    public override void UpdateSprite()
	{
		base.UpdateSprite();
		diagram.UnderlineName(gameObject, nodeAttached.isKey);
	}
	public override void OnPointerClick(PointerEventData eventData)
	{
		if (!beingDragged)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				diagramController.NodeLeftClicked(this);
			}
			else if (eventData.button == PointerEventData.InputButton.Right)
			{
				diagramController.OpenAttributeMenu(eventData);
			}
		}
	}
}
