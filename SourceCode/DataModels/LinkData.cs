using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

//Since we want to serialize this class on a json, we wont use inheritance or polymorphism
[System.Serializable]
public class LinkData {
	public enum LinkTypes { EntityRel, EntityRelReflexive, EntityAttr, RelationAtt, Generalization, Specialization, AttrAttr};
	public LinkTypes type;
	public string name;
	public string nodeState; //Used for Cardinality or Generalization Type
	public Vector2 auxRectPos;
	public bool participationIsTotal;
	public int[] linkedNodeId;

	public LinkData(){
		linkedNodeId = new int[2];
		participationIsTotal = false;
	}
	//Links which depend on only 2 nodes
	public LinkData(NodeData node1, NodeData node2, LinkTypes typeValue){
		linkedNodeId = new int[2] { node1.id, node2.id };
		switch (typeValue)
		{
			case LinkTypes.Generalization:
				//Since you cannot link from generalizations, only to them, the indicator node will always be the second one
				//also, the second one will be named
				name = node1.nodeName;
				break;
			case LinkTypes.Specialization:
				name = node1.nodeName;
				break;
			default:
				name = node1.id + "-" + node2.id;
				break;
		}
		type = typeValue;
	}
}