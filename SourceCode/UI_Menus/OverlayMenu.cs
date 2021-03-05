using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.EventSystems;

public class OverlayMenu : SimpleMenu<OverlayMenu>
{ 
	private void Update()
	{
		if (!Input.anyKey)
			return;

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			SceneHandler.Pause(true);
			Hide();
			PauseMenu.Show();
			PlayerEventHandler.OpenMenu();
		}
		else if (Input.GetKeyDown(KeyCode.F))
		{
			SceneHandler.Pause(true);
			Hide();
			PhoneMenu.Show();
			PlayerEventHandler.OpenPhone();
		}
	}
}
