using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Stage : NetworkBehaviour {

    public GameObject TilePrefab;
	int dimensions = 10;
    public Tile[,] tiles;
    public Material voidmat;

    private SyncListInt changedTiles = new SyncListInt(); // testing sending array across network
    //public TileType[] changedTiles = new TileType[100]; // testing sending array across network

    public void Awake()
    {
        Debug.Log("Stage awake");
        TilePrefab = (GameObject)Resources.Load("Prefabs/TilePrefab");

        tiles = new Tile[dimensions, dimensions];
    }

    public void SpawnTiles()
    {
        Debug.Log("initializing stage");

        // create a dimensions x dimensions 2d array of Tiles
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                GameObject tile = Instantiate(TilePrefab, new Vector3(i * 1, 0, j * 1), Quaternion.identity);
                

                Tile tileData = tile.GetComponent<Tile>();
                tileData.X = i;
                tileData.Y = j;
                tiles[i, j] = tileData;

                NetworkServer.Spawn(tile);

                changedTiles.Add((i + 1) * (j + 1));

                // not being synced on client
                tile.GetComponent<Renderer>().material = voidmat;
                tile.name = "tile_" + i + "_" + j;
                tile.transform.parent = this.gameObject.transform;
            }
        }
        Debug.Log("Made a New " + dimensions + "x" + dimensions + " Stage.");
    }

    public string PrintListLength()
    {
        return changedTiles.Count.ToString();
    }

    public Tile GetTileAt(int x, int y) {
		return tiles [x, y]; // return the tile at the given co-ordinates
	}

	public int Dimensions {
		get { return dimensions; }
	}
}


/* non-networked, data model implementation
 * 
	public Stage (int dimensions) {
		this.dimensions = dimensions;
		tiles = new Tile[dimensions, dimensions]; // create a new 2D array for tiles

		// create a dimensions x dimensions 2d array of Tiles
		for (int i = 0; i < dimensions; i++) {
			for (int j = 0; j < dimensions; j++) {
				tiles [i, j] = new Tile (i, j, TileType.Void);
			}
		}
		Debug.Log ("Made a New " + dimensions + "x" + dimensions + " Stage.");
	}
*/
