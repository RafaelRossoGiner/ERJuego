using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MessageData
{
	public enum State { Incompleted, Completed, NotRelated};
	public State state;
	public string entityCode;
	public string message;
	public List<string> highlights;

	string richMessage;

	MessageData()
	{
		highlights = new List<string>();
	}

	public string GetRichText() { return richMessage; }
	public string GetSender() { return entityCode; }

	public void HighlightText()
	{
		richMessage = message + " ";
		foreach(string word in highlights)
		{
			richMessage = richMessage.Replace(word+" ", "<b><color=#078310>" + word+"</color></b>"+" "); 
		}
	}
}
