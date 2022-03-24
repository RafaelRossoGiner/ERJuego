using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConfigData
{	
    public List<string> nodeNames;
	public string unnamedNode;
	public bool highlightNodeNames;
    public Dictionary<string, List<MessageData>> messages;
	public string noMessagesNotification;
	public Dictionary<string, List<string>> neededRooms;

	// ER Check configutation
	public int BasePoints;
	public string NoKeyEntError;
	public int NoKeyEntPoints;
	public string DanglingAttError;
	public int DanglingAttPoints;
	public string OverlinkedAttError;
	public int OverlinkedAttPoints;
	public string DanglingRelError;
	public int DanglingRelPoints;
	public string NoSpecError;
	public int NoSpecPoints;
	public string NoGenError;
	public int NoGenPoints;
	public string DoubleLineError;
	public int DoubleLinePoints;
	public string WeakEntityError;
	public int WeakEntityPoints;
	public int minimumNodes;
	public string insufficientNodesError;
	public string emptyDiagramError;

	// ER Similarity Check configuration
	public string correctEntity;
	public int correctEntityPoints;
	public string correctAttribute;
	public int correctAttributePoints;
	public string correctRelation;
	public int correctRelationPoints;
	public string correctWeakEntityConnection;
	public int correctWeakEntityConnectionPoints;
	public string correctEntRelLink;
	public int correctEntRelLinkPoints;
	public string correctDefaultLink;
	public int correctDefaultLinkPoints;
	public string correctCardinality;
	public int correctCardinalityPoints;
	public string correctParticipation;
	public int correctParticipationPoints;

	// Constructor
	public ConfigData(bool generateExamples = false)
	{
		// Create empty configuration object
		nodeNames = new List<string>();
		messages = new Dictionary<string, List<MessageData>>();
		neededRooms = new Dictionary<string, List<string>>();
		if (generateExamples)
		{
			// Add unnamed Node placeholder
			unnamedNode = "Sin Nombrar";
			highlightNodeNames = true;
			// Add examples to the configuration object
			nodeNames.AddRange(new string[] { "Ejemplo", "Prueba", "Testing" });

			string sender1 = "A1";
			List<MessageData> messagesSender1 = new List<MessageData>();
			messagesSender1.Add(new MessageData("Hey, mensaje de prueba"));
			messagesSender1.Add(new MessageData("Acuerdate de guardar cuando hayas terminado", new List<string>(new string[] { "terminado" })));
			messagesSender1.Add(new MessageData("Esto podría ser un requisito de un diagrama", new List<string>(new string[] { "requisito", "un", "un", "diagrama" })));
			messages.Add(sender1, messagesSender1);

			string sender2 = "A2";
			List<MessageData> messagesSender2 = new List<MessageData>();
			messagesSender2.Add(new MessageData("Esta habitación esta bloqueada por defecto"));
			messagesSender2.Add(new MessageData("Prueba importantisima", new List<string>(new string[] { "importantisima" })));
			messagesSender2.Add(new MessageData("Prueba poco importante", new List<string>(new string[] { "poco" })));
			messages.Add(sender2, messagesSender2);

			noMessagesNotification = "No hay ningún requisito indicado para este diagrama. Eres libre de diseñar lo que quieras.";

			// Add Room unlock template, half of the rooms are locked in a linear path
			string[] predefinedNames = Enum.GetNames(typeof(Interactable.PredefinedRoomCode));
			for (int i = 1; i < predefinedNames.Length; i++)
            {
				if (i < Mathf.Floor(predefinedNames.Length / 2f))
					neededRooms.Add(predefinedNames[i], new List<string>() { predefinedNames[i-1] });
				else
					neededRooms.Add(predefinedNames[i], new List<string>());
            }
			neededRooms[predefinedNames[2]].Add(predefinedNames[predefinedNames.Length-1]);

			// Add default message errors
			BasePoints = 100;
			NoKeyEntError = "Toda entidad debe tener al menos un atributo clave";
			NoKeyEntPoints = -5;
			DanglingAttError = "Todo atributo debe esta unido a una entidad o relación";
			DanglingAttPoints = -5;
			OverlinkedAttError = "Todo atributo puede estar unido como máximo a una sola entidad o relación";
			OverlinkedAttPoints = -5;
			DanglingRelError = "Toda relación une al menos a dos entidades o es reflexiva";
			DanglingRelPoints = -5;
			NoSpecError = "Toda generalización debe tener al menos una entidad que hereda (S)";
			NoSpecPoints = -5;
			NoGenError = "Toda generalización debe tener al menos una entidad de la que se hereda (G)";
			NoGenPoints = -5;
			DoubleLineError = "No se puede poner una participación total en un atributo de relación";
			DoubleLinePoints = -5;
			WeakEntityError = "Toda entidad débil debe estar unida a una relación débil";
			WeakEntityPoints = -5;
			minimumNodes = 2;
			insufficientNodesError = "Los diagramas deben tener un mínimo número de nodos para poder evaluarse";
			emptyDiagramError = "Este diagrama está vacío!";

			// Add default point values
			correctEntity = "Entidad correcta";
			correctEntityPoints = 5;
			correctAttribute = "Atributo correcto";
			correctAttributePoints = 1;
			correctRelation = "Relación correcta";
			correctRelationPoints = 10;
			correctDefaultLink = "Enlace correcto";
			correctDefaultLinkPoints = 10;
			correctEntRelLink = "Enlace correcto";
			correctEntRelLinkPoints = 20;
			correctCardinality = "Cardinalidad correcta";
			correctCardinalityPoints = 2;
			correctWeakEntityConnection = "Enlace correcto";
			correctWeakEntityConnectionPoints = 20;
			correctParticipation = "Participación correcta";
			correctParticipationPoints = 2;
		}
	}
}
