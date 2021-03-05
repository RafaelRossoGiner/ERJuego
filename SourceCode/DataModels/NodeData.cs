using UnityEngine;

//Since we want to serialize this class on a json, we wont use inheritance or polymorphism
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

	public NodeData(NodeType newType, Vector2 newPosition, int newId){
		id = newId;
		type = newType;
		position = newPosition;
		doubleLine = false;
		isKey = false;
		Rename("Sin nombrar");
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
}