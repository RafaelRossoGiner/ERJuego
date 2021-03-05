using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class PaintingGlobal
{
	static public Dictionary<string, List<Image>> cuadros;
    static PaintingGlobal()
	{
		cuadros = new Dictionary<string, List<Image>>();
	}
	public static void Existo(string controller)
	{
		//Asignarias en el diccionario, al controllador con id controller.id una lista NO repetida de cuadros
	}
	public static List<Image> DameCuadros(string id)
	{
		return new List<Image>();
	}
}
