using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;

public class PhoneMenuSolution : SimpleMenu<PhoneMenuSolution>
{
	[SerializeField]
	private DiagramEditor erDiagram;
	[SerializeField]
	Transform SolutionsList;
	[SerializeField]
	GameObject SolutionPrefab;

	private static ColorBlock lockedColorBlock = ColorBlock.defaultColorBlock;
	private static ColorBlock unlockedColorBlock = ColorBlock.defaultColorBlock;

	public void Start()
	{
		ShowSolutions();
	}
	public void Update()
	{
		if (!Input.anyKey)
			return;
		if (Input.GetKeyDown(KeyCode.F))
		{
			OnBackPressed();
		}
	}
	public override void OnBackPressed()
	{
		SceneHandler.Pause(false);
		Hide();
		OverlayMenu.Show();
		EventHandler.ClosePhone();
	}
	public void GoToMessages()
	{
		Hide();
		PhoneMenu.Show();
		EventHandler.CloseSolution();
	}

	public void ShowSolutions()
	{
		foreach (KeyValuePair<string, ERData> diagram in GameDiagramManager.solutions)
		{
			//Initialize each sender
			GameObject newDiagram = Instantiate(SolutionPrefab, SolutionsList);
			//Set text to senders name
			newDiagram.GetComponentInChildren<TextMeshProUGUI>().text = diagram.Key;
			if (GameDiagramManager.IsSolutionUnlocked(diagram.Key))
            {
				//Assign OnClick function to the button
				newDiagram.GetComponent<Button>().onClick.AddListener(delegate { ShowDiagram(diagram.Key); });
			}
            else
            {
				newDiagram.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f);
				newDiagram.GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
				newDiagram.GetComponent<Button>().enabled = false;

			}
		}
	}
	//Each sender button will call this method with the sender name
	void ShowDiagram(string diagramCode)
	{
		erDiagram.ClearDiagram();
		erDiagram.DrawDiagram(GameDiagramManager.GetSolution(diagramCode));
	}
}