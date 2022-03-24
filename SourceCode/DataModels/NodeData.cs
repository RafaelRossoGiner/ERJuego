using UnityEngine;
using System.Collections.Generic;

//Since we want to serialize this class in a json, we wont use inheritance or polymorphism
[System.Serializable]
public class NodeData {
	public static int nodeCount = 0;
	public enum NodeType { Entity, Relation, Attribute, Generalization}

	public int id;
	public string nodeName;
	public NodeType type;
	public Vector2 position;
	public bool doubleLine;
	public bool isKey;

	// This referenciation lists are not serialized but instead populated in the first frame of opening an ER.
	// This implies that the lists are empty until the diagram has been visualized at least once.
	// This is done through the DrawDiagram function on the Diagram Controller.
	[System.NonSerialized()] 
	private List<NodeData> linkedNodes;
	[System.NonSerialized()]
	private List<LinkData> links;

	public NodeData(NodeType newType, Vector2 newPosition, int newId){
		id = newId;
		type = newType;
		position = newPosition;
		doubleLine = false;
		isKey = false;
		Rename(SerializationManager.config.unnamedNode);
		linkedNodes = new List<NodeData>();
		links = new List<LinkData>();
		nodeName = SerializationManager.config.unnamedNode;
	}
	public void Rename(string newName)
	{
		nodeName = newName;
	}
	public bool SwitchIsKey()
	{
		isKey = !isKey;
		return isKey;
	}
	//========================================= [ Node Link Manipulation ] ======================================================
	public void AddLinkedNode(NodeData node, LinkData link)
	{
		linkedNodes.Add(node);
		links.Add(link);
	}
	public void RemLinkedNode(NodeData node, LinkData link)
	{
		linkedNodes.Remove(node);
		links.Remove(link);
	}
	public List<NodeData> LinkedNodes()
	{
		return linkedNodes;
	}
	public List<LinkData> Links()
	{
		return links;
	}
}