using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class GeneralizationController : NodeController
{
	[SerializeField]
	private List<string> states;
	private int index;

	public void Initialize(NodeData node, List<string> posibleStates, DiagramController controller, ERDiagram diagram)
	{
		Debug.ClearDeveloperConsole();
		Debug.Log("Called Initialization on GeneralizationController.cs");
		base.Initialize(node, controller, diagram);

		states = posibleStates;
		index = states.IndexOf(nodeAttached.nodeName);
		if (index == -1)
		{
			//Not nodeName found on the list
			index = 0;
		}
		textComp.text = states[index];
	}
	public override void OnPointerClick(PointerEventData eventData)
	{
		if (!beingDragged)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				//Go to next state
				if(diagramController.NodeLeftClicked(this) == DiagramController.Modes.selecting)
				{
					//Only change state if we are not linking/unlinking
					string oldState = nodeAttached.nodeName;
					if (index == states.Count - 1) index = 0; else index++;
					textComp.text = states[index];
					nodeAttached.Rename(states[index]);
					PlayerEventHandler.ChangeGenType(nodeAttached, oldState);
				}
			}
			else if (eventData.button == PointerEventData.InputButton.Right)
			{
				//Go to next state
				diagramController.OpenGeneralizationMenu(eventData);
			}
		}
	}
}
