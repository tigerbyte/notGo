using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunController : MonoBehaviour {

    GameObject RunnerObject; // Runner GameObject

    // references to the gamecontroller and the players for convenience
    GameController gameController;
    AudioController audioController;
    Stage stage;
    Player p1;
    Player p2;

    Material redMat, blueMat; // temporary solution to visually indicate runner owner [remove later]

    void Start () {
        // get a reference to the GameController, which will allow us to access Players, runners, etc.
        gameController = (GameController) GameObject.FindObjectOfType(typeof(GameController));
        audioController = (AudioController) GameObject.FindObjectOfType(typeof(AudioController));
        stage = gameController.stage;
        p1 = gameController.player1;
        p2 = gameController.player2;

        RunnerObject = (GameObject) Resources.Load("Prefabs/Runner");

        LoadAssets(); // [remove later]
    }

	void Update () {
        // check if players have a runner on the board, if not, check if they own a tile in their respective first row for spawning
        checkP1RunnerStatus();
        checkP2RunnerStatus();
    }

    void checkP1RunnerStatus() {
        // possible TODO: use shortest path to spawn runner in the most optimal starting position
        // (currently spawns in the first available position found)
        if (p1.Runner == null)
        {
            for (int i = 0; i < gameController.DIMENSIONS; i++)
            {
                if (gameController.stage.tiles[i, 0].Type == Tile.TileType.Player1)
                {
                    // temporarily store the tile that will be used for runner spawning position
                    Tile spawnTile = gameController.stage.tiles[i, 0];

                    // construct the data structure of the runner for logic operations
                    p1.constructRunner(spawnTile.X, spawnTile.Y);

                    // instantiate the gameObject corresponding to the newly spawned runner
                    p1.Runner.gameObj = Instantiate(RunnerObject, new Vector3(spawnTile.X, 1, spawnTile.Y), Quaternion.identity);
                    p1.Runner.gameObj.GetComponent<Renderer>().material = blueMat; // temporary solution to visually indicate runner owner [remove later]
                }
            }
        }
        else if (p1.Runner.RunPath != null && p1.Runner.RunPath.Count > 1)
        {
            // Debug.Log("p1RunnerObject.transform.position.z = " + p1.Runner.gameObj.transform.position.z + " p1.Runner.RunPath[1].Y = " + p1.Runner.RunPath[1].Y);
            if (p1.Runner.gameObj.transform.position.z < p1.Runner.RunPath[1].Y) { MoveRunnerObject(p1.Runner.gameObj, direction.up); }
            if (p1.Runner.gameObj.transform.position.z > p1.Runner.RunPath[1].Y) { MoveRunnerObject(p1.Runner.gameObj, direction.down); }
            if (p1.Runner.gameObj.transform.position.x > p1.Runner.RunPath[1].X) { MoveRunnerObject(p1.Runner.gameObj, direction.left); }
            if (p1.Runner.gameObj.transform.position.x < p1.Runner.RunPath[1].X) { MoveRunnerObject(p1.Runner.gameObj, direction.right); }

            p1.Runner.ComparePositionToPath();
        }
    }

    void checkP2RunnerStatus() {
        if (p2.Runner == null)
        {
            for (int i = 9; i > 0; i--)
            {
                if (gameController.stage.tiles[i, 9].Type == Tile.TileType.Player2)
                {
                    // temporarily store the tile that will be used for runner spawning position
                    Tile spawnTile = gameController.stage.tiles[i, 9];

                    // construct the data structure of the runner for logic operations
                    p2.constructRunner(spawnTile.X, spawnTile.Y);

                    // instantiate the gameObject corresponding to the newly spawned runner
                    p2.Runner.gameObj = Instantiate(RunnerObject, new Vector3(spawnTile.X, 1, spawnTile.Y), Quaternion.identity);
                    p2.Runner.gameObj.GetComponent<Renderer>().material = redMat; // temporary solution to visually indicate runner owner [remove later]
                }
            }
        }
    }


    // should use stage.Dimensions instead of 10 , but having issues with that ( non static stage , initialization order )
    List<PathNode> orderedNodes = new List<PathNode>(); // ordered list for djikstra sequence

    // build a map of connected tiles for player 1
    public void BuildNodeMap() 
    {
        // TO DO : Refactoring ...
        // Find a better way than using Tile AND PathNode objects
        // Lots of list work might be resource intensive
        // Use PathNode Explored bool to skip unnecessary work
        // seperate into different functions

        Debug.Log("building node map");

        PathNode[,] pathNodes = new PathNode[10, 10]; // used below , needs global scope to maintain state between function calls 
        Tile curr = stage.tiles[p1.Runner.X, p1.Runner.Y]; // current tile, starts at runner but expands out by lowest distance

        pathNodes[curr.X, curr.Y] = new PathNode(curr.X, curr.Y, 0);
        pathNodes[curr.X, curr.Y].Explored = true;
        orderedNodes.Add(pathNodes[curr.X, curr.Y]);

        PathNode destination = pathNodes[curr.X, curr.Y]; // the node with the highest Y value that we can send the runner to 



        Debug.Log("orderedNodes capacity is " + orderedNodes.Count);
        while (orderedNodes.Count > 0)
        {
            // check above, left, right, below Current tile (curr) , which starts at runner position and expands out to friendly tiles
            // current tile is updated to the next tile with lowest distance following top/left/right/down pattern

            // Check Above ( Y + 1 )
            if (curr.Y < 9 && stage.tiles[curr.X, curr.Y + 1].Type == Tile.TileType.Player1 && pathNodes[curr.X, curr.Y + 1] == null)
            {
                pathNodes[curr.X, curr.Y + 1] = new PathNode(curr.X, curr.Y + 1, (pathNodes[curr.X, curr.Y].Distance + 1));
                orderedNodes.Add(pathNodes[curr.X, curr.Y + 1]);

                // if the newly searched node has no parent, or it's parent has a distance greater than current tile, set current tile as its new parent
                if (pathNodes[curr.X, curr.Y + 1].Parent == null || pathNodes[curr.X, curr.Y + 1].Parent.Distance > pathNodes[curr.X, curr.Y].Distance) {
                    pathNodes[curr.X, curr.Y + 1].Parent = pathNodes[curr.X, curr.Y];
                }

                Debug.Log("created a path node at x:" + curr.X + " y:" + (curr.Y + 1) + " :: with parent at x: " + curr.X + " y: " + curr.Y);
            }

            // Check Left ( X - 1 )
            if (curr.X > 0 && stage.tiles[curr.X - 1, curr.Y].Type == Tile.TileType.Player1 && pathNodes[curr.X - 1, curr.Y] == null)
            {
                pathNodes[curr.X - 1, curr.Y] = new PathNode(curr.X - 1, curr.Y, (pathNodes[curr.X, curr.Y].Distance + 1));
                orderedNodes.Add(pathNodes[curr.X - 1, curr.Y]);

                // if the newly searched node has no parent, or it's parent has a distance greater than current tile, set current tile as its new parent
                if (pathNodes[curr.X - 1, curr.Y].Parent == null || pathNodes[curr.X - 1, curr.Y].Parent.Distance > pathNodes[curr.X, curr.Y].Distance) {
                    pathNodes[curr.X - 1, curr.Y].Parent = pathNodes[curr.X, curr.Y];
                }

                Debug.Log("created a path node at x:" + (curr.X - 1) + " y:" + curr.Y + " :: with parent at x: " + curr.X + " y: " + curr.Y);
            }

            // Check Right ( X + 1 )
            if (curr.X < 9 && stage.tiles[curr.X + 1, curr.Y].Type == Tile.TileType.Player1 && pathNodes[curr.X + 1, curr.Y] == null)
            {
                pathNodes[curr.X + 1, curr.Y] = new PathNode(curr.X + 1, curr.Y, (pathNodes[curr.X, curr.Y].Distance + 1));
                orderedNodes.Add(pathNodes[curr.X + 1, curr.Y]);

                // if the newly searched node has no parent, or it's parent has a distance greater than current tile, set current tile as its new parent
                if (pathNodes[curr.X + 1, curr.Y].Parent == null || pathNodes[curr.X + 1, curr.Y].Parent.Distance > pathNodes[curr.X, curr.Y].Distance) {
                    pathNodes[curr.X + 1, curr.Y].Parent = pathNodes[curr.X, curr.Y];
                }

                Debug.Log("created a path node at x:" + (curr.X + 1) + " y:" + curr.Y + " :: with parent at x: " + curr.X + " y: " + curr.Y);
            }

            // Check Below ( Y - 1 )
            if (curr.Y > 0 && stage.tiles[curr.X, curr.Y - 1].Type == Tile.TileType.Player1 && pathNodes[curr.X, curr.Y - 1] == null)
            {
                pathNodes[curr.X, curr.Y - 1] = new PathNode(curr.X, curr.Y - 1, (pathNodes[curr.X, curr.Y].Distance + 1));
                orderedNodes.Add(pathNodes[curr.X, curr.Y - 1]);

                // if the newly searched node has no parent, or it's parent has a distance greater than current tile, set current tile as its new parent
                if (pathNodes[curr.X, curr.Y - 1].Parent == null || pathNodes[curr.X, curr.Y - 1].Parent.Distance > pathNodes[curr.X, curr.Y].Distance) {
                    pathNodes[curr.X, curr.Y - 1].Parent = pathNodes[curr.X, curr.Y];
                }

                Debug.Log("created a path node at x:" + curr.X + " y:" + (curr.Y - 1) + " :: with parent at x: " + curr.X + " y: " + curr.Y);
            }

            // Sort path nodes from least to greatest distance
            orderedNodes.Sort(); 

            // print out ordered set of nodes (least to most distance)
            if (orderedNodes.Count > 0)
            {
                Debug.Log("-- PRINTING ORDERED LIST --");
                int elementNum = 0;
                foreach (PathNode p in orderedNodes)
                {
                    Debug.Log("orderedNodes Element[" + elementNum + "] x: " + p.X + " y: " + p.Y + " Distance = " + p.Distance);
                    elementNum++;
                }
                elementNum = 0;
            }

            // keep the destination updated with the highest reachable Y tile
            if (orderedNodes[0].Y > destination.Y) {
                Debug.Log("@@ UPDATING DESTINATION FROM Y=" + destination.Y + " TO Y=" + orderedNodes[0].Y);
                destination = orderedNodes[0];
            }

            // remove from list nodes to search through
            Debug.Log("REMOVING NODE AT X: " + orderedNodes[0].X + " Y: " + orderedNodes[0].Y);
            orderedNodes.RemoveAt(0);

            // set current (the next source tile) to the next item in the list (lowest distance not already searched)
            if (orderedNodes.Count > 0)
            {
                curr = stage.tiles[orderedNodes[0].X, orderedNodes[0].Y];
                Debug.Log("CURRENT TILE IS X:" + curr.X + " Y: " + curr.Y);
            }
        }


        List<PathNode> runPath = new List<PathNode>();
        PathNode backTraceNode = destination;
        runPath.Add(backTraceNode);
        while (backTraceNode.Parent != null)
        {
            runPath.Insert(0, backTraceNode.Parent);
            backTraceNode = backTraceNode.Parent;
        }
        p1.Runner.RunPath = runPath;

        int z = 0;
        foreach(PathNode p in runPath)
        {
            Debug.Log("runPath element[" + z + "] coordinates x=" + p.X + " y=" + p.Y);
            z++;                
        }
        Debug.Log("@@ Runner Destination is X:" + destination.X + " Y:" + destination.Y);
        
        
        // Debug.Log("PathNode+1 -> X:" + pathNodes[curr.X, curr.Y + 1].X + " Y:" + pathNodes[curr.X, curr.Y + 1].Y + " distance:" + pathNodes[curr.X, curr.Y + 1].Distance + " explored:" + pathNodes[curr.X, curr.Y + 1].Explored);
    }

    // put code in here from the nodeMap when refactoring
    List<PathNode> GenerateRunnerPath() {
        return new List<PathNode>();
    }

    // shift the position of the runnerObject every frame, and return its new position 
    // to compare with next node in runPath
    enum direction { up, down, left, right };
    float runSpeed = 1.5f;
    Vector3 MoveRunnerObject(GameObject runObj, direction dir)
    {
        audioController.PlayRunningSound();
        switch(dir)
        {
            case direction.up:
                runObj.transform.Translate(0, 0, runSpeed * Time.deltaTime);
                break;
            case direction.down:
                runObj.transform.Translate(0, 0, -runSpeed * Time.deltaTime);
                break;
            case direction.left:
                runObj.transform.Translate(-runSpeed * Time.deltaTime, 0, 0);
                break;
            case direction.right:
                runObj.transform.Translate(runSpeed * Time.deltaTime, 0, 0);
                break;
        }
        return runObj.transform.position;
    }


    void LoadAssets() // temporary solution to visually indicate runner owner [remove later]
    { 
        blueMat = (Material)Resources.Load("Materials/BlueTileMat");
        redMat = (Material)Resources.Load("Materials/RedTileMat");
    }
}
