using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConfigData
{
    public List<string> nodeNames;
    public Dictionary<string, List<MessageData>> messages;
}
