using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Unity;

public class SerializationManager : MonoBehaviour
{
    [SerializeField]
    static public ConfigData config;
    static public PlayerData playerData;

    private static string genDir;
    private static string genDataDir;
    private static string savesDir;
    private static string predefDir;
    private static string logDir;
    private static string solDir;
    private static string configPath;
    private static string playerDataPath;

    // Properties
    public static ConfigData Config { get { return config; } }
    public static PlayerData PlayerData { get { return playerData; } }
    public static string GeneralDir { get { return genDir; } }
    public static string GeneralDataDir { get { return genDataDir; } }
    public static string SavesDir { get { return savesDir; } }
    public static string PredefinedDir { get { return predefDir; } }
    public static string LogDir { get { return logDir; } }
    public static string SolutionDir { get { return solDir; } }


    public void Start()
    {
        DontDestroyOnLoad(this);
        // Path initialization
        genDir = Path.Combine(Application.dataPath, "Resources");
        genDataDir = Path.Combine(genDir, "Data");

        savesDir = Path.Combine(genDataDir, "Saves");
        logDir = Path.Combine(genDataDir, "Logs");
        playerDataPath = Path.Combine(genDataDir, "PlayerData.dat");

        predefDir = Path.Combine(genDataDir, "Predefined");
        solDir = Path.Combine(genDataDir, "Solutions");
        configPath = Path.Combine(genDataDir, "Config.json");

        // Initialize instance of the EventHandler, neccesary for serialization of the json events
        EventHandler.instance = new EventHandler();
        SceneHandler.Pause(false);
        
        // Check for missing directories or files and create them
        CheckAndRepairFolders();

        // Load DATA
        LoadConfig();
        GameDiagramManager.SetRoomLocks(config.neededRooms);
        GameDiagramManager.LoadPredefinedDiagrams(predefDir); // load pre-configurated diagrams
        GameDiagramManager.LoadSavedDiagrams(savesDir); // load and overwrite with stored diagrams
        GameDiagramManager.LoadAllSolutions(solDir);
        // Load Player Data - Should be loaded at the end
        LoadPlayerData();
    }

