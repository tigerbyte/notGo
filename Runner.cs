using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner {

    // public Stage stage;
    // public Tile currentTile;
    public int x;
    public int y;
    public int playerNumber;
    public GameObject gameObj;

    List<PathNode> runPath;

    float epsilon = 0.02f; // margin of error for confirming movement (prevent gameObj from getting stuck at ~0.99 of destination)

    // Runner constructor. Assigned to Player 1 or 2 based on the number given.
    public Runner(int playerNumber) {
        this.playerNumber = playerNumber;
        Debug.Log("constructed a runner for player" + playerNumber);
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

    public Runner(int x, int y, int playerNumber)
    {
        this.x = x;
        this.y = y;
        this.playerNumber = playerNumber;
        Debug.Log("constructed a runner with position for Player" + playerNumber);
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
        // Debug.Log("Comparing -- RUN PATH X:" + RunPath[1].X + " Y:" + RunPath[1].Y);
        // Debug.Log("Aboluste Position Difference:" + Mathf.Abs(GameObj.transform.position.z - RunPath[1].Y));
        if (RunPath.Count > 1 && Mathf.Abs(GameObj.transform.position.x - RunPath[1].X) < epsilon && Mathf.Abs(GameObj.transform.position.z - RunPath[1].Y) < epsilon)
        {
            Debug.Log("new position reached, deleting node from runpath");
            RunPath.RemoveAt(0);
        }
    }
}
