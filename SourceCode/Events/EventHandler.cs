using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class EventHandler
{
    public struct DataLog
    {
        public List<Tuple<string, ActionData>> actions;
        public List<DiagramLog> diagrams;
    };
    public static EventHandler instance; // singleton refering to an instance for storing data to be serialized
    public static string PlayerName { get; set; }
    public static string PlayerCaseId { get; set; }
    private DataLog dataLog;

	public EventHandler()
	{
        dataLog.actions = new List<Tuple<string, ActionData>>();
        dataLog.diagrams = new List<DiagramLog>();
    }

    public static void SetPlayer(string nameValue)
	{
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int cur_time = (int)(DateTime.UtcNow - epochStart).TotalSeconds;

        PlayerName = nameValue;
        PlayerCaseId = nameValue + "-" + cur_time.ToString();
    }

    //Generate Log file
    public static void GenerateLog()
    {
        instance.LogData();
        // Generate Action Log Object
        foreach (KeyValuePair<string, ERData> pair in GameDiagramManager.diagrams)
        {
            AddDiagramLog(pair.Value);
        }
        SerializationManager.SaveLog(instance.dataLog);
    }

    //Starting Log
    public void LogData()
	{
        ActionData newAction = new ActionData();
        string date_time = DateTime.UtcNow.ToString("dd/MM/yyyy H:mm:ss (zzz)");
        newAction.Add("CaseID", PlayerCaseId);
        newAction.Add("DateTime", date_time);
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("LogGeneralData", newAction));
	}
    //Final Log
    static public void UserClosedGame()
    {
        ActionData newAction = new ActionData();
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("User closed the Game", newAction));
        GenerateLog();
    }
    //Scenes
    static public void RoomMovement(string origin, string destination){
        ActionData newAction = new ActionData();
        newAction.Add("Origin", origin);
        newAction.Add("Destination", destination);
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("RoomMovement", newAction));
	}

    static public void OpenDiagram(string room, string diagramCode){
        ActionData newAction = new ActionData();
        newAction.Add("Room", room);
        newAction.Add("DiagramCode", diagramCode);
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("OpenDiagram", newAction));
    }
    //Diagram Data
    static public void AddDiagramLog(ERData diagram)
	{
        instance.dataLog.diagrams.Add(new DiagramLog(diagram));
	}
    //Menu and UI
    static public void OpenMenu()
    {
        ActionData newAction = new ActionData();
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("OpenMenu", newAction));
    }
    static public void OpenOptions()
    {
        ActionData newAction = new ActionData();
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("OpenOptions", newAction));
    }
    static public void CloseOptions()
    {
        ActionData newAction = new ActionData();
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("CloseOptions", newAction));
    }
    static public void CloseMenu()
    {
        ActionData newAction = new ActionData();
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("CloseMenu", newAction));
    }
    static public void OpenPhone()
    {
        ActionData newAction = new ActionData();
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("OpenPhone", newAction));
    }
    static public void ClosePhone()
    {
        ActionData newAction = new ActionData();
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("ClosePhone", newAction));
    }
    static public void OpenSolution()
    {
        ActionData newAction = new ActionData();
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("OpenSolution", newAction));
    }
    static public void CloseSolution()
    {
        ActionData newAction = new ActionData();
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("CloseSolution", newAction));
    }
    static public void SeeMessages(string sender)
    {
        ActionData newAction = new ActionData();
        newAction.Add("Sender", sender);
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("SeeMessages", newAction));
    }
    static public void ChangeMessageState(MessageData message, string oldState)
    {
        ActionData newAction = new ActionData();
        newAction.Add("Sender", message.sender);
        newAction.Add("Message", message.message);
        newAction.Add("oldState", oldState);
        newAction.Add("newState", message.state.ToString());
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("ChangeMessageState", newAction));
    }
    //Diagram Event
    static public void SaveDiagram(ERData node)
    {
        ActionData newAction = new ActionData();
        newAction.Add("DiagramCode", GameDiagramManager.GetCurrDiagramCode());
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("SaveDiagram", newAction));
    }
    static public void DeleteDiagram(ERData node)
    {
        ActionData newAction = new ActionData();
        newAction.Add("DiagramCode", GameDiagramManager.GetCurrDiagramCode());
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("DeleteDiagram", newAction));
    }
    static public void EvaluateDiagram(ERData node, int givenScore, int maxPosibleScore)
    {
        ActionData newAction = new ActionData();
        newAction.Add("DiagramCode", GameDiagramManager.GetCurrDiagramCode());
        newAction.Add("Given Score", givenScore.ToString());
        newAction.Add("Maximum Score", maxPosibleScore.ToString());
        newAction.Add("Total Points", SerializationManager.playerData.score.ToString());
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("EvaluateDiagram", newAction));
    }
    static public void AddNode(NodeData node){
        ActionData newAction = new ActionData();
        newAction.Add("DiagramCode", GameDiagramManager.GetCurrDiagramCode());
        newAction.Add("Name", node.nodeName);
        newAction.Add("Type", node.type.ToString());
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("AddNode", newAction));
    }
    static public void RemNode(NodeData node)
    {
        ActionData newAction = new ActionData();
        newAction.Add("DiagramCode", GameDiagramManager.GetCurrDiagramCode());
        newAction.Add("Name", node.nodeName);
        newAction.Add("Id", node.id.ToString());
        newAction.Add("Type", node.type.ToString());
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("RemoveNode", newAction));
    }
    static public void ChangeName(NodeData node, string oldName)
    {
        ActionData newAction = new ActionData();
        newAction.Add("DiagramCode", GameDiagramManager.GetCurrDiagramCode());
        newAction.Add("Id", node.id.ToString());
        newAction.Add("From", oldName);
        newAction.Add("To", node.nodeName);
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("ChangeName", newAction));
    }
    static public void ChangeGenType(NodeData node, string oldState)
    {
        ActionData newAction = new ActionData();
        newAction.Add("DiagramCode", GameDiagramManager.GetCurrDiagramCode());
        newAction.Add("Id", node.id.ToString());
        newAction.Add("From", oldState);
        newAction.Add("To", node.nodeName);
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("ChangeGenType", newAction));
    }
    static public void ChangeGenLinkType(LinkData link, string oldType)
    {
        ActionData newAction = new ActionData();
        newAction.Add("DiagramCode", GameDiagramManager.GetCurrDiagramCode());
        newAction.Add("Node1-Id", link.linkedNodeId[0].ToString());
        newAction.Add("Node2-Id", link.linkedNodeId[1].ToString());
        newAction.Add("From", oldType);
        newAction.Add("To", link.type.ToString());
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("ChangeGenLinkType", newAction));
    }
    static public void ChangeKeyAttribute(NodeData node, string oldState)
    {
        ActionData newAction = new ActionData();
        string keyValue = "";
        newAction.Add("DiagramCode", GameDiagramManager.GetCurrDiagramCode());
        newAction.Add("Name", node.nodeName);
        newAction.Add("Id", node.id.ToString());
        newAction.Add("Type", node.type.ToString());
        keyValue = node.isKey ? "Es" : "No es";
        newAction.Add("Estado", keyValue + " atributo clave");
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("ChangeKeyAttribute", newAction));
    }
    static public void AddLink(LinkData link){
        ActionData newAction = new ActionData();
        newAction.Add("DiagramCode", GameDiagramManager.GetCurrDiagramCode());
        newAction.Add("Node1-Id", link.linkedNodeId[0].ToString());
        newAction.Add("Node2-Id", link.linkedNodeId[1].ToString());
        newAction.Add("Type", link.type.ToString());
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("AddLink", newAction));
    }
    static public void RemLink(LinkData link)
    {
        ActionData newAction = new ActionData();
        newAction.Add("DiagramCode", GameDiagramManager.GetCurrDiagramCode());
        newAction.Add("Node1-Id", link.linkedNodeId[0].ToString());
        newAction.Add("Node2-Id", link.linkedNodeId[1].ToString());
        newAction.Add("Type", link.type.ToString());
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("RemoveLink", newAction));
    }
    static public void ChangeCardinality(LinkData link)
    {
        ActionData newAction = new ActionData();
        newAction.Add("DiagramCode", GameDiagramManager.GetCurrDiagramCode());
        newAction.Add("Name", link.nameIDs);
        newAction.Add("New Cardinality", link.nodeState);
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("ChangeCardinality", newAction));
    }
    static public void ChangeParticipation(LinkData link)
    {
        ActionData newAction = new ActionData();
        newAction.Add("DiagramCode", GameDiagramManager.GetCurrDiagramCode());
        newAction.Add("Name", link.nameIDs);
        string participation = link.participationIsTotal ? "Total" : "Parcial";
        newAction.Add("New Participation", link.nodeState);
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("ChangeParticipation", newAction));
    }
}
