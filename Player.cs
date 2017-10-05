using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {
    
    public GameController gameController;
    public Player opponent;
    public Runner runner;
    public int startingRow = 0;

    // new just testing
    public GameView gameView;

    [SyncVar]
    public int playerNumber;

    [SyncVar]
    public int X;

    [SyncVar]
    public int Y;

    [SyncVar]
    public float energy;

    int dimensions = 10;

    public void Start()
    {
        // destroy this script on objects that don't belong to the player
        if (!isLocalPlayer) Destroy(this);

        this.X = 0;
        this.Y = 0;

        gameController = (GameController)FindObjectOfType(typeof(GameController));
        gameView = (GameView)GameObject.FindObjectOfType(typeof(GameView));

        //if (isLocalPlayer)
        if (isServer)
        gameController.CmdRegisterPlayer(this.gameObject);
    }


    public void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown("w") || Input.GetKeyDown("a") || Input.GetKeyDown("s") || Input.GetKeyDown("d"))
            {
                if (Input.GetKeyDown("w") && this.Y < dimensions - 1)
                {
                    this.Y = this.Y + 1;
                    transform.Translate(0, 0, 1);
                }

                if (Input.GetKeyDown("a") && this.X > 0)
                {
                    this.X = this.X - 1;
                    transform.Translate(-1, 0, 0);
                }

                if (Input.GetKeyDown("s") && this.Y > 0)
                {
                    this.Y = this.Y - 1;
                    transform.Translate(0, 0, -1);
                }

                if (Input.GetKeyDown("d") && this.X < dimensions - 1)
                {
                    this.X = this.X + 1;
                    transform.Translate(1, 0, 0);
                }
            }

            if (Input.GetKeyDown("space"))
            {
                gameController.audioController.PlayCaptureSoundEffect();
                Debug.Log("player " + this.playerNumber + " pressed space.");
                Debug.Log("# of items in SyncList is : " + gameController.stage.PrintListLength());

                CmdChangeTileType();
            }
        }
    }

    [Command]
    public void CmdChangeTileType()
    {
        gameController.stage.tiles[this.X, this.Y].type = this.TileType;
        gameView.RpcUpdateTileAppearance();
    }

    public float Energy
    {
        get { return this.energy; }
        set { this.energy = value; }
    }

    public void ReduceEnergy(float amount)
    {
        Energy = Energy - amount;
    }
    
    public TileType TileType {
        get
        {
            if (playerNumber == 1) { return TileType.Player1; }
            else { return TileType.Player2; }
        }
    }

    public void Move(Stage stage, Direction direction)
    {
        switch (direction)
        {
            case Direction.up:
                if (this.Y < stage.Dimensions - 1) this.Y++;
                break;
            case Direction.down:
                if (this.Y > 0) this.Y--;
                break;
            case Direction.left:
                if (this.X > 0) this.X--;
                break;
            case Direction.right:
                if (this.X < stage.Dimensions - 1) this.X++;
                break;
        }
    }

    public Player Opponent
    {
        get { return opponent; }
        set { opponent = value; }
    }

    public Runner Runner {
        get { return runner; }
        set { runner = value;  }
    }

    public Runner constructRunner(int x, int y) {
        Runner = new Runner(x, y, this);
        return runner;
    }

    public Runner constructRunner(Vector2 spawnPosition)
    {
        Runner = new Runner((int)spawnPosition.x, (int)spawnPosition.y, this);
        return runner;
    }
}

/*

public Player(int playerNumber, int dimensions)
{

    this.playerNumber = playerNumber;

    if (playerNumber == 1)
    {
        this.X = 0;
        this.Y = 0;
        startingRow = 0;
    }
    else if (playerNumber == 2)
    {
        this.X = dimensions - 1;
        this.Y = dimensions - 1;
        startingRow = dimensions - 1;
    }
}


    */