using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneHandler
{
	public static string previousRoomName;

	private static bool inPause = false; //Game mustn't be paused on start
	private static bool inUIscene = true; //Game should start on the Main Menu
	//This values are set by the player controller, the player controller
	//is re-instanced on each room change so we need to update from the player-controller script
	private static PlayerController playerCont;
	private static OutlineLookAt playerLook;
	public static void SetPlayerComp(GameObject player, Transform camera)
	{
		playerCont = player.GetComponent<PlayerController>();
		playerLook = player.GetComponent<OutlineLookAt>();
	}
	public static void NextRoom(string sceneName)
	{
		EventHandler.RoomMovement(SceneManager.GetActiveScene().name, sceneName);
		GameDiagramManager.SetDiagramCode("");
		SceneManager.LoadScene(sceneName);
		SceneManager.LoadScene("PlayerUI", LoadSceneMode.Additive);
		inUIscene = false;
		Pause(inUIscene);
	}
	public static void NextRoom(string sceneName, string diagramCode)
	{
		EventHandler.RoomMovement(SceneManager.GetActiveScene().name, sceneName);
		GameDiagramManager.SetDiagramCode(diagramCode);
		GameDiagramManager.GetCurrDiagram();
		SceneManager.LoadScene(sceneName);
		SceneManager.LoadScene("PlayerUI", LoadSceneMode.Additive);
		inUIscene = false;
	}
	public static void StartDiagram()
	{
		previousRoomName = SceneManager.GetActiveScene().name;
		EventHandler.RoomMovement(previousRoomName, "Diagram");
		SceneManager.LoadScene("Diagram");
		SceneManager.LoadScene("PlayerUI", LoadSceneMode.Additive);
		inUIscene = true;
	}

	public static void CloseDiagram()
	{
		EventHandler.RoomMovement("Diagram", previousRoomName);
		SceneManager.LoadScene(previousRoomName);
		SceneManager.LoadScene("PlayerUI", LoadSceneMode.Additive);
		inUIscene = false;
		Pause(inUIscene);
	}

	public static bool isDiagram() { return inUIscene; }
	public static bool isPaused() { return inPause; }
	public static void Pause(bool wantToPause)
	{
		//If it's paused, deactivate movement and camera move
		//playerCont.enabled = !pauseValue;
		if (wantToPause)
		{
			CameraController.DisableRotation(true);
			if (playerLook != null)
				playerLook.enabled = false;
			Cursor.lockState = CursorLockMode.None;
		}
		else {
			CameraController.DisableRotation(false);
			if (playerLook != null)
				playerLook.enabled = true;
			if (!inUIscene)
				Cursor.lockState = CursorLockMode.Locked;
		}
		inPause = wantToPause;
	}
}
