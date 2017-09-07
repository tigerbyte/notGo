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
        gameController = (GameController)FindObjectOfType(typeof(GameController));
        audioController = (AudioController)FindObjectOfType(typeof(AudioController));
        stage = gameController.stage;
        p1 = gameController.p1;
        p2 = gameController.p2;

        RunnerObject = (GameObject)Resources.Load("Prefabs/Runner");

        LoadAssets(); // [remove later]
    }

    void Update() {
        updateRunner(p1);
        updateRunner(p2);
    }

    void updateRunner(Player player)
    {
        // if no runner exists, instantiate the runner, 
        // toDo : instantiate runner earlier,  maybe offscreen, instead of checking on its existence every frame
        if (player.Runner == null)
        {
            for (int i = 0; i < stage.Dimensions; i++)
            {
                if (stage.tiles[i, player.startingRow].Type == player.TileType)
                {
                    Vector2 runnerSpawnPosition = new Vector2(stage.tiles[i, player.startingRow].X, stage.tiles[i, player.startingRow].Y);
                    player.constructRunner(runnerSpawnPosition);
                    player.Runner.GameObj = Instantiate(RunnerObject, new Vector3(runnerSpawnPosition.x, 1, runnerSpawnPosition.y), Quaternion.identity);

                    if (player == p1) { player.Runner.GameObj.GetComponent<Renderer>().material = blueMat; }
                    if (player == p2) { player.Runner.GameObj.GetComponent<Renderer>().material = redMat; }
                }
            }
        }
        else if (player.Runner.RunPath != null && player.Runner.RunPath.Count > 1)
        {
            if (player.Runner.GameObj.transform.position.z < player.Runner.RunPath[1].Y) { MoveRunnerObject(player.Runner, direction.up); }
            if (player.Runner.GameObj.transform.position.z > player.Runner.RunPath[1].Y) { MoveRunnerObject(player.Runner, direction.down); }
            if (player.Runner.GameObj.transform.position.x > player.Runner.RunPath[1].X) { MoveRunnerObject(player.Runner, direction.left); }
            if (player.Runner.GameObj.transform.position.x < player.Runner.RunPath[1].X) { MoveRunnerObject(player.Runner, direction.right); }

            player.Runner.ComparePositionToPath();
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

        if (pathDebugging == true) { Debug.Log("/// building node map ///"); }
        
        PathNode[,] pathNodes = new PathNode[stage.Dimensions, stage.Dimensions];
        // current tile being analyzed, starts at runner position but expands out by lowest distance using djikstra
        pathNodes[player.Runner.X, player.Runner.Y] = new PathNode(player.Runner.X, player.Runner.Y, 0);
        PathNode curr = pathNodes[player.Runner.X, player.Runner.Y];

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
            // TO DO : find a way to reduce duplicate code ( different block for each direction )
            Tile target; // the tile we're targetting to run checks on

            // next 4 blocks are basically the same thing 4 times , one for each direction : up / left / right / down
            // if we find tiles with friendly type, create a corresponding pathNode
            // add it to the list of orderedNodes, keeping track of its distance from the original source, and its parent so we can build a final path

            // Check Above ( Y + 1 )
            if (curr.Y < 9 && stage.tiles[curr.X, curr.Y + 1].Type == player.TileType && pathNodes[curr.X, curr.Y + 1] == null)
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
            if (curr.X > 0 && stage.tiles[curr.X - 1, curr.Y].Type == player.TileType && pathNodes[curr.X - 1, curr.Y] == null)
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
            if (curr.X < 9 && stage.tiles[curr.X + 1, curr.Y].Type == player.TileType && pathNodes[curr.X + 1, curr.Y] == null)
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
            if (curr.Y > 0 && stage.tiles[curr.X, curr.Y - 1].Type == player.TileType && pathNodes[curr.X, curr.Y - 1] == null)
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
    Vector3 MoveRunnerObject(Runner runner, direction dir)
    {
        audioController.PlayRunningSound();
        switch (dir)
        {
            case direction.up:
                runner.GameObj.transform.Translate(0, 0, runner.Speed * Time.deltaTime);
                break;
            case direction.down:
                runner.GameObj.transform.Translate(0, 0, -runner.Speed * Time.deltaTime);
                break;
            case direction.left:
                runner.GameObj.transform.Translate(-runner.Speed * Time.deltaTime, 0, 0);
                break;
            case direction.right:
                runner.GameObj.transform.Translate(runner.Speed * Time.deltaTime, 0, 0);
                break;
        }
        return runner.GameObj.transform.position;
    }

    // see if any of the nodes that have been captured, are in a runner's path, if so re-build runpath
    // note : comparing 2 lists against each other probably super inefficient 
    public void CheckIfPathRequiresUpdate(Player player, List<Tile> capturedTiles)
    {
        if (player.Runner != null) {
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
    }

    void LoadAssets() // temporary solution to visually indicate runner owner [remove later]
    { 
        blueMat = (Material)Resources.Load("Materials/BlueTileMat");
        redMat = (Material)Resources.Load("Materials/RedTileMat");
    }
}
