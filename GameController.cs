using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameController : NetworkBehaviour
{
    public AudioController audioController;
    RunController runController;
    public GameView gameView;
    public InterfaceController interfaceController;
    public AssetData assets;
    public Stage stage; 
    public Player p1, p2; // players hold their selected tile co-ordinates
    public GameObject tile;
    public GameObject tileContainer;
    List<Tile> updatedTiles = new List<Tile>();

    public int DIMENSIONS = 10;

    float captureCost = 2.0f;
    float repairCost = 2.0f;

    void Awake()
    {
        Debug.Log("GameController awake");
        // LoadAssets(); // Load Prefabs/Materials from Resources folder
        assets = (AssetData)GameObject.FindObjectOfType(typeof(AssetData));

        audioController = (AudioController)GameObject.FindObjectOfType(typeof(AudioController));
        runController = (RunController)GameObject.FindObjectOfType(typeof(RunController));
        interfaceController = (InterfaceController)GameObject.FindObjectOfType(typeof(InterfaceController));
        stage = (Stage)GameObject.FindObjectOfType(typeof(Stage));

        tileContainer = (GameObject)GameObject.Find("TileContainer");

        /*
        p1 = new Player(1, stage.Dimensions);
        p2 = new Player(2, stage.Dimensions);
        p1.Opponent = p2;
        p2.Opponent = p1;
        */ 
        

        gameView = (GameView)GameObject.FindObjectOfType(typeof(GameView));
    }

    public void CreateTiles()
    {
        for (int i = 0; i < stage.Dimensions; i++)
        {
            for (int j = 0; j < stage.Dimensions; j++)
            {
                // instantiate a tile gameObject for every tile in the data array
                GameObject tileGameObject = Instantiate(tile, new Vector3(i * 1, 0, j * 1), Quaternion.identity);
               

                // give each tile a name in the inspector corresponding to its X/Y position
                tileGameObject.name = "tile_" + i + "_" + j;
                tileGameObject.transform.SetParent(tileContainer.transform);

                // give the Tile model in the 2D-array a reference to its corresponding instantiated Unity gameObject
                // stage.tiles[i, j].Tile_gameObj = tileGameObject;

                Tile.changedTiles.Add(stage.tiles[i, j]);

                NetworkServer.Spawn(tileGameObject);
            }
        }
        gameView.RpcUpdateTileAppearance();
    }

    void Update()
    {
        // PlayerUpdate();

        if (p1 != null && p2 != null)
        {
            IncrementPlayerEnergy(p1);
            IncrementPlayerEnergy(p2);
        }
    }

    public override void OnStartServer()
    {
        Debug.Log("started the server");
        // CreateTiles();
    }

    int playerCount = 0;
    public void OnPlayerConnected(NetworkPlayer networkplayer)
    {
        Debug.Log("Player " + playerCount++ + " connected from " + networkplayer.ipAddress + ":" + networkplayer.port);
    }

    [Command]
    public void CmdRegisterPlayer(GameObject player)
    {
        if (p1 == null)
        {
            p1 = player.GetComponent<Player>();
            p1.playerNumber = 1;
            gameView.p1 = p1;

            Debug.Log("registering player " + p1.playerNumber);
        }
        else // p2 has connected
        {
            p2 = player.GetComponent<Player>();
            p2.playerNumber = 2;
            gameView.p2 = p2;

            Debug.Log("registering player " + p2.playerNumber);

            p1.Opponent = p2;
            p2.Opponent = p1;

            InitializeGame();
        }
    }

    [Command]
    public void CmdChangeTileType(int x, int y, int playerNumber)
    {
        Debug.Log("Tile's parent's name is : " + stage.tiles[x, y].gameObject.transform.parent.name);
        if (playerNumber == 1)
        {
            stage.tiles[x, y].Type = TileType.Player1;
        }
        if (playerNumber == 2)
        {
            stage.tiles[x, y].Type = TileType.Player2;
        }
    }

    public void InitializeGame()
    {
        Debug.Log("initializing game");

        stage.SpawnTiles();
        gameView.stage = stage;

        gameView.RpcUpdateTileAppearance();
    }

    void PlayerUpdate()
    {
        // player 1 controls (WASD) ... todo: refactor
        if (Input.GetKeyDown("w") || Input.GetKeyDown("a") || Input.GetKeyDown("s") || Input.GetKeyDown("d"))
        {
            // the selected X and Y co-ordinates before applying changes
            int previousX = p1.X;
            int previousY = p1.Y;

            // change position of player 1's selected tile
            if (Input.GetKeyDown("w")) p1.Move(stage, Direction.up);
            if (Input.GetKeyDown("a")) p1.Move(stage, Direction.left);
            if (Input.GetKeyDown("s")) p1.Move(stage, Direction.down);
            if (Input.GetKeyDown("d")) p1.Move(stage, Direction.right);


            // add the previously occupied tile, and the tile landed on, to a list of tiles that should be updated, 
            // if they're not already there
            if (!Tile.changedTiles.Contains(stage.tiles[p1.X, p1.Y]))
                Tile.changedTiles.Add(stage.tiles[p1.X, p1.Y]);
            if (!Tile.changedTiles.Contains(stage.tiles[previousX, previousY]))
                Tile.changedTiles.Add(stage.tiles[previousX, previousY]);

            gameView.RpcUpdateTileAppearance();
        }

        if (Input.GetKeyDown("space"))
        {
            if ((stage.tiles[p1.X, p1.Y].Type == TileType.Void) && (p1.Energy > captureCost))
            {
                audioController.PlayCaptureSoundEffect();
                CaptureTile(p1);
            }

            // repair a friendly tile in a damaged state
            if ((stage.tiles[p1.X, p1.Y].Type == TileType.Player1) &&
                (stage.tiles[p1.X, p1.Y].Condition == TileCondition.Damaged) &&
                (p1.Energy > repairCost))
            {
                stage.tiles[p1.X, p1.Y].RepairTile();
                p1.ReduceEnergy(repairCost);
            }
            
            if (stage.tiles[p1.X, p1.Y].Type == TileType.Player2 && (p1.Energy > captureCost))
            {
                switch (stage.tiles[p1.X, p1.Y].Condition)
                {
                    case TileCondition.Damaged:
                        capturedTiles.Add(stage.tiles[p1.X, p1.Y]);
                        CaptureTile(p1);
                        break;
                    case TileCondition.Normal:
                        audioController.PlayShatterSound();
                        stage.tiles[p1.X, p1.Y].DamageTile(p1);
                        p1.ReduceEnergy(captureCost);
                        break;
                }
            }
            gameView.RpcUpdateTileAppearance();
        }

        // player 2 controls (arrows) ... refactor
        if (Input.GetKeyDown("up") || Input.GetKeyDown("down") || Input.GetKeyDown("left") || Input.GetKeyDown("right"))
        {
            // the selected X and Y co-ordinates before applying changes
            int previousX = p2.X;
            int previousY = p2.Y;

            // change the position of player 2's selected tile
            if (Input.GetKeyDown("up")) p2.Move(stage, Direction.up);
            if (Input.GetKeyDown("left")) p2.Move(stage, Direction.left);
            if (Input.GetKeyDown("down")) p2.Move(stage, Direction.down);
            if (Input.GetKeyDown("right")) p2.Move(stage, Direction.right);

            // add the previously occupied tile, and the tile landed on, to a list of tiles that should be updated, 
            // if they're not already there
            if (!Tile.changedTiles.Contains(stage.tiles[p2.X, p2.Y]))
                Tile.changedTiles.Add(stage.tiles[p2.X, p2.Y]);
            if (!Tile.changedTiles.Contains(stage.tiles[previousX, previousY]))
                Tile.changedTiles.Add(stage.tiles[previousX, previousY]);

            gameView.RpcUpdateTileAppearance();
        }

        if (Input.GetKeyDown("enter") || Input.GetKeyDown("/"))
        {
            if ((stage.tiles[p2.X, p2.Y].Type == TileType.Void) && (p2.Energy > captureCost))
            {
                audioController.PlayCaptureSoundEffect();
                CaptureTile(p2);
            }

            if ((stage.tiles[p2.X, p2.Y].Type == TileType.Player2) &&
                (stage.tiles[p2.X, p2.Y].Condition == TileCondition.Damaged) &&
                (p2.Energy > repairCost))
            {
                stage.tiles[p2.X, p2.Y].RepairTile();
                p2.ReduceEnergy(repairCost);
            }

            if (stage.tiles[p2.X, p2.Y].Type == TileType.Player1 && (p2.Energy > captureCost))
            {
                switch (stage.tiles[p2.X, p2.Y].Condition)
                {
                    case TileCondition.Damaged:
                        capturedTiles.Add(stage.tiles[p2.X, p2.Y]);
                        CaptureTile(p2);
                        break;
                    case TileCondition.Normal:
                        audioController.PlayShatterSound();
                        stage.tiles[p2.X, p2.Y].DamageTile(p2);
                        p2.ReduceEnergy(captureCost);
                        break;
                }
            }
            gameView.RpcUpdateTileAppearance();
        }
    }

    List<Tile> capturedTiles = new List<Tile>();
    // call this function when player takes a tile: pass in the player, and co-ordinates of tile to capture
    public void CaptureTile(Player player)
    {
        player.ReduceEnergy(captureCost);

        int x = player.X;
        int y = player.Y;
        int tempX = x;
        int tempY = y;

        List<Tile> tempCaptured = new List<Tile>();

        stage.tiles[x, y].Type = player.TileType;

        // check right
        tempX = x;
        while (tempX < DIMENSIONS - 1 && (stage.tiles[tempX + 1, y].Type == player.Opponent.TileType))
        {
            tempCaptured.Add(stage.tiles[tempX + 1, y]);
            Debug.Log("Added tile [ x:" + (tempX + 1) + ", y:" + y + "] to right branch.");

            if (tempX < DIMENSIONS-1)
            {
                tempX++;
                // if we have captured tiles and reach a friendly tile, convert the row (move to a bigger list)
                if ((tempX < DIMENSIONS-1) && (stage.tiles[tempX + 1, y].Type == player.TileType))
                {
                    capturedTiles.AddRange(tempCaptured);
                }
                // if the next iteration is out of bounds or tile has no owner, clear this branch
                else if ((tempX + 1 > DIMENSIONS-1) || stage.tiles[tempX + 1, y].Type == TileType.Void)
                {
                    tempCaptured.Clear();
                    break;
                }
            } else { break; }
        }

        // check left 
        tempX = x;
        while (tempX >= 1 && (stage.tiles[tempX - 1, y].Type == player.Opponent.TileType))
        {
            tempCaptured.Add(stage.tiles[tempX - 1, y]);
            Debug.Log("Added tile [ x:" + (tempX - 1) + ", y:" + y + "] to left branch.");

            if (tempX > 0)
            {
                tempX--;
                if ((tempX > 0) && stage.tiles[tempX - 1, y].Type == player.TileType)
                {
                    capturedTiles.AddRange(tempCaptured);
                }
                else if ((tempX - 1 < 0) || stage.tiles[tempX - 1, y].Type == TileType.Void)
                {
                    tempCaptured.Clear();
                    break;
                }
            } else { break; }
        }

        // check up
        tempY = y;
        while (tempY < DIMENSIONS - 1 && (stage.tiles[x, tempY + 1].Type == player.Opponent.TileType))
        {
            tempCaptured.Add(stage.tiles[x, tempY + 1]);
            Debug.Log("Added tile [ x:" + tempX + ", y:" + (tempY + 1) + "] to up branch.");

            if (tempY < DIMENSIONS - 1)
            {
                tempY++;
                if ((tempY < DIMENSIONS - 1) && stage.tiles[x, tempY + 1].Type == player.TileType)
                {
                    capturedTiles.AddRange(tempCaptured);
                }
                else if ((tempY + 1 > DIMENSIONS - 1) || stage.tiles[x, tempY + 1].Type == TileType.Void)
                {
                    tempCaptured.Clear();
                    break;
                }
            } else { break; }
        }

        // check down
        tempY = y;
        while (tempY >= 1 && (stage.tiles[x, tempY - 1].Type == player.Opponent.TileType))
        {
            tempCaptured.Add(stage.tiles[x, tempY - 1]);
            Debug.Log("Added tile [ x:" + tempX + ", y:" + (tempY - 1) + "] to down branch.");

            if (tempY > 0)
            {
                tempY--;
                if ((tempY > 0) && stage.tiles[x, tempY - 1].Type == player.TileType)
                {
                    capturedTiles.AddRange(tempCaptured);
                }
                else if ((tempY - 1 < 0) || stage.tiles[x, tempY - 1].Type == TileType.Void)
                {
                    tempCaptured.Clear();
                    break;
                }
            } else { break; }
        }

        AddCapturedToChanged(player);

        gameView.RpcUpdateTileAppearance();

        runController.BuildNodeMap(player);
    }


    // function to add the list of captured tiles generated by each branch, to a list of all tiles to be updated
    public void AddCapturedToChanged(Player player)
    {
        if (capturedTiles.Count > 0)
        {
            audioController.PlaySwapSoundEffect();

            foreach (Tile tile in capturedTiles)
            {
                Debug.Log("converting tile x:" + tile.X + " y: " + tile.Y);
                tile.Type = player.TileType;
                tile.Condition = TileCondition.Normal;
                Tile.changedTiles.Add(tile);
            }

            // check if the opponent's runner needs to update their runPath due to capture
            runController.CheckIfPathRequiresUpdate(player.Opponent, capturedTiles);

            gameView.RpcUpdateTileAppearance();

            capturedTiles.Clear();
        }
    }

    // increment the player's energy each frame
    void IncrementPlayerEnergy(Player player)
    {
        if (player.Energy < 10.0f) {
            player.Energy += Time.deltaTime * 1.0f;

            if (player.Energy > 10.0f)
                player.Energy = 10.0f;
        }
    }

    public void SignalVictory(Player player)
    {
        audioController.PlayVictorySound();
        interfaceController.ShowVictoryText(player);
    }
}
