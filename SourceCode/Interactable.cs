using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Interactable : MonoBehaviour
{
	public enum ObjectType { Aula, Pasillo, Despacho, Cafeteria, Diagram, None };
	public enum PredefinedRoomCode { A1, A2, A3, D1, D2, D3, A4, A5, A6, D4, D5, D6}
	public ObjectType objectType;
	public Transform respawnTransform;

	[SerializeField]
	private PredefinedRoomCode roomCodeSet;
	[SerializeField]
	private float lockedMessageTime = 3f;
	[SerializeField]
	private TextMeshPro plateTextComponent;
	[SerializeField]
	private GameObject diagramPlane;
	[SerializeField]
	private Texture diagramSchemeTexture;

	static public bool playerNeedsReposition = false;
	static public Vector3 nextRespawnPosition;
	static public Quaternion nextRespawnRotation;

	private string lockedMessage = "Esta habitación esta cerrada de momento! Primero debes completar las habitaciones: ";
	private static OverlayMenu overlayMenu;
	
	private string roomCode;

	private void Start()
    {
		roomCode = roomCodeSet.ToString();
		if (objectType == ObjectType.Diagram)
        {
			//Texture planeTexture = DiagramKeeper.GetTexture(roomCode);
			if (GameDiagramManager.IsPopulated(roomCode)) 
			{
				diagramPlane.GetComponent<Renderer>().material.mainTexture = diagramSchemeTexture;
				diagramPlane.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
			}
			else
            {
				diagramPlane.GetComponent<Renderer>().material.mainTexture = null;
				diagramPlane.GetComponent<Renderer>().material.color = new Color(0f, 0f, 0f);
			}				
			roomCode = "";
		} else if (objectType == ObjectType.Pasillo)
        {
			roomCode = "";
		}
		else if (plateTextComponent != null)
        {
			plateTextComponent.text = roomCode;
        }
	}
	public static void SetOverlayMenu(OverlayMenu newMenu)
	{
		overlayMenu = newMenu;
	}
	public string GetRoomCode()
    {
		return roomCode;
    }

	public void Interact()
	{
			switch (objectType)
			{
				case ObjectType.Aula:
				case ObjectType.Despacho:
					if (GameDiagramManager.IsUnlocked(roomCode))
					{
						//Move to the specified room and specify a diagram code for that room
						nextRespawnPosition = respawnTransform.position;
						nextRespawnRotation = respawnTransform.rotation;
						SceneHandler.NextRoom(objectType.ToString(), roomCode);
					}
					else
					{
						string completeMessage = lockedMessage;
						foreach(string room in GameDiagramManager.GetRequiredRoomsToUnlock(roomCode))
						{
							completeMessage += "[" + room + "]";
						}
						overlayMenu.DisplayMessage(completeMessage, lockedMessageTime);
					}
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

	public bool CodeDuplicated()
	{
		Interactable[] interactables = (Interactable[])FindObjectsOfType(typeof(Interactable));
		foreach (Interactable interact in interactables)
		{
			if (interact != this && interact.roomCode == roomCode)
				return true;
		}
		return false;
	}
}