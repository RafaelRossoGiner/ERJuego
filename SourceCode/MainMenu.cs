using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    string nameInputRegex;
    [SerializeField]
    TMP_InputField nameInput;
    [SerializeField]
    TextMeshProUGUI errorDisplay;
    public void PlayGame()
    {
        if (nameInput.text != "")
        {
            PlayerEventHandler.SetPlayer(nameInput.text);
            SceneHandler.NextRoom("Pasillo");
		}
		else
		{
            errorDisplay.text = "Debes introducir tu identificador antes de comenzar el juego";
		}
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
