using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interactable : MonoBehaviour
{
	public enum ObjectType { Aula, Pasillo, Despacho, Cafeteria, Diagram, None };

	public ObjectType objectType;
	public string diagramCode;
    public void Interact()
	{
		switch (objectType) 
		{
			case ObjectType.Aula:
			case ObjectType.Despacho:
				//Move to the specified room and specify a diagram code for that room
				SceneHandler.NextRoom(objectType.ToString(), diagramCode);
				break;
			case ObjectType.Diagram:
				SceneHandler.StartDiagram();
				break;
			case ObjectType.None:
				//Do nothing
				break;
			default:
				//Move to the specified room, nothing else required
				SceneHandler.NextRoom(objectType.ToString());
				break;
		}
	}

	public bool CodeDuplicated()
	{
		Interactable[] interactables = (Interactable[])FindObjectsOfType(typeof(Interactable));
		foreach (Interactable interact in interactables)
		{
			if (interact != this && interact.diagramCode == diagramCode)
				return true;
		}
		return false;
	}
}