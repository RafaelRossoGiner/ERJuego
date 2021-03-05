using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class JsonFileUtility
{

    public static string LoadJsonFromFile(string path, bool isResource)
    {
        if(isResource)
        {
            return LoadJsonAsResource(path);
        }
        else
        {
            return LoadJsonAsExternalResource(path);
        }
    }

    public static string LoadJsonAsResource(string path)
    {
        string jsonFilePath = path.Replace(".json", "");
        TextAsset loadedJsonfile = Resources.Load<TextAsset>(jsonFilePath);
        return loadedJsonfile.text;
    }

    public static string LoadJsonAsExternalResource(string path)
    {
        path = Path.GetDirectoryName(Application.dataPath) + '/' + path;
        if(!System.IO.File.Exists(path))
        {
            return null;
        }
        else
        {
            StreamReader reader = new StreamReader(path);
            string response = "";
            while(!reader.EndOfStream)
            {
                response += reader.ReadLine();
            }
            return response;
        }
    }

    public static void WriteJsonToExternalResource(string path, string content)
    {
        string filepath = Path.Combine(Application.persistentDataPath, path);
        using (StreamWriter stream = File.CreateText(filepath)){
            byte[] contentBytes = new UTF8Encoding(true).GetBytes(content);
            stream.Write(content);
        }
    }
}
