using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeLookDebug : MonoBehaviour
{
    CinemachineFreeLook brain;
    // Start is called before the first frame update
    void Start()
    {
        brain = GetComponent<CinemachineFreeLook>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && brain.m_XAxis.m_MaxSpeed > 21)
        {
            brain.m_XAxis.m_MaxSpeed -= 20;
		}else if ((Input.GetKeyDown(KeyCode.E))){
            brain.m_XAxis.m_MaxSpeed += 20;
        }
    }
}
