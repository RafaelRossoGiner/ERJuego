using System.IO;
using System.Collections.Generic;
using UnityEngine;

static public class DiagramKeeper
{
	public static Dictionary<string, ERData> diagrams;
	private static string currentCode;

	static DiagramKeeper(){
		diagrams = new Dictionary<string, ERData>();
	}

	public static void SetDiagramCode(string diagramCode){
		if (!diagrams.ContainsKey(diagramCode)){
			string dictionaryPath = Path.Combine(Path.Combine(Application.persistentDataPath, "Saves"), "Diagram-" + diagramCode + ".json");
			FileInfo file = new FileInfo(dictionaryPath);
			if (file.Exists){
				ERData erData = new ERData();
				Debug.Log(file.Name);
				erData.Load(file.Name);
				diagrams.Add(diagramCode, erData);
			}
			else
			{
				ERData erData = new ERData();
				erData.diagramCode = diagramCode;
				diagrams.Add(diagramCode, erData);
			}
		}
		currentCode = diagramCode;
	}

	public static ERData GetCurrDiagram() {
		return diagrams[currentCode];
	}

	public static string GetCurrDiagramCode(){
		return currentCode;
	}
}
