using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class DiagramLog
{
    public string DiagramName;
    public List<string> Entities, Relations, Attributes, WeakEntities, WeakRelations, MultAttributes, EntRel, EntAtt, RelAtt, AttrAttr;
    public Dictionary<string, List<string>> Generalizations;

    private Dictionary<int, string> IdToName;
    public DiagramLog(ERData data) {
        //This constructor transforms the more compact ERData class representation into a more
        //readable and useful serialization class for future data/process mining purposes
        DiagramName = data.diagramCode;

        Entities = new List<string>();
        Relations = new List<string>();
        Attributes = new List<string>();
        WeakEntities = new List<string>();
        WeakRelations = new List<string>();
        MultAttributes = new List<string>();
        EntRel = new List<string>();
        EntAtt = new List<string>();
        RelAtt = new List<string>();
        AttrAttr = new List<string>();

        Generalizations = new Dictionary<string, List<string>>();
        IdToName = new Dictionary<int, string>();

        foreach (NodeData node in data.nodes)
        {
            string name = "(" + node.id + ") " + node.nodeName;
            if (!node.doubleLine)
            {
                switch (node.type)
                {
                    case NodeData.NodeType.Entity:
                        Entities.Add(name);
                        IdToName.Add(node.id, node.nodeName);
                        break;
                    case NodeData.NodeType.Relation:
                        Relations.Add(name);
                        break;
                    case NodeData.NodeType.Attribute:
                        if (node.isKey) name += " - Atributo Clave";
                        Attributes.Add(name);
                        break;
                    case NodeData.NodeType.Generalization:
                        Generalizations.Add(name, new List<string>());
                        IdToName.Add(node.id, name);
                        break;
                }
            }
            else
            {
                switch (node.type)
                {
                    case NodeData.NodeType.Entity:
                        WeakEntities.Add(name);
                        break;
                    case NodeData.NodeType.Relation:
                        WeakRelations.Add(name);
                        break;
                    case NodeData.NodeType.Attribute:
                        if (node.isKey) name += " - Atributo Clave";
                        MultAttributes.Add(name);
                        break;
                    case NodeData.NodeType.Generalization:
                        Generalizations.Add(name, new List<string>());
                        IdToName.Add(node.id, name);
                        break;
                }
            }
        }
        foreach (LinkData link in data.links)
        {
            string participation = link.participationIsTotal ? "total" : "parcial";
            switch (link.type)
            {
                case LinkData.LinkTypes.EntityRel:
                case LinkData.LinkTypes.EntityRelReflexive:
                    EntRel.Add(link.name + " Participacion: " + participation + " Cardinalidad: " + link.nodeState);
                    break;
                case LinkData.LinkTypes.EntityAttr:
                    EntAtt.Add(link.name);
                    break;
                case LinkData.LinkTypes.RelationAtt:
                    RelAtt.Add(link.name);
                    break;
                case LinkData.LinkTypes.AttrAttr:
                    AttrAttr.Add(link.name);
                    break;
                case LinkData.LinkTypes.Generalization:
                    {
                        string Gen = IdToName[link.linkedNodeId[0]];
                        Debug.Log("Gen " + Gen);
                        List<string> SpecNodes = Generalizations[Gen];
                        Generalizations.Remove(Gen);
                        Gen += " - " + IdToName[link.linkedNodeId[1]];
                        IdToName[link.linkedNodeId[0]] = Gen;
                        Generalizations.Add(Gen, SpecNodes);
                    }
                    break;
                case LinkData.LinkTypes.Specialization:
                    {
                        string Gen = IdToName[link.linkedNodeId[0]];
                        Debug.Log("Spec "+Gen);
                        Generalizations[Gen].Add(IdToName[link.linkedNodeId[1]]);
                    }
                    break;
            }
        }
    }
}
