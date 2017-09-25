using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner {

    // public Stage stage;
    // public Tile currentTile;
    private int x;
    private int y;
    private float speed;
    private Player player;
    private GameObject gameObj;

    List<PathNode> runPath;

    float epsilon = 0.025f; // margin of error for confirming movement (prevent gameObj from getting stuck at ~0.99 of destination)

    // Runner constructor. Assigned to Player 1 or 2 based on the number given.
    public Runner(Player player) {
        this.Player = player;
    }

    public Runner(int x, int y, Player player)
    {
        this.x = x;
        this.y = y;
        this.Player = player;
        this.speed = 0.5f;
    }

    public int X
    {
        get { return x; }
        set { x = value; }
    }

    public int Y
    {
        get { return y; }
        set { y = value; }
    }

    public void setPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public float Speed
    {
        get { return speed; }
        set { this.speed = value; }
    }

    public Player Player
    {
        get { return player; }
        set { player = value; }
    }

    public GameObject GameObj
    {
        get { return gameObj; }
        set { gameObj = value; }
    }

    public List<PathNode> RunPath
    {
        get { return runPath; }
        set { runPath = value; }
    }

    public void RemoveFirstPathNode()
    {
        runPath.RemoveAt(0);
    }

    public void ComparePositionToPath()
    {
        if (RunPath.Count > 1 && Mathf.Abs(GameObj.transform.position.x - RunPath[1].X) < epsilon && Mathf.Abs(GameObj.transform.position.z - RunPath[1].Y) < epsilon)
        {
            Debug.Log("new position reached [ X:"+ RunPath[1].X+" Y:"+ RunPath[1].Y+" ], deleting node at [ X:" + RunPath[0].X + ", Y: " + RunPath[0].Y + " ] from runpath");

            this.setPosition(RunPath[1].X, RunPath[1].Y);
            RunPath.RemoveAt(0);

            string nodes = "";
            foreach (PathNode p in RunPath)
            {
                nodes += ("[ x: " + p.X + ", y: " + p.Y + " ], ");
            }
            Debug.Log("printing path: " + nodes); 
        } 
    }

    public bool HasReachedOtherSide()
    {
        if (this.Y == Player.Opponent.startingRow)
        {
            return true;
        }
        else { return false; }
    }
}
