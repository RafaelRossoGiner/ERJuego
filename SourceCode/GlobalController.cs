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
        PlayerEventHandler.SetPlayer("DebugPlayer");
        SceneHandler.Pause(false);
        //Load messages from 
        LoadConfig();
    }

    public bool LoadConfig()
    {
        string path = Path.Combine(Application.dataPath, "Config.json");
        if (!File.Exists(path))
        {
            path = Path.Combine(Application.persistentDataPath, "Config.json");
            if (!File.Exists(path))
            {
                return false;
            }
        }
        using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                string jsonString = reader.ReadToEnd();
                config = JsonConvert.DeserializeObject<ConfigData>(jsonString);
            }
        }
        foreach(KeyValuePair<string, List<MessageData>> sender in config.messages)
		{
            foreach (MessageData message in sender.Value)
            {
                message.HighlightText();
                message.state = MessageData.State.Incompleted;
            }
		}
        return true;
    }
}
