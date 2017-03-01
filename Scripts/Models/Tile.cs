using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {

	public static List<Tile> changedTiles = new List<Tile>(); // a static list of tiles that have had their state changed
	public enum TileType { Void, Player1, Player2 };
	TileType type; // the type of this tile (void/p1/p2)
	int x;
	int y;

	public bool typeHasChanged; // flag to determine whether material should be updated

	public GameObject tile_gameObj;

	public int X {
		get { return x; }
	}

	public int Y {
		get { return y; }
	}

	public TileType Type {
		get { return type;  }
		set { 
			typeHasChanged = true;
			changedTiles.Add (this); // add this to list of tiles that have had their states changed
			type = value; 
		}
	}		

	public Tile(int x, int y) {
		this.x = x;
		this.y = y;
		type = TileType.Void;
	}

	public Tile(int x, int y, TileType type) {
		this.x = x;
		this.y = y;
		this.type = type;
	}

	public GameObject Tile_gameObj {
		get { return tile_gameObj; }
		set { tile_gameObj = value; }
	}
}
