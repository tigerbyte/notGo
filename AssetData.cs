using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetData : ScriptableObject
{
    public Image P1EnergyBar, P2EnergyBar;
    public Texture crackedTexture, noTexture;
    public Material voidMat, voidRedSelected, voidBlueSelected, redOwnedBlueSelected,
                    redOwnedRedSelected, blueOwnedRedSelected, blueOwnedBlueSelected, blueMat, redMat,
                    blueOnBlueDamaged, redOnBlueDamaged, blueOnRedDamaged, redOnRedDamaged,
                    blueDamaged, redDamaged;
    public AudioClip runnerDeathSound, captureSound, runningSound, swapSound, victorySound, shatterSound;
    

    private void Awake()
    {
        LoadTextures();
        LoadMaterials();
        LoadSFX();
        LoadInterfaceComponents();
    }

    void LoadTextures()
    {
        crackedTexture = (Texture)Resources.Load("Textures/cracked_texture");
        noTexture = (Texture)Resources.Load("Textures/nothing");
    }

    void LoadMaterials()
    {
        voidMat = (Material)Resources.Load("Materials/VoidTileMat");
        blueMat = (Material)Resources.Load("Materials/BlueTileMat");
        redMat = (Material)Resources.Load("Materials/RedTileMat");
        voidRedSelected = (Material)Resources.Load("Materials/VoidRedSelected");
        voidBlueSelected = (Material)Resources.Load("Materials/VoidBlueSelected");
        redOwnedBlueSelected = (Material)Resources.Load("Materials/RedOwnedBlueSelected");
        redOwnedRedSelected = (Material)Resources.Load("Materials/RedOwnedRedSelected");
        blueOwnedBlueSelected = (Material)Resources.Load("Materials/BlueOwnedBlueSelected");
        blueOwnedRedSelected = (Material)Resources.Load("Materials/BlueOwnedRedSelected");
        blueDamaged = (Material)Resources.Load("Materials/BlueDamaged");
        redDamaged = (Material)Resources.Load("Materials/RedDamaged");
        blueOnBlueDamaged = (Material)Resources.Load("Materials/BlueOnBlueDamaged");
        redOnBlueDamaged = (Material)Resources.Load("Materials/RedOnBlueDamaged");
        blueOnRedDamaged = (Material)Resources.Load("Materials/BlueOnRedDamaged");
        redOnRedDamaged = (Material)Resources.Load("Materials/RedOnRedDamaged");
    }

    void LoadSFX()
    {
        runnerDeathSound = (AudioClip)Resources.Load("SoundFX/runnerdeath_sfx");
        captureSound = (AudioClip)Resources.Load("SoundFX/capture_sfx");
        runningSound = (AudioClip)Resources.Load("SoundFX/Running");
        swapSound = (AudioClip)Resources.Load("SoundFX/swap_sfx");
        victorySound = (AudioClip)Resources.Load("SoundFX/victory_sfx");
        shatterSound = (AudioClip)Resources.Load("SoundFX/crack_sfx");
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
