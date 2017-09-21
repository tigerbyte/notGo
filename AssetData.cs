﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetData : ScriptableObject
{
    public Image P1EnergyBar, P2EnergyBar;
    public Material voidMat, voidRedSelected, voidBlueSelected, redOwnedBlueSelected,
                    redOwnedRedSelected, blueOwnedRedSelected, blueOwnedBlueSelected, blueMat, redMat;
    

    private void Awake()
    {
        LoadMaterials();
        LoadInterfaceComponents();
    }

    void LoadMaterials()
    {
        voidMat = (Material)Resources.Load("Materials/VoidTileMat");
        voidRedSelected = (Material)Resources.Load("Materials/VoidRedSelected");
        voidBlueSelected = (Material)Resources.Load("Materials/VoidBlueSelected");
        redOwnedBlueSelected = (Material)Resources.Load("Materials/RedOwnedBlueSelected");
        redOwnedRedSelected = (Material)Resources.Load("Materials/RedOwnedRedSelected");
        blueOwnedBlueSelected = (Material)Resources.Load("Materials/BlueOwnedBlueSelected");
        blueOwnedRedSelected = (Material)Resources.Load("Materials/BlueOwnedRedSelected");
        blueMat = (Material)Resources.Load("Materials/BlueTileMat");
        redMat = (Material)Resources.Load("Materials/RedTileMat");
    }

    void LoadInterfaceComponents()
    {
        /*
        // Load P1's bar no matter what, but only load P2's bar if we're playing locally
        P1EnergyBar = Resources.Load<Image>("Sprites/P1_EnergyBar_Sprite");
        Debug.Log("loaded P1's bar");

        if (ApplicationData.GameMode == ApplicationData.GameModes.Local) {
            Debug.Log("loaded P2's bar");
            P2EnergyBar = Resources.Load<Image>("Sprites/P1_EnergyBar_Sprite");
        }
        */
    }
}