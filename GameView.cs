using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameView : ScriptableObject
{
    AssetData assets;
    GameController gameController;
    Stage stage;
    Player p1, p2;

    void Awake()
    {
        gameController = (GameController)GameObject.FindObjectOfType(typeof(GameController));
        assets = gameController.assets;
        stage = gameController.stage;
        p1 = gameController.p1;
        p2 = gameController.p2;
    }

    // to do : find a less ridiculous way to apply visual effects 
    public void UpdateTileAppearance()
    {
        foreach (Tile tile in Tile.changedTiles)
        {
            switch (tile.Type)
            {
                case Tile.TileType.Void:
                    tile.Renderer.material = assets.voidMat;
                    break;
                case Tile.TileType.Player1:
                    switch (tile.Condition)
                    {
                        case Tile.TileCondition.Normal:
                            tile.Renderer.material = assets.blueMat;
                            break;
                        case Tile.TileCondition.Damaged:
                            tile.Renderer.material = assets.blueDamaged;
                            break;
                    }
                    break;
                case Tile.TileType.Player2:
                    switch (tile.Condition)
                    {
                        case Tile.TileCondition.Normal:
                            tile.Renderer.material = assets.redMat;
                            break;
                        case Tile.TileCondition.Damaged:
                            tile.Renderer.material = assets.redDamaged;
                            break;
                    }
                    break;
            }
        }

        foreach (Tile tile in Tile.changedCondition)
        {
            switch (tile.Condition)
            {
                case Tile.TileCondition.Damaged:
                    switch (tile.Type)
                    {
                        case Tile.TileType.Player1:
                            tile.Renderer.material = assets.blueDamaged;
                            break;
                        case Tile.TileType.Player2:
                            tile.Renderer.material = assets.redDamaged;
                            break;
                    }
                    break;
                case Tile.TileCondition.Normal:
                    switch (tile.Type)
                    {
                        case Tile.TileType.Player1:
                            tile.Renderer.material = assets.blueMat;
                            break;
                        case Tile.TileType.Player2:
                            tile.Renderer.material = assets.redMat;
                            break;
                    }
                    break;
            }
        }

        Tile P1SelectedTile = stage.tiles[p1.X, p1.Y];
        Tile P2SelectedTile = stage.tiles[p2.X, p2.Y];

        if (P1SelectedTile.Type == Tile.TileType.Void)
        {
            P1SelectedTile.Renderer.material = assets.voidBlueSelected;
        }
        else if (P1SelectedTile.Type == Tile.TileType.Player1)
        {
            if (P1SelectedTile.Condition == Tile.TileCondition.Normal)
                P1SelectedTile.Renderer.material = assets.blueOwnedBlueSelected;
            if (P1SelectedTile.Condition == Tile.TileCondition.Damaged)
                P1SelectedTile.Renderer.material = assets.blueOnBlueDamaged;
        }
        else if (P1SelectedTile.Type == Tile.TileType.Player2)
        {
            if (P1SelectedTile.Condition == Tile.TileCondition.Normal)
                P1SelectedTile.Renderer.material = assets.redOwnedBlueSelected;
            if (P1SelectedTile.Condition == Tile.TileCondition.Damaged)
                P1SelectedTile.Renderer.material = assets.blueOnRedDamaged;
        }


        if (P2SelectedTile.Type == Tile.TileType.Void)
        {
            P2SelectedTile.Renderer.material = assets.voidRedSelected;
        }
        else if (P2SelectedTile.Type == Tile.TileType.Player1)
        {
            if (P2SelectedTile.Condition == Tile.TileCondition.Normal)
                P2SelectedTile.Renderer.material = assets.blueOwnedRedSelected;
            if (P2SelectedTile.Condition == Tile.TileCondition.Damaged)
                P2SelectedTile.Renderer.material = assets.redOnBlueDamaged;
        }
        else if (P2SelectedTile.Type == Tile.TileType.Player2)
        {
            if (P2SelectedTile.Condition == Tile.TileCondition.Normal)
                P2SelectedTile.Renderer.material = assets.redOwnedRedSelected;
            if (P2SelectedTile.Condition == Tile.TileCondition.Damaged)
                P2SelectedTile.Renderer.material = assets.redOnRedDamaged;
        }

        Tile.changedTiles.Clear();
        Tile.changedCondition.Clear();
    }
}
