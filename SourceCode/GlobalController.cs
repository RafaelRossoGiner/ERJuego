using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class GlobalController : MonoBehaviour
{
    [SerializeField]
    static public ConfigData config;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        //Initialize instance of the EventHandler, neccesary for serialization of the json events
        PlayerEventHandler.instance = new PlayerEventHandler();
        SceneHandler.Pause(false);
        CheckAndRepairFolders();
        //Load messages from 
        Debug.Log("Loading");
        LoadConfig();
    }

    public void CheckAndRepairFolders()
	{
        string saves = Path.Combine(Application.persistentDataPath, "Saves");
        string logs = Path.Combine(Application.persistentDataPath, "Logs");
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Saves")))
		{
            Debug.Log("Saves not found, creating");
            Directory.CreateDirectory(saves);
		}
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Logs")))
		{
            Debug.Log("Logs not found, creating");
            Directory.CreateDirectory(logs);
        }
    }

    public bool LoadConfig()
    {
        string path = Path.Combine(Application.dataPath, "Config.json");
        if (!File.Exists(path))
        {
            Debug.Log("Not Found 1");
            path = Path.Combine(Application.persistentDataPath, "Config.json");
            if (!File.Exists(path))
            {
                Debug.Log("Not Found 2");
                return false;
            }
        }
        using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                string jsonString = reader.ReadToEnd();
                config = JsonConvert.DeserializeObject<ConfigData>(jsonString);
                Debug.Log("Converted?");
            }
        }
        foreach(KeyValuePair<string, List<MessageData>> sender in config.messages)
		{
            foreach (MessageData message in sender.Value)
            {
                message.HighlightText();
                message.state = MessageData.State.Incompleted;
                Debug.Log("Message Proccesed");
            }
		}
        return true;
    }
}
