using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Newtonsoft.Json;
using System.IO;

[System.Serializable]
public class ERData
{
    public int lastId; //The highest integer id used in this Diagram
    public string diagramCode;
    public List<NodeData> nodes;
    public List<LinkData> links;

    public ERData()
    {
        nodes = new List<NodeData>();
        links = new List<LinkData>();
        lastId = 0;
    }
    //Copy Constructor
    public void CopyER(ERData data)
    {
        lastId = data.lastId;
        diagramCode = data.diagramCode;
        nodes = data.nodes;
        links = data.links;
    }

    public void AddNodeData(NodeData node)
    {
        nodes.Add(node);
        lastId++; 
        //Increase the id number to avoid repeating ids
    }

    public void RemoveNodeData(NodeData node)
    {
        nodes.Remove(node);
    }

    public void CreateLink(LinkData link)
    {
        links.Add(link);
    }

    public void RemoveLink(LinkData link)
    {
        links.Remove(link);
    }
    //Find a node by its id or null it does not exist
    public NodeData Find(int id)
	{
        foreach(NodeData node in nodes)
		{
            if (node.id == id)
                return node;
		}
        return null;
	}
    //Links involving 2 nodes such as Entity-Relation or Entity-Atribute
    public int FreeLinkSlots(NodeData node1, NodeData node2, LinkData.LinkTypes type)
    {
        int porEncontrar = (type == LinkData.LinkTypes.EntityRel) ? 2 : 1;
        foreach (LinkData currLink in links)
        {
            if (currLink.linkedNodeId[0] == node1.id && currLink.linkedNodeId[1] == node2.id ||
            currLink.linkedNodeId[1] == node1.id && currLink.linkedNodeId[0] == node2.id)
            {
                if (--porEncontrar == 0)
                {
                    break;
                }
            }
        }
        return porEncontrar;
    }

    public void ChangeName(NodeData node, string newName)
    {
        node.Rename(newName);
    }

    public void Save()
    {
        string path = Path.Combine(Path.Combine(Application.persistentDataPath, "Saves"), "Diagram-" + diagramCode + ".json");
        using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                string jsonString = JsonConvert.SerializeObject(this);
                writer.Write(jsonString);
            }
        }
        PlayerEventHandler.SaveDiagram(this);
    }
    public void Delete()
    {
        string path = Path.Combine(Path.Combine(Application.persistentDataPath, "Saves"), "Diagram-" + diagramCode + ".json");
		if (File.Exists(path))
		{
            Debug.Log("Delete");
            File.Delete(path);
		}
        PlayerEventHandler.DeleteDiagram(this);
    }

    public bool Load(string fileName)
    {
        //Probably there is a better way to initialize an object on itself, maybe from another object. 
        //Revise later when there is time available.
        string path = Path.Combine(Application.persistentDataPath, Path.Combine("Saves", fileName));
        if (!File.Exists(path))
        {
            return false;
        }
        using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                string jsonString = reader.ReadToEnd();
                this.CopyER(JsonConvert.DeserializeObject<ERData>(jsonString));
            }
        }
        return true;
    }
}
