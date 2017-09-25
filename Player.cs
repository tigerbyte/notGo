using System.Collections;
using UnityEngine;

public class Player {

    Player opponent;
    public int playerNumber;
    public int selectedX;
    public int selectedY;
    public Runner runner;
    public int startingRow;
    float energy;

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

    public float Energy
    {
        get { return this.energy; }
        set { this.energy = value; }
    }

    public void ReduceEnergy(float amount)
    {
        Energy = Energy - amount;
    }
    
    public Tile.TileType TileType {
        get
        {
            if (playerNumber == 1) { return Tile.TileType.Player1; }
            else { return Tile.TileType.Player2; }
        }
    }

    public void Move(Stage stage, Direction direction)
    {
        switch (direction)
        {
            case Direction.up:
                if (this.selectedY < stage.Dimensions - 1) this.selectedY++;
                break;
            case Direction.down:
                if (this.selectedY > 0) this.selectedY--;
                break;
            case Direction.left:
                if (this.selectedX > 0) this.selectedX--;
                break;
            case Direction.right:
                if (this.selectedX < stage.Dimensions - 1) this.selectedX++;
                break;
        }
    }

    public Player Opponent
    {
        get { return opponent; }
        set { opponent = value; }
    }

    public Runner Runner {
        get { return runner; }
        set { runner = value;  }
    }

    public Runner constructRunner(int x, int y) {
        Runner = new Runner(x, y, this);
        return runner;
    }

    public Runner constructRunner(Vector2 spawnPosition)
    {
        Runner = new Runner((int)spawnPosition.x, (int)spawnPosition.y, this);
        return runner;
    }
}
