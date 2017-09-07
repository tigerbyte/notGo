using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceController : MonoBehaviour {
    
    //public Canvas canvas;
    public GameController gameController;
    public Image P1EnergyBar;
    public Image P2EnergyBar;

    void Start () {
        gameController = (GameController)FindObjectOfType(typeof(GameController));
	}

	void Update () {
        UpdateEnergyBars();
	}
    
    private void UpdateEnergyBars()
    {
        P1EnergyBar.fillAmount = gameController.p1.Energy / 10.0f;
        P2EnergyBar.fillAmount = gameController.p2.Energy / 10.0f;
    }
}
