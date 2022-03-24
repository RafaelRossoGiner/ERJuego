using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    static float sensibility = 350;
    static float camYspd;
    static CinemachineFreeLook brain;

    public void Start()
    {
        brain = GetComponent<CinemachineFreeLook>();
        SetSensibility(sensibility);
        DisableRotation(false);
    }
    public static float GetSensibility() { return sensibility; }
    public static void SetSensibility(float newValue)
	{
        sensibility = newValue;
        if (brain != null)
        {
            if (sensibility > 1)
                brain.m_XAxis.m_MaxSpeed = newValue;
            else
                brain.m_XAxis.m_MaxSpeed = 1;
        }
    }
    public static void DisableRotation(bool disable)
	{
        if(brain!= null)
		    if (disable)
		    {
                camYspd = brain.m_YAxis.m_MaxSpeed;

                brain.m_XAxis.m_MaxSpeed = 0;
                brain.m_YAxis.m_MaxSpeed = 0;
		    }
		    else
		    {
                brain.m_XAxis.m_MaxSpeed = sensibility;
                brain.m_YAxis.m_MaxSpeed = camYspd;
		    }
	}
}
