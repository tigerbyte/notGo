using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunController : MonoBehaviour {

    public bool pathDebugging = false; // set true or false based on whether you want to see path debugger msgs

    GameObject RunnerObject; // Runner GameObject

    // references to the gamecontroller and the players for convenience
    GameController gameController;
    AudioController audioController;
    Stage stage;
    Player p1;
    Player p2;

    Material redMat, blueMat; // temporary solution to visually indicate runner owner [remove later]

    void Start() {
        // get a reference to the GameController, which will allow us to access Players, runners, etc.
        gameController = (GameController)GameObject.FindObjectOfType(typeof(GameController));
        audioController = (AudioController)GameObject.FindObjectOfType(typeof(AudioController));
        stage = gameController.stage;
        p1 = gameController.player1;
        p2 = gameController.player2;

        RunnerObject = (GameObject)Resources.Load("Prefabs/Runner");

        LoadAssets(); // [remove later]
    }

    void Update() {
        // check if players have a runner on the board, if not, check if they own a tile in their respective first row for spawning
        checkP1RunnerStatus();
        checkP2RunnerStatus();
    }

    void checkP1RunnerStatus() {
        // possible TODO: use shortest path to spawn runner in the most optimal starting position
        // (currently spawns in the first available position found)

        if (p1.Runner == null) // if p1 doesn't have a runner yet then spawn one
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
        // if a path exists and there are still nodes in it
        else if (p1.Runner.RunPath != null && p1.Runner.RunPath.Count > 1)
        {
            // TO DO :: currently allows diagonal movement ( two ifs satisfied simultaneously ), Limit to unidirectional movement

            // Debug.Log("p1RunnerObject.transform.position.z = " + p1.Runner.gameObj.transform.position.z + " p1.Runner.RunPath[1].Y = " + p1.Runner.RunPath[1].Y);
            if (p1.Runner.gameObj.transform.position.z < p1.Runner.RunPath[1].Y) { MoveRunnerObject(p1.Runner.gameObj, direction.up); }
            if (p1.Runner.gameObj.transform.position.z > p1.Runner.RunPath[1].Y) { MoveRunnerObject(p1.Runner.gameObj, direction.down); }
            if (p1.Runner.gameObj.transform.position.x > p1.Runner.RunPath[1].X) { MoveRunnerObject(p1.Runner.gameObj, direction.left); }
            if (p1.Runner.gameObj.transform.position.x < p1.Runner.RunPath[1].X) { MoveRunnerObject(p1.Runner.gameObj, direction.right); }

            // check if we've reached a new tile, for updates
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
        // if a path exists and there are still nodes in it
        else if (p2.Runner.RunPath != null && p2.Runner.RunPath.Count > 1)
        {
            // TO DO :: currently allows diagonal movement ( two ifs satisfied simultaneously ), Limit to unidirectional movement

            // Debug.Log("p1RunnerObject.transform.position.z = " + p1.Runner.gameObj.transform.position.z + " p1.Runner.RunPath[1].Y = " + p1.Runner.RunPath[1].Y);
            if (p2.Runner.gameObj.transform.position.z < p2.Runner.RunPath[1].Y) { MoveRunnerObject(p2.Runner.gameObj, direction.up); }
            if (p2.Runner.gameObj.transform.position.z > p2.Runner.RunPath[1].Y) { MoveRunnerObject(p2.Runner.gameObj, direction.down); }
            if (p2.Runner.gameObj.transform.position.x > p2.Runner.RunPath[1].X) { MoveRunnerObject(p2.Runner.gameObj, direction.left); }
            if (p2.Runner.gameObj.transform.position.x < p2.Runner.RunPath[1].X) { MoveRunnerObject(p2.Runner.gameObj, direction.right); }

            // check if we've reached a new tile, for updates
            p2.Runner.ComparePositionToPath();
        }
    }

    List<PathNode> orderedNodes = new List<PathNode>(); // A list of nodes (Tile positions) ordered by least to greatest distance from runner

    // build a map of connected tiles for player 1
    public void BuildNodeMap(Player player)
    {
        // TO DO : Refactoring ...
        // Find a better way than using Tile AND PathNode objects
        // Lots of list work might be resource intensive
        // Use PathNode Explored bool to skip unnecessary work
        // seperate into different functions
        if (player.Runner == null) { return; }

        if (pathDebugging == true) {
            Debug.Log("Player 1 Max Y is = " + findMaxY());
            Debug.Log("/// building node map ///");
        }

        PathNode[,] pathNodes = new PathNode[10, 10];
        // current tile being analyzed, starts at runner position but expands out by lowest distance using djikstra
        pathNodes[player.Runner.X, player.Runner.Y] = new PathNode(player.Runner.X, player.Runner.Y, 0);
        PathNode curr = pathNodes[player.Runner.X, player.Runner.Y];

        Tile.TileType playersTileType = player.GetTileType();

        // clear any leftovers (from previous use) in the ordered list, just in case
        // the list is ordered by nodes with the lowest distance from our tile being analyzed, and gets cleared out as we expand out further in our search
        if (orderedNodes.Count > 0) { orderedNodes.Clear(); }

        // add the node corresponding to runner position to the first spot in the list
        orderedNodes.Add(pathNodes[curr.X, curr.Y]);

        // the node with the highest Y value that we can send the runner to, gets updated later if we find nodes with higher Y values
        PathNode destination = pathNodes[curr.X, curr.Y];

        if (pathDebugging == true) { Debug.Log("orderedNodes capacity is " + orderedNodes.Count + ": [ x = " + orderedNodes[0].X + ", y = " + orderedNodes[0].Y + " ]"); }

        while (orderedNodes.Count > 0)
        {
            // next 4 blocks are basically the same thing 4 times , one for each direction : up / left / right / down
            // if we find tiles with friendly type, create a corresponding pathNode
            // add it to the list of orderedNodes, keeping track of its distance from the original source, and its parent so we can build a final path

            // Check Above ( Y + 1 )
            if (curr.Y < 9 && stage.tiles[curr.X, curr.Y + 1].Type == playersTileType && pathNodes[curr.X, curr.Y + 1] == null)
            {
                pathNodes[curr.X, curr.Y + 1] = new PathNode(curr.X, curr.Y + 1, (pathNodes[curr.X, curr.Y].Distance + 1));
                orderedNodes.Add(pathNodes[curr.X, curr.Y + 1]);

                // if the newly searched node has no parent, or it's parent has a distance greater than current tile, set current tile as its new parent
                if (pathNodes[curr.X, curr.Y + 1].Parent == null || pathNodes[curr.X, curr.Y + 1].Parent.Distance > pathNodes[curr.X, curr.Y].Distance) {
                    pathNodes[curr.X, curr.Y + 1].Parent = pathNodes[curr.X, curr.Y];
                }

                if (pathDebugging == true) { Debug.Log("created a path node at x:" + curr.X + " y:" + (curr.Y + 1) + " :: with parent at x: " + curr.X + " y: " + curr.Y); }
            }

            // Check Left ( X - 1 )
            if (curr.X > 0 && stage.tiles[curr.X - 1, curr.Y].Type == playersTileType && pathNodes[curr.X - 1, curr.Y] == null)
            {
                pathNodes[curr.X - 1, curr.Y] = new PathNode(curr.X - 1, curr.Y, (pathNodes[curr.X, curr.Y].Distance + 1));
                orderedNodes.Add(pathNodes[curr.X - 1, curr.Y]);

                // if the newly searched node has no parent, or it's parent has a distance greater than current tile, set current tile as its new parent
                if (pathNodes[curr.X - 1, curr.Y].Parent == null || pathNodes[curr.X - 1, curr.Y].Parent.Distance > pathNodes[curr.X, curr.Y].Distance) {
                    pathNodes[curr.X - 1, curr.Y].Parent = pathNodes[curr.X, curr.Y];
                }

                if (pathDebugging == true) { Debug.Log("created a path node at x:" + (curr.X - 1) + " y:" + curr.Y + " :: with parent at x: " + curr.X + " y: " + curr.Y); }
            }

            // Check Right ( X + 1 )
            if (curr.X < 9 && stage.tiles[curr.X + 1, curr.Y].Type == playersTileType && pathNodes[curr.X + 1, curr.Y] == null)
            {
                pathNodes[curr.X + 1, curr.Y] = new PathNode(curr.X + 1, curr.Y, (pathNodes[curr.X, curr.Y].Distance + 1));
                orderedNodes.Add(pathNodes[curr.X + 1, curr.Y]);

                // if the newly searched node has no parent, or it's parent has a distance greater than current tile, set current tile as its new parent
                if (pathNodes[curr.X + 1, curr.Y].Parent == null || pathNodes[curr.X + 1, curr.Y].Parent.Distance > pathNodes[curr.X, curr.Y].Distance) {
                    pathNodes[curr.X + 1, curr.Y].Parent = pathNodes[curr.X, curr.Y];
                }

                if (pathDebugging == true) { Debug.Log("created a path node at x:" + (curr.X + 1) + " y:" + curr.Y + " :: with parent at x: " + curr.X + " y: " + curr.Y); }
            }

            // Check Below ( Y - 1 )
            if (curr.Y > 0 && stage.tiles[curr.X, curr.Y - 1].Type == playersTileType && pathNodes[curr.X, curr.Y - 1] == null)
            {
                pathNodes[curr.X, curr.Y - 1] = new PathNode(curr.X, curr.Y - 1, (pathNodes[curr.X, curr.Y].Distance + 1));
                orderedNodes.Add(pathNodes[curr.X, curr.Y - 1]);

                // if the newly searched node has no parent, or it's parent has a distance greater than current tile, set current tile as its new parent
                if (pathNodes[curr.X, curr.Y - 1].Parent == null || pathNodes[curr.X, curr.Y - 1].Parent.Distance > pathNodes[curr.X, curr.Y].Distance) {
                    pathNodes[curr.X, curr.Y - 1].Parent = pathNodes[curr.X, curr.Y];
                }

                if (pathDebugging == true) { Debug.Log("created a path node at x:" + curr.X + " y:" + (curr.Y - 1) + " :: with parent at x: " + curr.X + " y: " + curr.Y); }
            }



            // print out ordered set of nodes (least to most distance)
            if (pathDebugging == true && orderedNodes.Count > 0)
            {
                int elementNum = 0;
                string orderedList = "orderedNodes ";
                foreach (PathNode p in orderedNodes)
                {
                    orderedList += ("Element[" + elementNum + "] x:" + p.X + ", y:" + p.Y + ", Distance = " + p.Distance);
                    elementNum++;
                }
                Debug.Log(orderedList);
                elementNum = 0;
            }


            // keep the destination updated with the highest reachable Y tile
            if (player.playerNumber == 1)
            {
                if (orderedNodes[0].Y > destination.Y)
                {
                    if (pathDebugging == true) { Debug.Log("@@ UPDATING DESTINATION FROM Y=" + destination.Y + " TO Y=" + orderedNodes[0].Y); }
                    destination = orderedNodes[0];
                }
            }

            // keep the destination updated with the lowest reachable Y tile
            if (player.playerNumber == 2)
            {
                if (orderedNodes[0].Y < destination.Y)
                {
                    if (pathDebugging == true) { Debug.Log("@@ UPDATING DESTINATION FROM Y=" + destination.Y + " TO Y=" + orderedNodes[0].Y); }
                    destination = orderedNodes[0];
                }
            }



            // remove from list nodes to search through
            orderedNodes.RemoveAt(0);
            if (pathDebugging == true) { Debug.Log("REMOVING FROM orderedNodes, NODE AT X: " + orderedNodes[0].X + " Y: " + orderedNodes[0].Y); }

            // set current (the next source tile) to the next item in the list (lowest distance not already searched)
            if (orderedNodes.Count > 0)
            {
                if (pathDebugging == true) { Debug.Log("NEW ORDERNODES[0] EQUALS X: " + orderedNodes[0].X + " Y:" + orderedNodes[0].Y); }
                curr = pathNodes[orderedNodes[0].X, orderedNodes[0].Y];
                if (pathDebugging == true) { Debug.Log("CURRENT TILE IS X:" + curr.X + " Y: " + curr.Y); }
            } else
            {
                if (pathDebugging == true) { Debug.Log("Ordered Nodes COUNT = ZERO"); }
            }
        }

        // Sort path nodes from least to greatest distance
        orderedNodes.Sort();


        List<PathNode> runPath = new List<PathNode>();
        PathNode backTraceNode = destination;
        runPath.Add(backTraceNode);
        while (backTraceNode.Parent != null)
        {
            runPath.Insert(0, backTraceNode.Parent);
            backTraceNode = backTraceNode.Parent;
        }
        player.Runner.RunPath = runPath;


        if (pathDebugging == true)
        {
            int z = 0;
            string fullPath = "";
            foreach (PathNode p in runPath)
            {
                fullPath += (" -> [Element " + z + "]" + " X=" + p.X + ", Y=" + p.Y);
                // Debug.Log("runPath element[" + z + "] coordinates x=" + p.X + " y=" + p.Y);
                z++;
            }
            Debug.Log("Full Run Path" + fullPath);
            Debug.Log("@@ Runner Destination is X:" + destination.X + " Y:" + destination.Y);
        }


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
        switch (dir)
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

    // see if any of the nodes that have been captured, are in a runner's path, if so re-build runpath
    // note : comparing 2 lists against each other probably super inefficient 
    public void CheckIfPathRequiresUpdate(Player player, List<Tile> capturedTiles)
    {
        Debug.Log(" >>>>>> Checking if path requires Update <<<<<< ");

        foreach (Tile captured in capturedTiles)
        {
            foreach (PathNode currentPath in player.Runner.RunPath)
            {
                if ((captured.X == currentPath.X) && (captured.Y == currentPath.Y))
                {
                    Debug.Log(" >>>>> PATH REQUIRES UPDATE <<<<");
                    BuildNodeMap(player);
                    break;
                }  
            }
        }
    }

    int findMaxY()
    {
        int maxY = 0;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (stage.tiles[i, j].Type == Tile.TileType.Player1) 
                    if (stage.tiles[i, j].Y > maxY) { maxY = stage.tiles[i, j].Y; }
            }
        }
        return maxY;
    }

    void LoadAssets() // temporary solution to visually indicate runner owner [remove later]
    { 
        blueMat = (Material)Resources.Load("Materials/BlueTileMat");
        redMat = (Material)Resources.Load("Materials/RedTileMat");
    }
}
