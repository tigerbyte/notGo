using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    AudioController audioController;
    RunController runController;
    GameView gameView;
    public InterfaceController interfaceController;
    public AssetData assets;
    public Stage stage; // Stage contains a 2D array of tiles (Stage.tiles[x,y])
    public Player p1, p2; // players hold their selected tile co-ordinates
    public GameObject tile;
    public GameObject tileContainer;
    List<Tile> updatedTiles = new List<Tile>();

    public int DIMENSIONS = 10;

    void Awake()
    {
        // LoadAssets(); // Load Prefabs/Materials from Resources folder
        assets = ScriptableObject.CreateInstance<AssetData>();

        audioController = (AudioController)GameObject.FindObjectOfType(typeof(AudioController));
        runController = (RunController)GameObject.FindObjectOfType(typeof(RunController));
        interfaceController = (InterfaceController)GameObject.FindObjectOfType(typeof(InterfaceController));

        stage = new Stage(DIMENSIONS);

        tileContainer = (GameObject)GameObject.Find("TileContainer");

        p1 = new Player(1, stage.Dimensions);
        p2 = new Player(2, stage.Dimensions);
        p1.Opponent = p2;
        p2.Opponent = p1;

        gameView = ScriptableObject.CreateInstance<GameView>();

        CreateTiles(); // create tile game objects to associate with data structure
    }

    // Instantiate tile GameObjects, and assign them to the 2d Tile array data-structure 
    // that was created by Stage class . 
    // This is called once , on game startup. 
    void CreateTiles()
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
                stage.tiles[i, j].Tile_gameObj = tileGameObject;

                Tile.changedTiles.Add(stage.tiles[i, j]);
            }
        }
        gameView.UpdateTileAppearance();
    }

    // Update the player on every frame
    void Update()
    {
        PlayerUpdate();
        IncrementPlayerEnergy(p1);
        IncrementPlayerEnergy(p2);
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

            gameView.UpdateTileAppearance();
        }

        if (Input.GetKeyDown("space"))
        {
            if ((stage.tiles[p1.X, p1.Y].Type == Tile.TileType.Void) && (p1.Energy > 3.0f))
            {
                audioController.PlayCaptureSoundEffect();
                CaptureTile(p1);
            }

            // repair a friendly tile in a damaged state
            if ((stage.tiles[p1.X, p1.Y].Type == Tile.TileType.Player1) &&
                (stage.tiles[p1.X, p1.Y].Condition == Tile.TileCondition.Damaged) &&
                (p1.Energy > 2.0f))
            {
                stage.tiles[p1.X, p1.Y].RepairTile();
                p1.ReduceEnergy(2.0f);
            }
            
            if (stage.tiles[p1.X, p1.Y].Type == Tile.TileType.Player2 && (p1.Energy > 3.0f))
            {
                switch (stage.tiles[p1.X, p1.Y].Condition)
                {
                    case Tile.TileCondition.Damaged:
                        capturedTiles.Add(stage.tiles[p1.X, p1.Y]);
                        CaptureTile(p1);
                        break;
                    case Tile.TileCondition.Normal:
                        audioController.PlayShatterSound();
                        stage.tiles[p1.X, p1.Y].DamageTile(p1);
                        p1.ReduceEnergy(3.0f);
                        break;
                }
            }
            gameView.UpdateTileAppearance();
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

            gameView.UpdateTileAppearance();
        }

        if (Input.GetKeyDown("enter") || Input.GetKeyDown("/"))
        {
            if ((stage.tiles[p2.X, p2.Y].Type == Tile.TileType.Void) && (p2.Energy > 3.0f))
            {
                audioController.PlayCaptureSoundEffect();
                CaptureTile(p2);
            }

            if ((stage.tiles[p2.X, p2.Y].Type == Tile.TileType.Player2) &&
                (stage.tiles[p2.X, p2.Y].Condition == Tile.TileCondition.Damaged) &&
                (p2.Energy > 2.0f))
            {
                stage.tiles[p2.X, p2.Y].RepairTile();
                p2.ReduceEnergy(2.0f);
            }

            if (stage.tiles[p2.X, p2.Y].Type == Tile.TileType.Player1 && (p2.Energy > 3.0f))
            {
                switch (stage.tiles[p2.X, p2.Y].Condition)
                {
                    case Tile.TileCondition.Damaged:
                        capturedTiles.Add(stage.tiles[p2.X, p2.Y]);
                        CaptureTile(p2);
                        break;
                    case Tile.TileCondition.Normal:
                        audioController.PlayShatterSound();
                        stage.tiles[p2.X, p2.Y].DamageTile(p2);
                        p2.ReduceEnergy(3.0f);
                        break;
                }
            }
            gameView.UpdateTileAppearance();
        }
    }

    List<Tile> capturedTiles = new List<Tile>();
    // call this function when player takes a tile: pass in the player, and co-ordinates of tile to capture
    public void CaptureTile(Player player)
    {
        player.ReduceEnergy(3.0f);

        int x = player.X;
        int y = player.Y;
        int tempX = x;
        int tempY = y;

        List<Tile> tempCaptured = new List<Tile>();

        // collection of tiles to be switched

        stage.tiles[x, y].Type = player.TileType;

        // check right
        tempX = x;
        while (tempX < DIMENSIONS - 1 && (stage.tiles[tempX + 1, y].Type == player.Opponent.TileType))
        {
            tempCaptured.Add(stage.tiles[tempX + 1, y]);
            Debug.Log("Added tile [ x:" + (tempX + 1) + ", y:" + y + "] to right branch.");

            tempX++;
            // if we have captured tiles and reach a friendly tile, convert the row (move to a bigger list)
            if ((stage.tiles[tempX + 1, y] != null) && (stage.tiles[tempX + 1, y].Type == player.TileType))
            {
                capturedTiles.AddRange(tempCaptured);
            }
            // if the next iteration is out of bounds or tile has no owner, clear this branch
            else if ((tempX + 1 > DIMENSIONS) || stage.tiles[tempX + 1, y].Type == Tile.TileType.Void)
            {
                tempCaptured.Clear();
                break;
            }
        }

        // check left 
        tempX = x;
        while (tempX >= 1 && (stage.tiles[tempX - 1, y].Type == player.Opponent.TileType))
        {
            tempCaptured.Add(stage.tiles[tempX - 1, y]);
            Debug.Log("Added tile [ x:" + (tempX - 1) + ", y:" + y + "] to right branch.");

            tempX--;
            if ((stage.tiles[tempX - 1, y] != null) && stage.tiles[tempX - 1, y].Type == player.TileType)
            {
                capturedTiles.AddRange(tempCaptured);
            }
            else if ((tempX < 0) || stage.tiles[tempX - 1, y].Type == Tile.TileType.Void)
            {
                tempCaptured.Clear();
                break;
            }
        }

        // check up
        tempY = y;
        while (tempY < DIMENSIONS - 1 && (stage.tiles[x, tempY + 1].Type == player.Opponent.TileType))
        {
            tempCaptured.Add(stage.tiles[x, tempY + 1]);
            Debug.Log("Added tile [ x:" + tempX + ", y:" + (tempY + 1) + "] to right branch.");

            tempY++;
            if ((stage.tiles[x, tempY + 1] != null) && stage.tiles[x, tempY + 1].Type == player.TileType)
            {
                capturedTiles.AddRange(tempCaptured);
            }
            else if ((tempY > DIMENSIONS - 1) || stage.tiles[x, tempY + 1].Type == Tile.TileType.Void)
            {
                tempCaptured.Clear();
                break;
            }
        }

        // check down
        tempY = y;
        while (tempY >= 1 && (stage.tiles[x, tempY - 1].Type == player.Opponent.TileType))
        {
            tempCaptured.Add(stage.tiles[x, tempY - 1]);
            Debug.Log("Added tile [ x:" + tempX + ", y:" + (tempY - 1) + "] to right branch.");

            tempY--;
            if ((stage.tiles[x, tempY - 1] != null) && stage.tiles[x, tempY - 1].Type == player.TileType)
            {
                capturedTiles.AddRange(tempCaptured);
            }
            else if ((tempY < 0) || stage.tiles[x, tempY - 1].Type == Tile.TileType.Void)
            {
                tempCaptured.Clear();
                break;
            }
        }

        AddCapturedToChanged(player);

        gameView.UpdateTileAppearance();

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
                tile.Condition = Tile.TileCondition.Normal;
                Tile.changedTiles.Add(tile);
            }

            // check if the opponent's runner needs to update their runPath due to capture
            runController.CheckIfPathRequiresUpdate(player.Opponent, capturedTiles);

            gameView.UpdateTileAppearance();

            capturedTiles.Clear();
        }
    }

    // increment the player's energy each frame
    void IncrementPlayerEnergy(Player player)
    {
        if (player.Energy < 10.0f) { player.Energy += Time.deltaTime * 1.0f; }
    }

    public void SignalVictory(Player player)
    {
        audioController.PlayVictorySound();
        interfaceController.ShowVictoryText(player);
    }
}
