using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour {

	AudioController audio;
	enum direction { up, down, left, right };
	public Stage stage; // Stage contains a 2D array of tiles (Stage.tiles[x,y])
	Player player1, player2; // players hold their selected tile co-ordinates
	public GameObject tile;
	Material voidMat, voidRedSelected, voidBlueSelected, redOwnedBlueSelected, redOwnedRedSelected, blueOwnedRedSelected, blueOwnedBlueSelected, blueMat, redMat;
   	List<Tile> updatedTiles = new List<Tile>(); // a list of tiles whose states have changed (could be static in tile class??)

	public const int DIMENSIONS = 10;

	void Start () {
		LoadAssets (); // Load Prefabs/Materials from Resources folder
		audio = (AudioController) GameObject.FindObjectOfType(typeof(AudioController));
		player1 = new Player (1);
		player2 = new Player (2);
        stage = new Stage (DIMENSIONS); // instantiate a square stage with height/width of dimensions
		CreateTiles (); // create tile game objects to associate with data structure

        
	}

	// something
	void CreateTiles () { // create tile game objects
		for (int i = 0; i < stage.Dimensions; i++) {
			for (int j = 0; j < stage.Dimensions; j++) {
				// instantiate a tile gameObject for every tile in the data array
				GameObject tile_go = Instantiate (tile, new Vector3 (i * 1, 0, j * 1), Quaternion.identity);
				tile_go.name = "tile_" + i + "_" + j; // give tile a name based on its position

				stage.tiles[i, j].Tile_gameObj = tile_go; // give the Tile data a reference to the gameObject

				Tile.changedTiles.Add(stage.tiles[i, j]);
			}
		}
		Debug.Log ("created tilezz");
		UpdateTiles ();
	}

    
   
  


   
    

 

	void UpdateTiles () {
		foreach (Tile t in Tile.changedTiles) {
			// Debug.Log ("running case for tile at x=" + t.X + " and y=" + t.Y);
			switch (t.Type) {
			case Tile.TileType.Void:
				if (t.X == player1.selectedX && t.Y == player1.selectedY) {
					t.Tile_gameObj.GetComponent<Renderer> ().material = voidBlueSelected; 
				} else if (t.X == player2.selectedX && t.Y == player2.selectedY) {
					t.Tile_gameObj.GetComponent<Renderer> ().material = voidRedSelected;
				} else {
					t.Tile_gameObj.GetComponent<Renderer> ().material = voidMat;
				}
				break;
			case Tile.TileType.Player1:
				if (t.X == player1.selectedX && t.Y == player1.selectedY) {
					t.Tile_gameObj.GetComponent<Renderer> ().material = blueOwnedBlueSelected; 
				} else if (t.X == player2.selectedX && t.Y == player2.selectedY) {
					t.Tile_gameObj.GetComponent<Renderer> ().material = blueOwnedRedSelected;

				} else {
					t.Tile_gameObj.GetComponent<Renderer> ().material = blueMat;



				}
				break;
			case Tile.TileType.Player2:
				if (t.X == player1.selectedX && t.Y == player1.selectedY) {
					t.Tile_gameObj.GetComponent<Renderer> ().material = redOwnedRedSelected; 
				} else if (t.X == player2.selectedX && t.Y == player2.selectedY) {
					t.Tile_gameObj.GetComponent<Renderer> ().material = redOwnedBlueSelected;
				} else {
					t.Tile_gameObj.GetComponent<Renderer> ().material = redMat;
				}
				break;
			}
			// Tile.changedTiles.Remove (t);
		}
		Tile.changedTiles.Clear (); // clear the list
	}
		
	void Update () {
		PlayerUpdate ();

	}

	// move the given player in the given direction
	void MovePlayer(Player p, direction d) { 
		switch (d) {
		case direction.up:
			if (p.selectedY < DIMENSIONS-1) p.selectedY++;
			break;
		case direction.down:
			if (p.selectedY > 0) p.selectedY--;
			break;
		case direction.left:
			if (p.selectedX > 0) p.selectedX--;
			break;
		case direction.right:
			if (p.selectedX < DIMENSIONS-1) p.selectedX++;
			break;
		}
	}

	void PlayerUpdate() { 
		// to do: split controls for networking

		// player 1 controls (WASD) ... refactor
		if (Input.GetKeyDown ("w") || Input.GetKeyDown ("a") || Input.GetKeyDown ("s") || Input.GetKeyDown ("d")) {
			// the selected X and Y co-ordinates before applying changes
			int previousX = player1.selectedX;
			int previousY = player1.selectedY;

			// change position of player 1's selected tile
			if (Input.GetKeyDown ("w")) MovePlayer(player1, direction.up);    
			if (Input.GetKeyDown ("a")) MovePlayer(player1, direction.left); 
			if (Input.GetKeyDown ("s")) MovePlayer(player1, direction.down); 
			if (Input.GetKeyDown ("d")) MovePlayer(player1, direction.right);

			// add the previously occupied tile, and the tile landed on, to a list of tiles that should be updated, 
			// if they're not already there
			if (!Tile.changedTiles.Contains(stage.tiles [player1.selectedX, player1.selectedY]))
				Tile.changedTiles.Add (stage.tiles [player1.selectedX, player1.selectedY]);
			if (!Tile.changedTiles.Contains (stage.tiles [previousX, previousY])) 
				Tile.changedTiles.Add (stage.tiles [previousX, previousY]);
			
			// graphically update the tiles in the list
			UpdateTiles ();
		}
			
		if (Input.GetKeyDown ("space")) {
			if (stage.tiles [player1.selectedX, player1.selectedY].Type == Tile.TileType.Void) {
				CaptureTile (1, player1.selectedX, player1.selectedY);
			}
		}

		// player 2 controls (arrows) ... refactor
		if (Input.GetKeyDown ("up") || Input.GetKeyDown ("down") || Input.GetKeyDown ("left") || Input.GetKeyDown ("right")) {
			// the selected X and Y co-ordinates before applying changes
			int previousX = player2.selectedX;
			int previousY = player2.selectedY;

			// change the position of player 2's selected tile
			if (Input.GetKeyDown ("up"))    MovePlayer(player2, direction.up); 
			if (Input.GetKeyDown ("left"))  MovePlayer(player2, direction.left); 
			if (Input.GetKeyDown ("down"))  MovePlayer(player2, direction.down); 
			if (Input.GetKeyDown ("right")) MovePlayer(player2, direction.right);

			// add the previously occupied tile, and the tile landed on, to a list of tiles that should be updated, 
			// if they're not already there
			if (!Tile.changedTiles.Contains(stage.tiles [player2.selectedX, player2.selectedY]))
				Tile.changedTiles.Add (stage.tiles [player2.selectedX, player2.selectedY]);
			if (!Tile.changedTiles.Contains (stage.tiles [previousX, previousY])) 
				Tile.changedTiles.Add (stage.tiles [previousX, previousY]);

			// graphically update the tiles in the list
            UpdateTiles ();
		}

		if (Input.GetKeyDown ("enter")) {
			if (stage.tiles [player2.selectedX, player2.selectedY].Type == Tile.TileType.Void) {
				CaptureTile (2, player2.selectedX, player2.selectedY);
			}
        }
	}




	// call this function when player takes a tile: pass in the player Number(1/2), and co-ordinates of tile to capture
	// runnerUpdate() is called when a tile is updated
	public void CaptureTile (int playerNum, int x, int y) {

		audio.PlayCaptureSoundEffect ();

		int tempX = x;
		int tempY = y;

		// collection of tiles to be switched
		List<Tile> capturedTiles = new List<Tile> (); // refreshes after searching each branch

		if (playerNum == 1) {
			stage.tiles [x, y].Type = Tile.TileType.Player1;

			tempX = x;
			while (tempX < DIMENSIONS - 1 && (stage.tiles [tempX + 1, y].Type == Tile.TileType.Player2)) {
				capturedTiles.Add (stage.tiles [tempX + 1, y]);

				tempX++;
				// if we have captured tiles and reach a friendly tile, convert the row (move to a bigger list)
				if (capturedTiles.Count > 0 && stage.tiles [tempX + 1, y].Type == Tile.TileType.Player1) {
					AddCapturedToChanged (1, capturedTiles);
					// if the next iteration is out of bounds or tile has no owner, kill this branch
				} else if ((tempX + 1 > DIMENSIONS) || stage.tiles [tempX + 1, y].Type == Tile.TileType.Void) {
					capturedTiles.Clear ();
				} 
			} 

			// check left branch
			tempX = x;   //reset search params
			while (tempX >= 1 && (stage.tiles [tempX - 1, y].Type == Tile.TileType.Player2)) {
				capturedTiles.Add (stage.tiles [tempX - 1, y]);

				tempX--;
				if (capturedTiles.Count > 0 && stage.tiles [tempX - 1, y].Type == Tile.TileType.Player1) {
					AddCapturedToChanged (1, capturedTiles);
				} else if ((tempX < 0) || stage.tiles [tempX - 1, y].Type == Tile.TileType.Void) {
					capturedTiles.Clear ();
				} 
			} 

			// check up
			tempY = y;
			while (tempY < DIMENSIONS - 1 && (stage.tiles [x, tempY + 1].Type == Tile.TileType.Player2)) {
				capturedTiles.Add (stage.tiles [x, tempY + 1]);

				tempY++;
				if (capturedTiles.Count > 0 && stage.tiles [x, tempY + 1].Type == Tile.TileType.Player1) {
					AddCapturedToChanged (1, capturedTiles);
				} else if ((tempY > DIMENSIONS - 1) || stage.tiles [x, tempY + 1].Type == Tile.TileType.Void) { 
					capturedTiles.Clear (); 
				} 
			}

			//check down
			tempY = y;
			while (tempY >= 1 && (stage.tiles [x, tempY - 1].Type == Tile.TileType.Player2)) {
				capturedTiles.Add (stage.tiles [x, tempY - 1]);

				tempY--;
				if (capturedTiles.Count > 0 && stage.tiles [x, tempY - 1].Type == Tile.TileType.Player1) {
					AddCapturedToChanged (1, capturedTiles);
				} else if ((tempY < 0) || stage.tiles [x, tempY - 1].Type == Tile.TileType.Void) {
					capturedTiles.Clear ();
				} 
			}
		
			//////////////////////
			////// Player 2 //////
			//////////////////////

		} else if (playerNum == 2) {
			stage.tiles [x, y].Type = Tile.TileType.Player2;
			capturedTiles.Clear ();

			tempX = x;
			while (tempX < DIMENSIONS - 1 && (stage.tiles [tempX + 1, y].Type == Tile.TileType.Player1)) {
				capturedTiles.Add (stage.tiles [tempX + 1, y]);

				tempX++;
				if (capturedTiles.Count > 0 && stage.tiles [tempX + 1, y].Type == Tile.TileType.Player2) {
					AddCapturedToChanged (2, capturedTiles);
				} else if ((tempX +1 > DIMENSIONS) || stage.tiles [tempX + 1, y].Type == Tile.TileType.Void) {
					capturedTiles.Clear ();
				}
			}
				
			tempX = x;   //reset search params
			while (tempX > 0 && (stage.tiles [tempX - 1, y].Type == Tile.TileType.Player1)) {
				capturedTiles.Add (stage.tiles [tempX - 1, y]);

				tempX--;
				if (capturedTiles.Count > 0 && stage.tiles [tempX - 1, y].Type == Tile.TileType.Player2) {
					AddCapturedToChanged (2, capturedTiles);
				} else if ((tempX < 0) || stage.tiles [tempX - 1, y].Type == Tile.TileType.Void) {
					capturedTiles.Clear ();
				} 
			}
				
			tempY = y;
			while (tempY < DIMENSIONS - 1 && (stage.tiles [x, tempY + 1].Type == Tile.TileType.Player1)) {
				capturedTiles.Add (stage.tiles [x, tempY + 1]);

				tempY++;
				if (capturedTiles.Count > 0 && stage.tiles [x, tempY + 1].Type == Tile.TileType.Player2) {
					AddCapturedToChanged (2, capturedTiles);
				} else if ((tempY + 1 > DIMENSIONS) || stage.tiles [x, tempY + 1].Type == Tile.TileType.Void) {
					capturedTiles.Clear ();
				} 
			}
				
			tempY = y;
			while ((tempY > 0) && (stage.tiles [x, tempY - 1].Type == Tile.TileType.Player1)) {
				capturedTiles.Add (stage.tiles [x, tempY - 1]);
                
				tempY--;
				if (capturedTiles.Count > 0 && stage.tiles [x, tempY - 1].Type == Tile.TileType.Player2) {
					AddCapturedToChanged (2, capturedTiles);
				} else if ((tempY < 0) || (stage.tiles [x, tempY - 1].Type == Tile.TileType.Void)) {
					capturedTiles.Clear ();
				} 
			}
		}
		// update all the tiles that have been changed
		UpdateTiles ();

	}
	

	// function to add the list of captured tiles generated by each branch, to a list of all tiles to be updated

	public void AddCapturedToChanged(int playerNum, List<Tile> capturedTiles) {
		Debug.Log ("capturedTiles capacity is : " + capturedTiles.Count);

		if (capturedTiles.Count > 0) {
			audio.PlaySwapSoundEffect (); 

			foreach (Tile t in capturedTiles) {
				// Debug.Log ("this captured tile X=" + t.X + ", Y=" + t.Y);
				if (playerNum == 1) t.Type = Tile.TileType.Player1;


				if (playerNum == 2) t.Type = Tile.TileType.Player2;
				Tile.changedTiles.Add (t);
			}
			capturedTiles.Clear();

		}

	}

	void LoadAssets() { // Load Prefabs/Materials from Resources folder
		// load from the Assets/Resources Folder
		tile = (GameObject)Resources.Load ("Prefabs/TilePrefab");

		// Load Materials
		voidMat = (Material)Resources.Load ("Materials/VoidTileMat");
		voidRedSelected = (Material)Resources.Load ("Materials/VoidRedSelected");
		voidBlueSelected = (Material)Resources.Load ("Materials/VoidBlueSelected");
		redOwnedBlueSelected = (Material)Resources.Load ("Materials/RedOwnedBlueSelected");
		redOwnedRedSelected = (Material)Resources.Load ("Materials/RedOwnedRedSelected");
		blueOwnedBlueSelected = (Material)Resources.Load ("Materials/BlueOwnedBlueSelected");
		blueOwnedRedSelected = (Material)Resources.Load ("Materials/BlueOwnedRedSelected");
		blueMat = (Material)Resources.Load ("Materials/BlueTileMat");
		redMat = (Material)Resources.Load ("Materials/RedTileMat");
	}
}
