using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintingController : MonoBehaviour
{
    [SerializeField]
    private string id;
    List<Image> myPaintings;
    void Awake()
    {
        //Informar al PaintingGlobal de que existo y que me asigne una lista de cuadros:
        PaintingGlobal.Existo(id);
    }
    // Start is called before the first frame update
    void Start()
    {
        myPaintings = PaintingGlobal.DameCuadros(id);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
