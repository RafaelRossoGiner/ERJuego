using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

[System.Serializable]
public class MessageData
{
	public enum MessageStates { Incompleted, Completed, NotRelated};
	public string message;
	public List<string> additionalHighlights;

	[System.NonSerialized]
	public MessageStates state = MessageStates.Incompleted;
	[System.NonSerialized]
	public string sender;

	private string richMessage;
	public MessageData(string setMessage, List<string> setHighlights = null)
	{
		message = setMessage;
		state = MessageStates.Incompleted;
		if (setHighlights == null)
		{
			additionalHighlights = new List<string>();
        }
        else
        {
			additionalHighlights = setHighlights;
        }
	}

	public string GetRichText() { return richMessage; }
	public string GetSender() { return sender; }

	public void InitializeMessage(string setSender)
	{
		sender = setSender;
		if (message[message.Length-1] == '.')
        {
			richMessage = message.Remove(message.Length - 1) + " ";
        }
        else
        {
			richMessage = message + " ";
		}
		if (SerializationManager.config.highlightNodeNames)
		{ 
			foreach (string name in SerializationManager.config.nodeNames)
			{
				richMessage = Regex.Replace(richMessage, name + " ", "<b><color=#078310>" + name + "</color></b>" + " ", RegexOptions.IgnoreCase);
			}
		}
		foreach (string word in additionalHighlights)
		{
			richMessage = Regex.Replace(richMessage, word + " ", "<b><color=#078310>" + word + "</color></b>" + " ", RegexOptions.IgnoreCase);
		}
	}
}
