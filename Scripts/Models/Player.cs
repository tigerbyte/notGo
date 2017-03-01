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

	public int selectedX;
	public int selectedY;

	float tileCooldown = 3.0f;
	bool canPlay = true; // can play a tile (not on cooldown)

	public Player(short playerNumber) {
		if (playerNumber == 1) {
			this.selectedX = 0;
			this.selectedY = 0;
		} else if (playerNumber == 2) {
			this.selectedX = 9;
			this.selectedY = 9;
		}
	}

	public float TileCooldown {
		get { return tileCooldown;  }
		set { tileCooldown = value; }
	}

	public bool CanPlay {
		get { return canPlay; }
		set { canPlay = value; }
	}
}
