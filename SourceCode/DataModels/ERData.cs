using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

[System.Serializable]
public class ERData
{
    //========================================= [ Diagram Attributes ] ======================================================
    public int lastId; //The highest integer id used in this Diagram
    public string diagramCode;
    public List<NodeData> nodes;
    public List<LinkData> links;

    //========================================= [ Check Attributes ] ======================================================
    private static int basePoints = SerializationManager.config.BasePoints;
    private static string noKeyEntError = SerializationManager.config.NoKeyEntError;
    private static int noKeyEntPoints = SerializationManager.config.NoKeyEntPoints;
    private static string danglingAttError = SerializationManager.config.DanglingAttError;
    private static int danglingAttPoints = SerializationManager.config.DanglingAttPoints;
    private static string overlinkedAttError = SerializationManager.config.OverlinkedAttError;
    private static int overlinkedAttPoints = SerializationManager.config.OverlinkedAttPoints;
    private static string danglingRelError = SerializationManager.config.DanglingRelError;
    private static int danglingRelPoints = SerializationManager.config.DanglingRelPoints;
    private static string noSpecError = SerializationManager.config.NoSpecError;
    private static int noSpecPoints = SerializationManager.config.NoSpecPoints;
    private static string noGenError = SerializationManager.config.NoGenError;
    private static int noGenPoints = SerializationManager.config.NoGenPoints;
    private static string doubleLineError = SerializationManager.config.DoubleLineError;
    private static int doubleLinePoints = SerializationManager.config.DoubleLinePoints;
    private static int minNodes = SerializationManager.config.minimumNodes;
    private static string weakEntityError = SerializationManager.config.WeakEntityError;
    private static int weakEntityPoints = SerializationManager.config.WeakEntityPoints;
    private static string insufficientNodesError = SerializationManager.config.insufficientNodesError;
    private static string emptyDiagramError = SerializationManager.config.emptyDiagramError;
    //========================================= [ Similarity Check Attributes ] ======================================================
    private static string correctEntity = SerializationManager.config.correctEntity;
    private static int correctEntityPoints = SerializationManager.config.correctEntityPoints;
    private static string correctAttribute = SerializationManager.config.correctAttribute;
    private static int correctAttributePoints = SerializationManager.config.correctAttributePoints;
    private static string correctRelation = SerializationManager.config.correctRelation;
    private static int correctRelationPoints = SerializationManager.config.correctRelationPoints;
    private static string correctDefaultLink = SerializationManager.config.correctDefaultLink;
    private static int correctDefaultLinkPoints = SerializationManager.config.correctDefaultLinkPoints;
    private static string correctEntRelLink = SerializationManager.config.correctEntRelLink;
    private static int correctEntRelLinkPoints = SerializationManager.config.correctEntRelLinkPoints;
    private static string correctCardinality = SerializationManager.config.correctCardinality;
    private static int correctCardinalityPoints = SerializationManager.config.correctCardinalityPoints;
    private static string correctParticipation = SerializationManager.config.correctParticipation;
    private static int correctParticipationPoints = SerializationManager.config.correctParticipationPoints;
    private static string correctWeakEntityConnection = SerializationManager.config.correctWeakEntityConnection;
    private static int correctWeakEntityConnectionPoints = SerializationManager.config.correctWeakEntityConnectionPoints;
    // Constructors
    public ERData()
    {
        nodes = new List<NodeData>();
        links = new List<LinkData>();
        lastId = 0;
    }
    public ERData(string code) : this()
    {
        diagramCode = code;
    }
    public ERData(ERData reference)
    {
        nodes = reference.nodes;
        links = reference.links;
        lastId = reference.lastId;
        diagramCode = reference.diagramCode;
    }
    // Add references between nodes and links inside their respective data instances
    public void LoadReferences()
    {
        NodeData node1;
        NodeData node2;
        foreach (LinkData link in links)
        {
            node1 = FindNodeByID(link.linkedNodeId[0]);
            node2 = FindNodeByID(link.linkedNodeId[1]);
            link.linkedNodes = new NodeData[2] { node1, node2};
            node1.AddLinkedNode(node2, link);
            node2.AddLinkedNode(node1, link);
        }
    }
    // Empty ER information
    public void ClearER()
    {
        nodes.Clear();
        links.Clear();
        lastId = 0;
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
        link.linkedNodes[0].AddLinkedNode(link.linkedNodes[1], link);
        link.linkedNodes[1].AddLinkedNode(link.linkedNodes[0], link);
    }

