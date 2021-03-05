using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;

public class PhoneMenu : SimpleMenu<PhoneMenu>
{
	[SerializeField]
	Transform SenderList, MessageList;
	[SerializeField]
	GameObject SenderPrefab, MessagePrefab;

	private static Color light_blue = new Color(0.5792453f, 255f, 0.8980471f);
	private static Color light_green = new Color(0.4302955f, 0.9622642f, 0.4523566f);
	private static Color light_orange = new Color(1f, 0.6844898f, 0.3339622f);
	private static Color palidText = Color.black - new Color(0f, 0f, 0f, 0.4f);

	public void Start()
	{
		ShowSenders();
	}

	public void Update()
	{
		if (!Input.anyKey)
			return;
		if (Input.GetKeyDown(KeyCode.F))
		{
			SceneHandler.Pause(false);
			Hide();
			OverlayMenu.Show();
			PlayerEventHandler.ClosePhone();
		}
	}

	public void ShowSenders()
	{
		foreach (KeyValuePair<string, List<MessageData>> sender in GlobalController.config.messages)
		{
			//Initialize each sender
			GameObject newSender = Instantiate(SenderPrefab, SenderList);
			//Set text to senders name
			newSender.GetComponentInChildren<TextMeshProUGUI>().text = sender.Key;
			//Assign OnClick function to the button
			newSender.GetComponent<Button>().onClick.AddListener(delegate { ShowMessages(sender.Key); });
		}
	}
	//Each sender button will call this method with the sender name
	void ShowMessages(string sender)
	{
		Debug.Log("ShowMessages of: " + sender);
		foreach (Transform child in MessageList)
		{
			Destroy(child.gameObject);
		}
		foreach (MessageData message in GlobalController.config.messages[sender])
		{
			//Initialize each message sent by the sender entity
			GameObject messageObj = Instantiate(MessagePrefab, MessageList);
			//Set text values
			DrawMessage(message, messageObj);
			messageObj.GetComponent<Button>().onClick.AddListener(delegate { MessagedClicked(message, messageObj); });
		}
		PlayerEventHandler.SeeMessages(sender);
	}
	//Each message button will call this method with the message object
	void MessagedClicked(MessageData message, GameObject messageObj)
	{
		string oldState = message.state.ToString();
		switch (message.state)
		{
			case MessageData.State.Incompleted:
				message.state = MessageData.State.Completed;
				break;
			case MessageData.State.Completed:
				message.state = MessageData.State.NotRelated;
				break;
			case MessageData.State.NotRelated:
				message.state = MessageData.State.Incompleted;
				break;
			default:
				Debug.LogError("Unidentified message state when a message was clicked on PhoneMenu.cs");
				break;
		}
		DrawMessage(message, messageObj);
		PlayerEventHandler.ChangeMessageState(message, oldState);
	}
	void DrawMessage(MessageData message, GameObject messageObj)
	{
		switch (message.state)
		{
			case MessageData.State.Incompleted:
				messageObj.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
				messageObj.GetComponentInChildren<TextMeshProUGUI>().text = message.GetRichText();
				messageObj.GetComponent<Image>().color = light_blue;
				break;
			case MessageData.State.Completed:
				messageObj.GetComponentInChildren<TextMeshProUGUI>().color = palidText;
				messageObj.GetComponentInChildren<TextMeshProUGUI>().text = message.message;
				messageObj.GetComponent<Image>().color = light_green;
				break;
			case MessageData.State.NotRelated:
				messageObj.GetComponentInChildren<TextMeshProUGUI>().color = palidText;
				messageObj.GetComponentInChildren<TextMeshProUGUI>().text = message.message;
				messageObj.GetComponent<Image>().color = light_orange;
				break;
			default:
				Debug.LogError("Unidentified message state when trying to draw a message on PhoneMenu.cs");
				break;
		}
	}
}
