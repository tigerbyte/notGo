using System.Collections;
using UnityEngine;

public class Player {

    /*
	public struct TilePosition {
		public int x;
		public int y;
	}
	TilePosition selectedTile;
	public TilePosition SelectedTile {
		get { return selectedTile; }
		set { selectedTile = value; }
	}
	*/

    public int playerNumber;
    public int selectedX;
    public int selectedY;
    public Runner runner;

    float tileCooldown = 3.0f;
    bool canPlay = true; // can play a tile (not on cooldown)

    // Construct the player, assign the player number, and assign the tile they're initially hovering over
    public Player(int playerNumber) {

        this.playerNumber = playerNumber;

        if (playerNumber == 1) {
            this.selectedX = 0;
            this.selectedY = 0;
        } else if (playerNumber == 2) {
            this.selectedX = 9;
            this.selectedY = 9;
        }
    }

    public float TileCooldown {
        get { return tileCooldown; }
        set { tileCooldown = value; }
    }

    public bool CanPlay {
        get { return canPlay; }
        set { canPlay = value; }
    }

    public Tile.TileType GetTileType() {
        if (playerNumber == 1) { return Tile.TileType.Player1; }
        else { return Tile.TileType.Player2; }
    }

    public Runner Runner {
        get { return runner; }
        set { runner = value;  }
    }

    // called by RunController
    public Runner constructRunner(int x, int y) {
        this.Runner = new Runner(x, y, this.playerNumber);
        return runner;
    }
}
