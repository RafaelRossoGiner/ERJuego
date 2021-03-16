using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interactable : MonoBehaviour
{
	//Be careful, this component has a custom editor called InteractableEditor, this
	//comes with some disadvantages and drawbacks when editing it's properties.
	//do not change the name of the properties unless you mirror them on the editor script
	public enum ObjectType { Aula, Pasillo, Despacho, Cafeteria, Diagram, None };

	public ObjectType objectType;
	public string diagramCode;
	public Transform respawnTransform;

	static public bool playerNeedsReposition = false;
	static public Vector3 nextRespawnPosition;
	static public Quaternion nextRespawnRotation;

	static public Interactable lastInteractable;

    public void Interact()
	{
		lastInteractable = this;
		switch (objectType) 
		{
			case ObjectType.Aula:
			case ObjectType.Despacho:
				//Move to the specified room and specify a diagram code for that room
				nextRespawnPosition = respawnTransform.position;
				nextRespawnRotation = respawnTransform.rotation;
				SceneHandler.NextRoom(objectType.ToString(), diagramCode);
				break;
			case ObjectType.Diagram:
				SceneHandler.StartDiagram();
				break;
			case ObjectType.Pasillo:
				playerNeedsReposition = true;
				SceneHandler.NextRoom(objectType.ToString());
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

	static public void SetLastInteractableTexture(Texture2D newTexture)
	{
		
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