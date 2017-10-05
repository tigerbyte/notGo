using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Tile : NetworkBehaviour
{
    public static List<Tile> changedTiles = new List<Tile>(); // a static list of tiles that have had their state changed
    public static List<Tile> changedCondition = new List<Tile>();
    public GameController gameController;

    [SyncVar]
    public TileType type;

    [SyncVar]
    public TileCondition condition;

    [SyncVar]
    public int x;

    [SyncVar]
    public int y;

    public AssetData assets;

    public bool typeHasChanged; // flag to determine whether material should be updated

    public void Start()
    {
        gameController = (GameController)GameObject.FindObjectOfType(typeof(GameController));

        Type = TileType.Void;
        Condition = TileCondition.Normal;

        this.gameObject.GetComponent<Renderer>().material = gameController.assets.voidMat;
    }

    public void Update()
    {
        
    }

    public Tile TileData
    {
        get { return this; }
    }

    public int X
    {
        get { return x; }
        set { this.x = value; }

    }

    public int Y
    {
        get { return y; }
        set { this.y = value; }
    }

    public TileType Type
    {
        get { return type; }
        set
        {
            typeHasChanged = true;
            changedTiles.Add(this); // add this to list of tiles that have had their states changed
            type = value;
        }
    }

    public TileType OpponentType
    {
        get
        {
            if (Type == TileType.Player1) { return TileType.Player2; }
            else if (Type == TileType.Player2) { return TileType.Player1; }
            else { return TileType.Void; }
        }
    }

    public TileCondition Condition
    {
        get { return this.condition; }
        set { this.condition = value; }
    }

    public Renderer Renderer
    {
        get { return this.GetComponent<Renderer>(); }
    }

    public void DamageTile(Player attacker)
    {
        switch (this.condition)
        {
            case TileCondition.Damaged:
                // this.type = attacker.TileType;
                this.condition = TileCondition.Normal;
                break;
            case TileCondition.Normal:
                changedCondition.Add(this);
                this.condition = TileCondition.Damaged;
                break;
            case TileCondition.Reinforced:
                this.condition = TileCondition.Normal;
                break;
        }
        changedCondition.Add(this);
    }

    public void RepairTile() // could combine into a 'reinforce' mechanic
    {
        this.condition = TileCondition.Normal;
    }
}
