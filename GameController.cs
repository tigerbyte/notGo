using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	AudioController audioController;
    RunController runController; // [remove later] maybe
    enum direction { up, down, left, right };
	public Stage stage; // Stage contains a 2D array of tiles (Stage.tiles[x,y])
	public Player player1, player2; // players hold their selected tile co-ordinates
	public GameObject tile;
	Material voidMat, voidRedSelected, voidBlueSelected, redOwnedBlueSelected, redOwnedRedSelected, blueOwnedRedSelected, blueOwnedBlueSelected, blueMat, redMat;
   	List<Tile> updatedTiles = new List<Tile>(); // a list of tiles whose states have changed (could be static in tile class??)

	public int DIMENSIONS = 10;

	void Start () {
        LoadAssets(); // Load Prefabs/Materials from Resources folder
        audioController = (AudioController) GameObject.FindObjectOfType(typeof(AudioController));
        runController = (RunController) GameObject.FindObjectOfType(typeof(RunController));
        stage = new Stage (DIMENSIONS); // instantiate a square stage with height/width of dimensions
        player1 = new Player(1, stage.Dimensions);
        player2 = new Player(2, stage.Dimensions);
        CreateTiles (); // create tile game objects to associate with data structure
    }

    // Instantiate tile GameObjects, and assign them to the 2d Tile array data-structure 
    // that was created by Stage class . 
    // This is called once , on game startup. 
    void CreateTiles () { 
		for (int i = 0; i < stage.Dimensions; i++) {
			for (int j = 0; j < stage.Dimensions; j++) {

				// instantiate a tile gameObject for every tile in the data array
				GameObject tile_go = Instantiate (tile, new Vector3 (i * 1, 0, j * 1), Quaternion.identity);

                // give each tile a name in the inspector corresponding to its X/Y position
                tile_go.name = "tile_" + i + "_" + j;

                // give the Tile model in the 2D-array a reference to its corresponding instantiated Unity gameObject
                stage.tiles[i, j].Tile_gameObj = tile_go; 

				Tile.changedTiles.Add(stage.tiles[i, j]);
			}
		}
		UpdateTileAppearance ();
	}

	void UpdateTileAppearance () {
		foreach (Tile t in Tile.changedTiles) {
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
		
    // Update the player actions on every frame
	void Update () {
        PlayerUpdate();
	}

	// move the given player in the given direction, if their position is still within bounds of play area
	void MovePlayer(Player player, direction d) { 
		switch (d) {
		case direction.up:
			if (player.selectedY < stage.Dimensions - 1) player.selectedY++;
			break;
		case direction.down:
			if (player.selectedY > 0) player.selectedY--;
			break;
		case direction.left:
			if (player.selectedX > 0) player.selectedX--;
			break;
		case direction.right:
			if (player.selectedX < stage.Dimensions - 1) player.selectedX++;
			break;
		}
	}

	void PlayerUpdate() { 
		// to do: split controls for networking

		// player 1 controls (WASD) ... todo: refactor
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
			UpdateTileAppearance ();
		}
			
		if (Input.GetKeyDown ("space")) {
			if (stage.tiles [player1.selectedX, player1.selectedY].Type == Tile.TileType.Void) {
				CaptureTile (player1, player1.selectedX, player1.selectedY);
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
            UpdateTileAppearance ();
		}

		if (Input.GetKeyDown ("enter")) {
			if (stage.tiles [player2.selectedX, player2.selectedY].Type == Tile.TileType.Void) {
				CaptureTile (player2, player2.selectedX, player2.selectedY);
			}
        }
	}


	// call this function when player takes a tile: pass in the player Number(1/2), and co-ordinates of tile to capture
	// runnerUpdate() is called when a tile is updated
	public void CaptureTile (Player player, int x, int y) {

		audioController.PlayCaptureSoundEffect ();

		int tempX = x;
		int tempY = y;

		// collection of tiles to be switched
		List<Tile> capturedTiles = new List<Tile> (); // refreshes after searching each branch

        Tile.TileType playersTileType = player.GetTileType();
        Tile.TileType opponentTileType = Tile.TileType.Void;

        Player opponent = null;
        if (player == player1) { opponent = player2; }
        if (player == player2) { opponent = player1; }
        opponentTileType = opponent.GetTileType();

        stage.tiles [x, y].Type = playersTileType;
        
        // check right
		tempX = x;
		while (tempX < DIMENSIONS - 1 && (stage.tiles [tempX + 1, y].Type == opponentTileType)) {
			capturedTiles.Add (stage.tiles [tempX + 1, y]);
            Debug.Log("Added tile [ x:" + (tempX + 1) + ", y:" + y + "] to right branch.");

            tempX++;
			// if we have captured tiles and reach a friendly tile, convert the row (move to a bigger list)
			if ((stage.tiles[tempX + 1, y] != null) && (stage.tiles [tempX + 1, y].Type == playersTileType)) {
				AddCapturedToChanged (player, opponent, capturedTiles); 

			// if the next iteration is out of bounds or tile has no owner, clear this branch
			} else if ((tempX + 1 > DIMENSIONS) || stage.tiles [tempX + 1, y].Type == Tile.TileType.Void) {
				capturedTiles.Clear ();
                Debug.Log("Cleared capturedTiles in right branch");
                break;
            } 
		} 

		// check left 
		tempX = x;  
		while (tempX >= 1 && (stage.tiles [tempX - 1, y].Type == opponentTileType)) {
			capturedTiles.Add (stage.tiles [tempX - 1, y]);
            Debug.Log("Added tile [ x:" + (tempX - 1) + ", y:" + y + "] to right branch.");

            tempX--;
			if ((stage.tiles[tempX - 1, y] != null) && stage.tiles [tempX - 1, y].Type == playersTileType) {
				AddCapturedToChanged (player, opponent, capturedTiles);

            } else if ((tempX < 0) || stage.tiles [tempX - 1, y].Type == Tile.TileType.Void) {
				capturedTiles.Clear ();
                Debug.Log("Cleared capturedTiles in down branch");
                break;
            } 
		} 

		// check up
		tempY = y;
		while (tempY < DIMENSIONS - 1 && (stage.tiles [x, tempY + 1].Type == opponentTileType)) {
			capturedTiles.Add (stage.tiles [x, tempY + 1]);
            Debug.Log("Added tile [ x:" + tempX + ", y:" + (tempY + 1) + "] to right branch.");

            tempY++;
			if ((stage.tiles[x, tempY + 1] != null) && stage.tiles [x, tempY + 1].Type == playersTileType) {
				AddCapturedToChanged (player, opponent, capturedTiles);

            } else if ((tempY > DIMENSIONS - 1) || stage.tiles [x, tempY + 1].Type == Tile.TileType.Void) { 
				capturedTiles.Clear ();
                Debug.Log("Cleared capturedTiles in up branch");
                break;
            } 
		}

		// check down
		tempY = y;
		while (tempY >= 1 && (stage.tiles [x, tempY - 1].Type == opponentTileType)) {
			capturedTiles.Add (stage.tiles [x, tempY - 1]);
            Debug.Log("Added tile [ x:" + tempX + ", y:" + (tempY - 1) + "] to right branch.");

            tempY--;
			if ((stage.tiles[x, tempY - 1] != null) && stage.tiles [x, tempY - 1].Type == playersTileType) {
				AddCapturedToChanged (player, opponent, capturedTiles);

            } else if ((tempY < 0) || stage.tiles [x, tempY - 1].Type == Tile.TileType.Void) {
				capturedTiles.Clear ();
                Debug.Log("Cleared capturedTiles in left branch");
                break;
            } 
		}

		// update all the tiles that have been changed
		UpdateTileAppearance ();

        runController.BuildNodeMap(player);
    }
	

	// function to add the list of captured tiles generated by each branch, to a list of all tiles to be updated
	public void AddCapturedToChanged(Player player, Player opponent, List<Tile> capturedTiles) {
		// Debug.Log ("capturedTiles capacity [number of tiles to capture] is : " + capturedTiles.Count);

        if (capturedTiles.Count > 0) {
			audioController.PlaySwapSoundEffect (); 

			foreach (Tile tile in capturedTiles) {
                // Debug.Log ("this captured tile X=" + t.X + ", Y=" + t.Y);
                tile.Type = player.GetTileType();
				Tile.changedTiles.Add (tile);
			}

            // check if the opponent's runner needs to update their runPath
            runController.CheckIfPathRequiresUpdate(opponent, capturedTiles);

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
