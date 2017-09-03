using System.Collections;
using UnityEngine;

public class Player {

    public int playerNumber;
    public int selectedX;
    public int selectedY;
    public Runner runner;
    public int startingRow;

    float tileCooldown = 3.0f;
    bool canPlay = true; // can play a tile (it's not on cooldown)

    public Player(int playerNumber, int dimensions) {

        this.playerNumber = playerNumber;

        if (playerNumber == 1)
        {
            this.selectedX = 0;
            this.selectedY = 0;
            startingRow = 0;
        }
        else if (playerNumber == 2)
        {
            this.selectedX = dimensions - 1;
            this.selectedY = dimensions - 1;
            startingRow = dimensions - 1;
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

    public Runner constructRunner(int x, int y) {
        Runner = new Runner(x, y, playerNumber);
        return runner;
    }

    public Runner constructRunner(Vector2 spawnPosition)
    {
        Runner = new Runner((int)spawnPosition.x, (int)spawnPosition.y, playerNumber);
        return runner;
    }
}
