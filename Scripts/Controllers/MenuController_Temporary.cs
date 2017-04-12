using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Make a Permanent Scene Manager class

public class MenuController_Temporary : MonoBehaviour {

	GameObject mainPanel, battlePanel, deckPanel, tutorialPanel, settingsPanel;
	public List<GameObject> panelList = new List<GameObject> ();

	public AudioController audioCtrl;

	void Start () {
		panelList.Add (mainPanel = (GameObject)GameObject.Find ("MainMenuPanel"));
		panelList.Add (battlePanel = (GameObject)GameObject.Find ("BattleMenuPanel"));
		panelList.Add (deckPanel = (GameObject)GameObject.Find ("DeckMenuPanel"));
		panelList.Add (tutorialPanel = (GameObject)GameObject.Find ("TutorialPanel"));
		panelList.Add (settingsPanel = (GameObject)GameObject.Find ("SettingsPanel"));

		SetActivePanel (mainPanel);

		audioCtrl = (AudioController) GameObject.FindObjectOfType(typeof(AudioController));
	}

	// TODO: make the panel switching more elegant 
	// (combine into one function and pass in the desired active menu perhaps)
	public void SetActivePanel (GameObject activePanel) {
		foreach (GameObject panel in panelList) {
			if (panel != activePanel) { panel.SetActive (false); }
		}
		activePanel.SetActive (true);

		audioCtrl.PlayCaptureSoundEffect ();
	}

	public void ReturnToMainMenu () {
		foreach (GameObject panel in panelList) {
			if (panel != mainPanel) { panel.SetActive (false); }
		}
		mainPanel.SetActive (true);
	}

	public void StartMatch () {
		SceneManager.LoadScene ("Scenes/GameScene", LoadSceneMode.Single);
		audioCtrl.PlayNewMatchSound ();
	}
}
