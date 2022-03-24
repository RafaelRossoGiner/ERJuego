using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
public class ConfirmExitMenu : SimpleMenu<ConfirmExitMenu>
{
	public void OnCancel()
	{
		Hide();
		PauseMenu.Show();
	}
	public void OnConfirm()
	{
		EventHandler.UserClosedGame();
		SerializationManager.SavePlayerData();
		//Close the app
		Application.Quit();
	}
    public override void OnBackPressed()
    {
		OnCancel();
    }
}
