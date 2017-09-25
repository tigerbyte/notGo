using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceController : MonoBehaviour
{   
    public Canvas canvas;
    public GameController gameController;
    AssetData assets;

    public Image P2EnergyBar;
    public Image P2EnergyBarFill;
    public Mask P2EnergyBarMask;
    public Image P1EnergyBarFill;
    public Text VictoryText;

    delegate void UpdateUserInterface();
    UpdateUserInterface updateInterface;

    void Start () {
        canvas = (Canvas)FindObjectOfType(typeof(Canvas));
        gameController = (GameController)FindObjectOfType(typeof(GameController));
        assets = gameController.assets;

        // determine which method we should use on update based on our game type (Local vs Networked)
        if (ApplicationData.GameMode == ApplicationData.GameModes.Local)
        {
            updateInterface = new UpdateUserInterface(UpdateInterfaceLocal);
        }

        else if (ApplicationData.GameMode == ApplicationData.GameModes.Networked)
        {
            P2EnergyBarMask.showMaskGraphic = false;
            P2EnergyBarFill.enabled = false;
            P2EnergyBar.enabled = false;
            updateInterface = new UpdateUserInterface(UpdateInterfaceNetworked);
        }
	}

    private void UpdateInterfaceLocal()
    {
        P1EnergyBarFill.fillAmount = gameController.p1.Energy / 10.0f;
    }

    private void UpdateInterfaceNetworked()
    {
        P1EnergyBarFill.fillAmount = gameController.p1.Energy / 10.0f;
        P2EnergyBarFill.fillAmount = gameController.p2.Energy / 10.0f;
    }

    void Update()
    {
        updateInterface();
    }

    public void ShowVictoryText(Player player)
    {
        VictoryText.GetComponent<Text>().enabled = true;
        VictoryText.text = "Player " + player.playerNumber + " is victorious.";
    }
}
