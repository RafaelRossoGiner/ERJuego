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

	private static Color light_red = new Color(1f, 0.6393762f, 0.6358491f);
	private static Color light_green = new Color(0.4302955f, 0.9622642f, 0.4523566f);
	private static Color light_yellow = new Color(0.983665f, 1f, 0.6352941f);
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
			OnBackPressed();
		}
	}
	public override void OnBackPressed()
	{
		SceneHandler.Pause(false);
		Hide();
		OverlayMenu.Show();
		EventHandler.ClosePhone();
	}
	public void GoToSolution()
	{
		Hide();
		PhoneMenuSolution.Show();
		EventHandler.OpenSolution();
	}

	public void ShowSenders()
	{
		string currentDiagram = GameDiagramManager.GetCurrDiagramCode();
		if (currentDiagram == "")
		{
			// Instantiate all available senders
			foreach (KeyValuePair<string, List<MessageData>> sender in SerializationManager.config.messages)
			{
				//Initialize each sender
				GameObject newSender = Instantiate(SenderPrefab, SenderList);
				//Set text to senders name
				newSender.GetComponentInChildren<TextMeshProUGUI>().text = sender.Key;
				if (GameDiagramManager.IsUnlocked(sender.Key))
				{
					//Assign OnClick function to the button
					newSender.GetComponent<Button>().onClick.AddListener(delegate { ShowMessages(sender.Key); });
				}
				else
				{
					newSender.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f);
					newSender.GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
					newSender.GetComponent<Button>().enabled = false;

				}
			}
		}
        else
        {
			//Initialize each sender
			GameObject newSender = Instantiate(SenderPrefab, SenderList);
			//Set text to senders name
			newSender.GetComponentInChildren<TextMeshProUGUI>().text = currentDiagram;
			newSender.GetComponent<Button>().onClick.AddListener(delegate { ShowMessages(currentDiagram); });
			ShowMessages(currentDiagram);
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
		if (SerializationManager.config.messages.ContainsKey(sender))
		{
			foreach (MessageData message in SerializationManager.config.messages[sender])
			{
				//Initialize each message sent by the sender entity
				GameObject messageObj = Instantiate(MessagePrefab, MessageList);
				//Set text values
				DrawMessage(message, messageObj);
				messageObj.GetComponent<Button>().onClick.AddListener(delegate { MessagedClicked(message, messageObj); });
			}
        }
        else
        {
			GameObject messageObj = Instantiate(MessagePrefab, MessageList);
			messageObj.GetComponent<Image>().color = light_green;
			messageObj.GetComponentInChildren<TextMeshProUGUI>().text = SerializationManager.config.noMessagesNotification;
		}
		EventHandler.SeeMessages(sender);
	}
	//Each message button will call this method with the message object
	void MessagedClicked(MessageData message, GameObject messageObj)
	{
		string oldState = message.state.ToString();
		switch (message.state)
		{
			case MessageData.MessageStates.Incompleted:
				message.state = MessageData.MessageStates.Completed;
				break;
			case MessageData.MessageStates.Completed:
				message.state = MessageData.MessageStates.NotRelated;
				break;
			case MessageData.MessageStates.NotRelated:
				message.state = MessageData.MessageStates.Incompleted;
				break;
			default:
				Debug.LogError("Unidentified message state when a message was clicked on PhoneMenu.cs");
				break;
		}
		DrawMessage(message, messageObj);
		EventHandler.ChangeMessageState(message, oldState);
	}
	void DrawMessage(MessageData message, GameObject messageObj)
	{
		switch (message.state)
		{
			case MessageData.MessageStates.Incompleted:
				messageObj.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
				messageObj.GetComponentInChildren<TextMeshProUGUI>().text = message.GetRichText();
				messageObj.GetComponent<Image>().color = light_red;
				break;
			case MessageData.MessageStates.Completed:
				messageObj.GetComponentInChildren<TextMeshProUGUI>().color = palidText;
				messageObj.GetComponentInChildren<TextMeshProUGUI>().text = message.message;
				messageObj.GetComponent<Image>().color = light_green;
				break;
			case MessageData.MessageStates.NotRelated:
				messageObj.GetComponentInChildren<TextMeshProUGUI>().color = palidText;
				messageObj.GetComponentInChildren<TextMeshProUGUI>().text = message.message;
				messageObj.GetComponent<Image>().color = light_yellow;
				break;
			default:
				Debug.LogError("Unidentified message state when trying to draw a message on PhoneMenu.cs");
				break;
		}
	}
}
