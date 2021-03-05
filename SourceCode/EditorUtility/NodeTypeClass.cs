using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Class needed to show the enum in the editor as a dynamic parameter. 
Enums aren't supported on the OnClick() delegate but objects are.*/
public class NodeTypeClass : MonoBehaviour
{
	public NodeData.NodeType nodeType;
}
