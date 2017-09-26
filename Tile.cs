using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{

    public static List<Tile> changedTiles = new List<Tile>(); // a static list of tiles that have had their state changed
    public static List<Tile> changedCondition = new List<Tile>();
    public enum TileType { Void, Player1, Player2 };
    TileType type;
    public enum TileCondition { Damaged, Normal, Reinforced };
    TileCondition condition;
    int x;
    int y;

    public bool typeHasChanged; // flag to determine whether material should be updated

    public GameObject tile_gameObj;


    public Tile(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.Type = TileType.Void;
        this.Condition = TileCondition.Normal;
    }

    public Tile(int x, int y, TileType type)
    {
        this.x = x;
        this.y = y;
        this.Type = type;
        this.Condition = TileCondition.Normal;
    }

    public int X { get { return x; } }
    public int Y { get { return y; } }

    public GameObject Tile_gameObj
    {
        get { return tile_gameObj; }
        set { tile_gameObj = value; }
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
        get { return Tile_gameObj.GetComponent<Renderer>(); }
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
