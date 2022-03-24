using UnityEngine;
using UnityEngine.UI.Extensions;
using TMPro;
using System.Collections.Generic;
public class OverlayMenu : SimpleMenu<OverlayMenu>
{ 
	[SerializeField]
	private GameObject messagePanel;
	[SerializeField]
	private TextMeshProUGUI scoreText;

	float timer; // Controls time left until display message dissapear
	private List<KeyValuePair<string, float>> messagesLists;

    void Start()
    {
		Interactable.SetOverlayMenu(this);
		DiagramMenuController.SetOverlayMenu(this);
		timer = 0f;
		scoreText.text = SerializationManager.playerData.score.ToString();

		messagesLists = new List<KeyValuePair<string, float>>();
	}

    void Update()
	{
		if (messagesLists.Count > 0)
        {
			if (timer > 0)
				timer -= Time.deltaTime;
			else
				NextMessage();
		}
			
		if (!Input.anyKey)
			return;
		else if (Input.GetKeyDown(KeyCode.F))
		{
			OpenPhone();
		}
	}
	public void OpenSettings()
    {
		SceneHandler.Pause(true);
		Hide();
		PauseMenu.Show();
		EventHandler.OpenMenu();
	}
	public void OpenPhone()
    {
		SceneHandler.Pause(true);
		Hide();
		PhoneMenu.Show();
		EventHandler.OpenPhone();
	}

	public void DisplayMessage(string message, float seconds)
    {
		messagesLists.Add(new KeyValuePair<string, float>(message, seconds));
		if (messagesLists.Count == 1)
        {
			messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = message;
			messagePanel.SetActive(true);
			timer = seconds;
		}
	}
	public void NextMessage()
	{

		if (messagesLists.Count > 0)
		{
			// Eliminate old message
			messagesLists.RemoveAt(0);
		}
		if (messagesLists.Count > 0)
        {
			// Get new message
			KeyValuePair<string, float> messageInfo = messagesLists[0];

			// Display message
			messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = messageInfo.Key;
			messagePanel.SetActive(true);
			timer = messageInfo.Value;
        }
        else
        {
			messagePanel.SetActive(false);
			timer = 0;
		}
	}
	public void AddScorePoints(int increment)
    {
		SerializationManager.playerData.score += increment;
		scoreText.text = SerializationManager.playerData.score.ToString();
	}

	public override void OnBackPressed()
	{
		OpenSettings();
	}
}
