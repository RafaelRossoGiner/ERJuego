using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public int score;
    public string username;
    public List<string> completedRooms;
	public Dictionary<string, bool> diagramAlreadyChecked;

	public PlayerData()
	{
		// Create empty configuration object
		score = 0;
		username = "";
		completedRooms = new List<string>();
		diagramAlreadyChecked = new Dictionary<string, bool>();
	}
}
