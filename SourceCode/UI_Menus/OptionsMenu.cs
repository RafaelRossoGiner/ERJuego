using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using TMPro;
using System.Collections.Generic;

public class OptionsMenu : SimpleMenu<OptionsMenu>
{
	[SerializeField]
	private Slider sensibilitySlider;
	[SerializeField]
	private TextMeshProUGUI sensibilityText;
	[SerializeField]
	private TMP_Dropdown resolutionDropwdown;

	private List<Resolution> validResolutions;

	//Sensibility option disabled due to some issues and the incoming dead line of the project

	public void Start() 
	{
		//Setup Sensibility Slider
		sensibilitySlider.SetValueWithoutNotify(CameraController.GetSensibility());

		/* 
		List<Resolution> resolutions = new List<Resolution>(Screen.resolutions);
		validResolutions = new List<Resolution>();
		foreach(Resolution res in resolutions)
		{
			float aspect = res.width / (float)res.height;
			if (aspect >= 1.6f && aspect < 1.78f)
			{
				validResolutions.Add(res);
			}
		}

		resolutionDropwdown.ClearOptions();

		List<string> options = new List<string>();

		int resIndex = 0;
		for (int i = 0; i < validResolutions.Count; i++)
		{
			string option = validResolutions[i].width + " x " + validResolutions[i].height;
			options.Add(option);

			if (validResolutions[i].width == Screen.width && validResolutions[i].height == Screen.height)
			{
				resIndex = i;
			}

		}

		resolutionDropwdown.AddOptions(options);
		resolutionDropwdown.value = resIndex;
		resolutionDropwdown.RefreshShownValue();
		*/
	}
	/*
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.I))
			Screen.SetResolution(1920, 1080, Screen.fullScreen);
	}
	public void SetResolution(int resIndex)
	{
		Resolution resolution = validResolutions[resIndex];
		Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
	}
	*/
	
	public void SetFullScreen(bool isFullScreen)
	{
		Screen.fullScreen = isFullScreen;
	}
	
	public void OnSensibilityChanged(System.Single value)
	{
		CameraController.SetSensibility(value);
		sensibilityText.SetText((value / 10).ToString());
		CameraController.DisableRotation(true);
	}
	public void OnInformation()
	{
		Hide();
		InformationMenu.Show();
	}
	public void OnGoBack()
	{
		Hide();
		PauseMenu.Show();
	}
	public override void OnBackPressed()
	{
		SceneHandler.Pause(false);
		Hide();
		OverlayMenu.Show();
		EventHandler.OpenMenu();
	}
}