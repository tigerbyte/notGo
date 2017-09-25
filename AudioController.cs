using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioController : MonoBehaviour {

    GameController gameController;
    AssetData assets;

    public AudioSource audioPlayer;
    public AudioClip runnerDeath;

	void Awake() {
		DontDestroyOnLoad (transform.gameObject); // prevents destruction of this object when loading new scenes
        gameController = (GameController)FindObjectOfType(typeof(GameController));
	}

	void Start () {
        assets = gameController.assets; // get reference to assets which has references to all SFX
		audioPlayer = gameObject.GetComponents<AudioSource> () [0];
    }

    public void PlayCaptureSoundEffect() { audioPlayer.PlayOneShot(assets.captureSound); }
	public void PlaySwapSoundEffect()    { audioPlayer.PlayOneShot(assets.swapSound); } 
    public void PlayRunnerDeathSound()   { audioPlayer.PlayOneShot(assets.runnerDeathSound); }
    public void PlayVictorySound()       { audioPlayer.PlayOneShot(assets.victorySound); }
    public void PlayNewMatchSound()      { }

    // to do: find a way to loop running sound appropriately without using .Play()
    public void PlayRunningSound()
    {
        if (!audioPlayer.isPlaying)
        {
            audioPlayer.clip = assets.runningSound; audioPlayer.Play();
        }
    }
}
