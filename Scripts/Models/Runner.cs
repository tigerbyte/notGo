using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner  {

    public double x;
    public double y;
    public enum Owner { Player1, Player2 };
    Owner owner;
   
    public GameObject runner_gameObj;

    public Runner(Owner playerNumber)
    {
        owner = playerNumber;

    }
    public Runner(double x, double y, Owner playerNumber) {

        this.x = x;
        this.y = y;
        owner = playerNumber;

    }
    public GameObject Runner_gameObj {
        get { return runner_gameObj; }
        set { runner_gameObj = value; }

    }
}
