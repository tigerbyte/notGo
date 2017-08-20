using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Make a Scene Manager class

public class AudioController : MonoBehaviour {

	public AudioSource capture;
	public AudioSource swap;
	public AudioSource newmatch;
    public AudioSource running;

	void Awake() {
		DontDestroyOnLoad (transform.gameObject); // prevents destruction when loading new scenes
	}

	void Start () {
		// one audiosource for each sound effect
		capture = gameObject.GetComponents<AudioSource> () [0];
		swap = gameObject.GetComponents<AudioSource> () [1];
        running = gameObject.GetComponents<AudioSource>() [2];
        // newmatch = gameObject.GetComponents<AudioSource> () [2];
    }

	public void PlayCaptureSoundEffect() { capture.Play ();  }
	public void PlaySwapSoundEffect()    { swap.Play ();     }  
	public void PlayNewMatchSound()      { newmatch.Play (); }  
    public void PlayRunningSound()       {
        if (!running.isPlaying)
        running.Play ();
    }
}
