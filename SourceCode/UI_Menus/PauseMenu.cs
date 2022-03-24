using UnityEngine;
using UnityEngine.UI.Extensions;

public class PauseMenu : SimpleMenu<PauseMenu>
{
	public void OnContinue()
	{
		SceneHandler.Pause(false);
		Hide();
		OverlayMenu.Show();
		EventHandler.CloseMenu();
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
    public override void OnBackPressed()
    {
		SceneHandler.Pause(false);
		Hide();
		OverlayMenu.Show();
		EventHandler.CloseMenu();
	}
}
