using System.IO;
using System.Collections.Generic;
using UnityEngine;

static public class GameDiagramManager
{
	public static Dictionary<string, ERData> diagrams;
	public static Dictionary<string, ERData> solutions;

	private static Dictionary<string, ERData> predefinedDiagrams;
	private static Dictionary<string, bool> solutionUnlocked;
	private static Dictionary<string, List<string>> neededRooms;
	private static Dictionary<string, Texture2D> diagramGraphicStates;
	private static string currentCode;
	// Constructor
	static GameDiagramManager(){
		diagrams = new Dictionary<string, ERData>();
		solutions = new Dictionary<string, ERData>();

		predefinedDiagrams = new Dictionary<string, ERData>();
		solutionUnlocked = new Dictionary<string, bool>();
		neededRooms = new Dictionary<string, List<string>>();
		diagramGraphicStates = new Dictionary<string, Texture2D>();
	}
	// Methods
	public static void SetDiagramCode(string diagramCode){
		currentCode = diagramCode;
	}

	public static ERData GetCurrDiagram() {
		if (!diagrams.ContainsKey(currentCode)){
			if (predefinedDiagrams.ContainsKey(currentCode))
            {
				diagrams[currentCode] = new ERData(predefinedDiagrams[currentCode]);
			}
            else
            {
				diagrams[currentCode] = new ERData(currentCode);
			}
		}
		return diagrams[currentCode];
	}
	public static void ResetCurrentDiagram()
	{
		ERData erData = diagrams[currentCode];
		SerializationManager.DeleteER(erData);
		diagrams[currentCode].ClearER();
	}
	public static string GetCurrDiagramCode(){
		return currentCode;
	}
	public static List<string> RoomCompleted(string roomCode)
	{
		List<string> unlocked = new List<string>();
		foreach (KeyValuePair<string, List<string>> roomList in neededRooms)
        {
			if (roomList.Value.Remove(roomCode) && roomList.Value.Count == 0)
            {
				unlocked.Add(roomList.Key);
			}
        }
		if (solutionUnlocked.ContainsKey(roomCode))
        {
			solutionUnlocked[roomCode] = true;
		}
		return unlocked;
	}
	public static bool IsSolutionUnlocked (string roomCode)
	{
		bool isUnlocked;
		if (solutionUnlocked.ContainsKey(roomCode))
			isUnlocked = solutionUnlocked[roomCode];
		else
			isUnlocked = false;
		return isUnlocked;
	}
	public static bool IsUnlocked(string roomCode)
    {
		bool isUnlocked;
		if (neededRooms.ContainsKey(roomCode))
			isUnlocked = (neededRooms[roomCode].Count == 0);
		else
			isUnlocked = true;
		return isUnlocked;
	}
	public static List<string> GetRequiredRoomsToUnlock(string roomCode)
	{
		return neededRooms[roomCode];
	}
	public static void SetRoomLocks(Dictionary<string, List<string>> roomPrerequisites)
	{
		neededRooms = roomPrerequisites;
	}
	public static void LoadPredefinedDiagrams(string predefDirPath)
    {
		DirectoryInfo savesDir = new DirectoryInfo(predefDirPath);
		foreach (FileInfo saveFile in savesDir.GetFiles())
		{
			ERData erData = SerializationManager.LoadER(saveFile.FullName);
			if (erData != null)
			{
				erData.LoadReferences();
				predefinedDiagrams[erData.diagramCode] = erData;
			}
		}
	}
	public static void LoadSavedDiagrams(string savesDirPath)
	{
		DirectoryInfo savesDir = new DirectoryInfo(savesDirPath);
		foreach (FileInfo saveFile in savesDir.GetFiles())
		{
			ERData erData = SerializationManager.LoadER(saveFile.FullName);
			if (erData != null)
			{
				erData.LoadReferences();
				diagrams[erData.diagramCode] = erData;
			}
		}
	}
	public static void LoadAllSolutions(string solutionDirPath)
	{
		DirectoryInfo solutionsDir = new DirectoryInfo(solutionDirPath);
		foreach (FileInfo solutionFile in solutionsDir.GetFiles())
		{
			ERData erData = SerializationManager.LoadER(solutionFile.FullName);
			if (erData != null)
            {
				erData.LoadReferences();
				solutions.Add(erData.diagramCode, erData);
				solutionUnlocked.Add(erData.diagramCode, false);
			}
		}
	}

	public static ERData GetSolution(string diagramCode)
	{
		return solutions[diagramCode];
	}
	public static void AddTexture(Texture2D diagramState, string diagramCode)
    {
		diagramGraphicStates[diagramCode] = diagramState;
	}
	public static Texture2D GetTexture(string diagramCode)
	{
		if (diagrams[diagramCode].nodes.Count > 0) // && diagramStates.ContainsKey(diagramCode)
		{
			return diagramGraphicStates[diagramCode];
        }
        else
        {
			return null;
        }
	}
	public static bool IsPopulated(string diagramCode)
	{
		if (diagrams.ContainsKey(diagramCode) && diagrams[diagramCode].nodes.Count > 0) // && diagramStates.ContainsKey(diagramCode)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