    static public void CheckAndRepairFolders()
	{
        if (!Directory.Exists(genDataDir))
        {
            Directory.CreateDirectory(genDataDir);
        }
        if (!Directory.Exists(savesDir))
		{
            Debug.Log("Saves folder not found, creating folder");
            Directory.CreateDirectory(savesDir);
		}
        if (!Directory.Exists(predefDir))
        {
            Debug.Log("Predefined folder not found, creating folder and copying");
            Directory.CreateDirectory(predefDir);
        }
        if (!Directory.Exists(logDir))
		{
            Debug.Log("Logs folder not found, creating folder");
            Directory.CreateDirectory(logDir);
        }
        if (!Directory.Exists(solDir))
        {
            Debug.Log("Solutions folder not found, creating folder");
            Directory.CreateDirectory(solDir);
        }
        if (!File.Exists(configPath))
        {
            Debug.Log("Config not found on " + configPath);
            SaveConfig(new ConfigData(true));
        }
        if (!File.Exists(playerDataPath))
        {
            playerData = new PlayerData();
            SavePlayerData();
        }
    }
    //========================================= [ Config Serialization ] ===================================================
    public static void SaveConfig(ConfigData config)
    {
        using (FileStream stream = new FileStream(configPath, FileMode.Create, FileAccess.Write))
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                // Create and initialize configuration with examples (indicated by passing true in the constructor)
                ConfigData template = new ConfigData(true);

                // Write the template configuration
                string jsonString = JsonConvert.SerializeObject(template, Formatting.Indented);
                writer.Write(jsonString);
            }
        }
    }
    public static bool LoadConfig()
    {
        if (!File.Exists(configPath))
        { 
            return false;
        }
        else
        {
            using (FileStream stream = new FileStream(configPath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string jsonString = reader.ReadToEnd();
                    config = JsonConvert.DeserializeObject<ConfigData>(jsonString);
                }
            }
            foreach (KeyValuePair<string, List<MessageData>> sender in config.messages)
            {
                foreach (MessageData message in sender.Value)
                {
                    message.InitializeMessage(sender.Key);
                    message.state = MessageData.MessageStates.Incompleted;
                }
            }
            return true;
        }
    }
    //========================================= [ Player Progress Manipulation ] ==========================================
    public static bool IsAlreadyCheckedDiagram(string roomCode)
    {
        bool isChecked;
        if (!playerData.diagramAlreadyChecked.TryGetValue(roomCode, out isChecked))
        {
            isChecked = false;
        }
        return isChecked;
    }
    public static void SetAlreadyCheckedDiagram(string roomCode)
    {
        playerData.diagramAlreadyChecked[roomCode] = true;
    }
    //========================================= [ Player Progress Serialization ] ==========================================
    public static void SavePlayerData()
    {
        // Serialize and write the file in the path
        string jsonString = JsonConvert.SerializeObject(playerData, Formatting.Indented);
        Encriptor.EncryptDataToFile(playerDataPath, jsonString);
    }
    public static bool LoadPlayerData()
    {
        if (!File.Exists(playerDataPath))
        {
            return false;
        }
        else
        {
            string jsonString;
            if (Encriptor.DecryptDataFromFile(playerDataPath, out jsonString))
            {
                // Could decrypt
                playerData = JsonConvert.DeserializeObject<PlayerData>(jsonString);
            }
            else
            {
                // Could not decrypt
                Debug.LogWarning("Could not decrypt player progress file");
            }
            foreach (string roomCompleted in playerData.completedRooms)
            {
                GameDiagramManager.RoomCompleted(roomCompleted);
            }
            return true;
        }
    }
    //========================================= [ Log Serialization ] ======================================================
    //Generate Log file
    public static void SaveLog(EventHandler.DataLog dataLog)
    {
        string playerCaseID = EventHandler.PlayerCaseId;

        // Create Folder to store all related Logs
        string logInstanceFolder = Path.Combine(logDir, "ActionLogFile-" + playerCaseID);
        if (!Directory.Exists(logInstanceFolder))
        {
            Directory.CreateDirectory(logInstanceFolder);
        }

        // Create path to store the log and name the file with an identifier
        string path = Path.Combine(logInstanceFolder, "ActionLogFile-" + playerCaseID + ".json");
        // Serialize and write the file in said path
        using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                string jsonString = JsonConvert.SerializeObject(dataLog.actions, Formatting.Indented);
                writer.Write(jsonString);
            }
        }
        // Create path to store the log and name the file with an identifier
        path = Path.Combine(logInstanceFolder, "DiagramsLogFile-" + playerCaseID + ".json");
        // Serialize and write the file in said path
        using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                string jsonString = JsonConvert.SerializeObject(dataLog.diagrams, Formatting.Indented);
                writer.Write(jsonString);
            }
        }
    }
    //========================================= [ ER Serialization ] ======================================================
    public static void SaveER(ERData erData)
    {
        string path = Path.Combine(savesDir, "Diagram-" + erData.diagramCode + ".dat");
        string jsonString = JsonConvert.SerializeObject(erData, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        if (!Encriptor.EncryptDataToFile(path, jsonString))
        {
            // Could not encrypt
            Debug.LogWarning("Error while trying to encrypt " + path);
        }

        // Non encrypted, old save
        //using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
        //{
        //    using (StreamWriter writer = new StreamWriter(stream))
        //    {
        //        jsonString = JsonConvert.SerializeObject(erData, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }); ;
        //        writer.Write(jsonString);
        //    }
        //}
        EventHandler.SaveDiagram(erData);
    }
    public static void DeleteER(ERData erData)
    {
        string path = Path.Combine(savesDir, "Diagram-" + erData.diagramCode + ".dat");
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        EventHandler.DeleteDiagram(erData);
    }

    public static ERData LoadER(string fullFilePath)
    {
        ERData erData = null;
        if (!File.Exists(fullFilePath))
        {
            Debug.LogWarning("ER file " + fullFilePath + " is not found");
            return null;
        }else if (!fullFilePath.EndsWith(".dat"))
        {
            Debug.LogWarning("ER file " + fullFilePath + " will be ignored because of the extension type");
            return null;
        }
        else
        {
            if (Encriptor.DecryptDataFromFile(fullFilePath, out string jsonString))
            {
                // Could decrypt
                erData = JsonConvert.DeserializeObject<ERData>(jsonString);
            }
            else
            {
                // Could not decrypt
                Debug.LogWarning("Could not decrypt file " + fullFilePath);
            }
        }
        return erData;
    }
}
