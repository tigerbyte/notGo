using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

	public AudioSource capture;
	public AudioSource swap;

	void Start () {
		// one audiosource for each sound effect
		capture = this.gameObject.GetComponents<AudioSource> () [0];
		swap = this.gameObject.GetComponents<AudioSource> () [1];
	}

	public void PlayCaptureSoundEffect() { capture.Play (); }
	public void PlaySwapSoundEffect()    { swap.Play ();    }  
		
	void Update () {}
}
