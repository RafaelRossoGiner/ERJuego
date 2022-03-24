using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

//Since we want to serialize this class on a json, we wont use inheritance or polymorphism
[System.Serializable]
public class LinkData {
	public enum LinkTypes { EntityRel, EntityRelReflexive, EntityAttr, RelationAtt, Generalization, Specialization, AttrAttr};
	public LinkTypes type;
	public string nameIDs;
	public string Name { get {return linkedNodes[0].nodeName + "-" + linkedNodes[1].nodeName; } }
	public string nodeState; //Used for Cardinality or Generalization Type
	public Vector2 auxRectPos;
	public bool participationIsTotal;
	public int[] linkedNodeId;
	[System.NonSerialized]
	public NodeData[] linkedNodes;

	public LinkData()
    {
		// Needed for serialization since the parametized constructor requires data that may not be ready.
		// This constructor is called intead for the serialization process to be able to instance links.
    }
	//Links which depend on only 2 nodes
	public LinkData(NodeData node1, NodeData node2, LinkTypes typeValue){
		switch (typeValue)
		{
			case LinkTypes.Generalization:
				//Since you cannot link from generalizations, only to them, the indicator node will always be the second one
				//also, the second one will be named
				nameIDs = node1.nodeName;
				break;
			case LinkTypes.Specialization:
				nameIDs = node1.nodeName;
				break;
			default:
				nameIDs = node1.id + "-" + node2.id;
				break;
		}
		type = typeValue;

		linkedNodeId = new int[2] { node1.id, node2.id };
		//Connect nodes in the ER structure with reference for fast access on checking
		linkedNodes = new NodeData[2] { node1, node2 };
	}
}