    public void RemoveLink(LinkData link)
    {
        link.linkedNodes[0].RemLinkedNode(link.linkedNodes[1], link);
        link.linkedNodes[1].RemLinkedNode(link.linkedNodes[0], link);
        links.Remove(link);
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
    //========================================= [ Diagram Utility functions ] ======================================================
    public NodeData FindNodeByID(int id)
    {
        foreach (NodeData node in nodes)
        {
            if (node.id == id)
                return node;
        }
        return null;
    }
    public List<string> GetUsedNames()
    {
        List<string> names = new List<string>();
        foreach(NodeData node in nodes) {
            names.Add(node.nodeName);
        }
        return names;
    }
    //========================================= [ ER Correctness Check ] ======================================================
    public bool CheckErrors(out List<(int, string)> errorMessages, ref int score, ref int maxScore)
    {
        errorMessages = new List<(int, string)>();
        score += basePoints;
        maxScore += basePoints;
        if (nodes.Count == 0)
        {
            errorMessages.Add((-basePoints, emptyDiagramError));
            return false;
        }
        else if (nodes.Count < minNodes)
        {
            errorMessages.Add((-basePoints, insufficientNodesError + "(" + minNodes + ")"));
            return false;
        }
        else
        {
            foreach (NodeData node in nodes)
            {
                switch (node.type)
                {
                    case NodeData.NodeType.Entity:
                        if (node.doubleLine)
                        {
                            // It is a weak entity
                            bool hasWeakRelation = false;
                            foreach (NodeData linkedNode in node.LinkedNodes())
                            {
                                if (linkedNode.type == NodeData.NodeType.Relation && linkedNode.doubleLine)
                                {
                                    hasWeakRelation = true;
                                    break;
                                }
                            }
                            if (!hasWeakRelation)
                            {
                                errorMessages.Add((weakEntityPoints, "Entidad " + node.nodeName + ": " + weakEntityError));
                                score += weakEntityPoints;
                            }
                        }
                        else
                        {
                            // Is not a weak entity
                            bool hasKeyAtribute = false;
                            foreach (NodeData linkedNode in node.LinkedNodes())
                            {
                                if (linkedNode.type == NodeData.NodeType.Attribute && linkedNode.isKey)
                                {
                                    hasKeyAtribute = true;
                                    break;
                                }
                            }
                            if (!hasKeyAtribute)
                            {
                                errorMessages.Add((noKeyEntPoints, "Entidad " + node.nodeName + ": " + noKeyEntError));
                                score += noKeyEntPoints;
                            }
                        }
                        break;
                    case NodeData.NodeType.Relation:
                        if (node.LinkedNodes().Count > 0)
                        {
                            int entityLinks = 0; // Amount of linked nodes which are Entities
                            foreach (NodeData linkedNode in node.LinkedNodes())
                            {
                                if (entityLinks > 2)
                                {
                                    break;
                                }
                                else
                                {
                                    if (linkedNode.type == NodeData.NodeType.Entity)
                                        entityLinks++;
                                }
                            }
                            if (entityLinks < 2)
                            {
                                errorMessages.Add((danglingRelPoints, "Relación " + node.nodeName + ": " + danglingRelError));
                                score += danglingRelPoints;
                            }
                        }
                        else
                        {
                            errorMessages.Add((danglingRelPoints, "Relación " + node.nodeName + ": " + danglingRelError));
                            score += danglingRelPoints;
                        }
                        break;
                    case NodeData.NodeType.Attribute:
                        if (node.LinkedNodes().Count > 0)
                        {
                            int wantedLinks = 0; // Amount of linked nodes which are Entities or Relations
                            foreach (NodeData linkedNode in node.LinkedNodes())
                            {
                                if (wantedLinks > 1)
                                {
                                    break;
                                }
                                else
                                {
                                    if (linkedNode.type == NodeData.NodeType.Entity || linkedNode.type == NodeData.NodeType.Relation)
                                        wantedLinks++;
                                }
                            }
                            if (wantedLinks == 0)
                            {
                                errorMessages.Add((danglingAttPoints, "Atributo " + node.nodeName + ": " + danglingAttError));
                                score += danglingAttPoints;
                            }
                            else if (wantedLinks > 1)
                            {
                                errorMessages.Add((overlinkedAttPoints, "Atributo " + node.nodeName + ": " + overlinkedAttError));
                                score += overlinkedAttPoints;
                            }
                        }
                        else
                        {
                            errorMessages.Add((danglingAttPoints, "Atributo " + node.nodeName + ": " + danglingAttError));
                            score += danglingAttPoints;
                        }
                        break;
                    case NodeData.NodeType.Generalization:
                        bool SuperClassExist = false;
                        bool SubClassExist = false;
                        foreach (LinkData linkedNode in node.Links())
                        {
                            if (!SuperClassExist && linkedNode.type == LinkData.LinkTypes.Generalization)
                            {
                                SuperClassExist = true;
                            }
                            if (!SubClassExist && linkedNode.type == LinkData.LinkTypes.Specialization)
                            {
                                SubClassExist = true;
                            }
                        }
                        if (!SuperClassExist)
                        {
                            errorMessages.Add((noGenPoints, "Generalización: " + node.nodeName + ": " + noGenError));
                            score += noGenPoints;
                        }
                        if (!SubClassExist)
                        {
                            errorMessages.Add((noSpecPoints, "Generalización: " + node.nodeName + ": " + noSpecError));
                            score += noSpecPoints;
                        }
                        break;
                }
            }
            foreach (LinkData link in links)
            {
                if (link.type == LinkData.LinkTypes.RelationAtt && link.participationIsTotal)
                {
                    errorMessages.Add((doubleLinePoints, "Enlace " + link.Name + ": " + doubleLineError));
                    score += doubleLinePoints;
                }
            }
        }
        // Diagram can be qualified
        return true;
    }
    //========================================= [ ER Similatiry Check ] ===================================================
    public bool CheckSimilarity(ERData refER, out List<(int, string)> correctElements, ref int score, ref int maxScore) {
        correctElements = new List<(int, string)>();
        Dictionary<string, NodeData> refNodes = new Dictionary<string, NodeData>();
        Dictionary<string, LinkData> refLinks = new Dictionary<string, LinkData>();
        NodeData refNode;
        LinkData refLink;

        // Node points
        foreach (NodeData node in refER.nodes)
        {
            if (refNodes.ContainsKey(node.nodeName))
            {
                Debug.LogWarning("Repeated nodes on the solution to diagram " + refER.diagramCode);
                return false;
            }
            else
            {
                refNodes[node.nodeName] = node;
            }
            switch (node.type)
            {
                case NodeData.NodeType.Entity:
                    maxScore += correctEntityPoints;
                    break;
                case NodeData.NodeType.Attribute:
                    maxScore += correctAttributePoints;
                    break;
                case NodeData.NodeType.Relation:
                    maxScore += correctRelationPoints;
                    break;
                default:
                    break;
            }
        }
        // Check matching nodes
        foreach (NodeData m_node in nodes)
        {
            if(refNodes.TryGetValue(m_node.nodeName, out refNode) && m_node.type == refNode.type && m_node.doubleLine == refNode.doubleLine)
            {
                switch (refNode.type)
                {
                    case NodeData.NodeType.Entity:
                        score += correctEntityPoints;
                        correctElements.Add((correctEntityPoints, m_node.nodeName + ": " + correctEntity));
                        break;
                    case NodeData.NodeType.Attribute:
                        if (m_node.isKey == refNode.isKey) {
                            score += correctAttributePoints;
                            correctElements.Add((correctAttributePoints, m_node.nodeName + ": " + correctAttribute));
                        }
                        break;
                    case NodeData.NodeType.Relation:
                        score += correctRelationPoints;
                        correctElements.Add((correctRelationPoints, m_node.nodeName + ": " + correctRelation));
                        break;
                    default:
                        break;
                }
            }
        }
        // Node links
        foreach (LinkData link in refER.links)
        {
            refLinks[link.Name] = link;
            switch (link.type)
            {
                case LinkData.LinkTypes.AttrAttr:
                case LinkData.LinkTypes.EntityAttr:
                case LinkData.LinkTypes.RelationAtt:
                    maxScore += correctDefaultLinkPoints;
                    maxScore += correctParticipationPoints;
                    break;
                case LinkData.LinkTypes.EntityRel:
                case LinkData.LinkTypes.EntityRelReflexive:
                    maxScore += correctEntRelLinkPoints;
                    maxScore += correctCardinalityPoints;
                    break;
                default:
                    break;
            }
        }
        // Check matching links
        foreach (LinkData m_link in links)
        {
            string name = m_link.linkedNodes[0].nodeName + "-" + m_link.linkedNodes[1].nodeName;
            string invName = m_link.linkedNodes[1].nodeName + "-" + m_link.linkedNodes[0].nodeName;
            // If the first search is succesful, the || operator enters in shortcut so the value of the refLink is not overriden by the second search
            if (refLinks.TryGetValue(name, out refLink) || refLinks.TryGetValue(invName, out refLink))
            {
                switch (refLink.type)
                {
                    case LinkData.LinkTypes.AttrAttr:
                    case LinkData.LinkTypes.EntityAttr:
                    case LinkData.LinkTypes.RelationAtt:
                        score += correctDefaultLinkPoints;
                        correctElements.Add((correctDefaultLinkPoints, m_link.Name + ": " + correctDefaultLink));
                        if (m_link.participationIsTotal == refLink.participationIsTotal)
                        {
                            score += correctCardinalityPoints;
                            correctElements.Add((correctParticipationPoints, m_link.Name + ": " + correctParticipation));
                        }
                        break;
                    case LinkData.LinkTypes.EntityRel:
                    case LinkData.LinkTypes.EntityRelReflexive:
                        score += correctEntRelLinkPoints;
                        correctElements.Add((correctEntRelLinkPoints, m_link.Name + ": " + correctEntRelLink));
                        if (m_link.nodeState == refLink.nodeState)
                        {
                            score += correctCardinalityPoints;
                            correctElements.Add((correctCardinalityPoints, m_link.Name + ": " + correctCardinality));
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        return true;
    }
}
