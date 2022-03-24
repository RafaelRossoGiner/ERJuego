using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class InformationMenu : SimpleMenu<InformationMenu>
{
	public void OnGitHub()
	{
		Application.OpenURL("https://github.com/RafaelRossoGiner/BBDD2");
	}
	public void OnGoBack()
	{
		SceneHandler.Pause(false);
		Hide();
		OverlayMenu.Show();
		EventHandler.OpenMenu();
	}
}
