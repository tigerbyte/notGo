using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerMove : NetworkBehaviour {

    Material greenMat;
	// Use this for initialization
	void Start () {

        greenMat = (Material)Resources.Load("Materials/GreenMat");
    }
	
	// Update is called once per frame
	void Update () {

        // if not the local player then dont allow movement
        if (!isLocalPlayer)
        {
            return;
        }

        var x = Input.GetAxis("Horizontal") * 0.1f;
        var z = Input.GetAxis("Vertical") * 0.1f;

        transform.Translate(x, 0, z);

	}

    public override void OnStartLocalPlayer()
    {
        GetComponent<Renderer>().material.color = Color.green; 
    }


}
