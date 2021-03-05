using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[System.Serializable]
public struct DirectoryStructure
{
    public string[] directories;
}

public class DirectoryManager : MonoBehaviour
{
    public DirectoryStructure fileStruct;
    public static bool correctStructure = false;
    public static bool existingFiles = false;

    void Awake()
    {
        fileStruct = JsonUtility.FromJson<DirectoryStructure>(JsonFileUtility.LoadJsonFromFile("DirectoryStructure.json", true));
        bool okStr = DirectoryManager.CheckDirectoryStructure(fileStruct.directories);
        if (!okStr)
        {
            Application.Quit();
        }
    }

    public static bool CheckDirectoryStructure(string[] directories)
    {
        correctStructure = true;
        foreach(string directory in directories)
        {
            if (!Directory.Exists(Path.GetDirectoryName(Application.dataPath) + '/' + directory))
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(Application.dataPath) + '/' + directory);
                }
                catch(Exception e)
                {
                    correctStructure = false;
                }
            }
            if (IsDirectoryEmpty(Path.GetDirectoryName(Application.dataPath) + '/' + directory))
            {

            }
        }
        return correctStructure;
    }

    public static bool CheckFileStructure(string[] directories)
    {
        existingFiles = true;
        foreach (string directory in directories)
        {

        }
        return existingFiles;
    }
    public void SetDefaultFileStructure()
    {

    }

    public static bool IsDirectoryEmpty(string path)
    {
        return Directory.GetFiles(path).Length == 0;
    }
}