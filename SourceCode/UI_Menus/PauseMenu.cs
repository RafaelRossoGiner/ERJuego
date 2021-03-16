using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class PauseMenu : SimpleMenu<PauseMenu>
{
	public void OnContinue()
	{
		SceneHandler.Pause(false);
		Hide();
		OverlayMenu.Show();
		PlayerEventHandler.CloseMenu();
	}
	public void OnOptions()
	{
		Hide();
		OptionsMenu.Show();
	}
	public void OnSaveAndExit()
	{
		Hide();
		ConfirmExitMenu.Show();
	}
	private void Update()
	{
		if (!Input.anyKey)
			return;
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			SceneHandler.Pause(false);
			Hide();
			OverlayMenu.Show();
			PlayerEventHandler.CloseMenu();
		}
	}
}
