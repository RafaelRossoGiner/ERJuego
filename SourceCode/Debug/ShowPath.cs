using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ShowPath : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Text>().text = "Se guarda en: " + Application.persistentDataPath + " Codigo: " + DiagramKeeper.GetCurrDiagramCode();
    }
}
