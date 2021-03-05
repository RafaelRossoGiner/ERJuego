using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModelDebug : MonoBehaviour
{
    GameObject playerRend;
    // Start is called before the first frame update
    void Start()
    {
        playerRend = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            playerRend.SetActive(!playerRend.activeSelf);
        }
    }
}
