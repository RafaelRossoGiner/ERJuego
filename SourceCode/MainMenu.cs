using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static void PlayGame()
    {
        SceneHandler.NextRoom("Pasillo");
    }

    public static void QuitGame()
    {
        Application.Quit();
        
    }
}
