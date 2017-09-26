using System.Collections;
using UnityEngine;

public class Player {

    Player opponent;
    public int playerNumber;
    public int X;
    public int Y;
    public Runner runner;
    public int startingRow;
    float energy;

    public Player(int playerNumber, int dimensions) {

        this.playerNumber = playerNumber;

        if (playerNumber == 1)
        {
            this.X = 0;
            this.Y = 0;
            startingRow = 0;
        }
        else if (playerNumber == 2)
        {
            this.X = dimensions - 1;
            this.Y = dimensions - 1;
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
                if (this.Y < stage.Dimensions - 1) this.Y++;
                break;
            case Direction.down:
                if (this.Y > 0) this.Y--;
                break;
            case Direction.left:
                if (this.X > 0) this.X--;
                break;
            case Direction.right:
                if (this.X < stage.Dimensions - 1) this.X++;
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
