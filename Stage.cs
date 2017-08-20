using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage {
    // TO DO: change STAGE class to static ? Will require lots of changes in GameController

	public Tile[,] tiles;
	int dimensions; // height/width of the tile space

	public Stage (int dimensions) {
		this.dimensions = dimensions;
		tiles = new Tile[dimensions, dimensions]; // create a new 2D array for tiles

		// create a dimensions x dimensions 2d array of Tiles
		for (int i = 0; i < dimensions; i++) {
			for (int j = 0; j < dimensions; j++) {
				tiles [i, j] = new Tile (i, j, Tile.TileType.Void);
			}
		}
		Debug.Log ("Made a New " + dimensions + "x" + dimensions + " Stage.");
	}
		
	public Tile GetTileAt(int x, int y) {
		if (tiles [x, y] == null) {
			tiles [x, y] = new Tile (x, y, Tile.TileType.Void); 
		}
		return tiles [x, y]; // return the tile at the given co-ordinates
	}

	public int Dimensions {
		get { return dimensions; }
	}
}
