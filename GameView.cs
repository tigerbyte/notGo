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
                    tile.Renderer.material = assets.blueMat;
                    break;
                case Tile.TileType.Player2:
                    tile.Renderer.material = assets.redMat;
                    break;
            }
        }

        Tile P1SelectedTile = stage.tiles[p1.selectedX, p1.selectedY];
        Tile P2SelectedTile = stage.tiles[p2.selectedX, p2.selectedY];

        if (P1SelectedTile.Type == Tile.TileType.Void) { P1SelectedTile.Renderer.material = assets.voidBlueSelected; }
        else if (P1SelectedTile.Type == Tile.TileType.Player1) { P1SelectedTile.Renderer.material = assets.blueOwnedBlueSelected; }
        else if (P1SelectedTile.Type == Tile.TileType.Player2) { P1SelectedTile.Renderer.material = assets.redOwnedBlueSelected; }

        if (P2SelectedTile.Type == Tile.TileType.Void) { P2SelectedTile.Renderer.material = assets.voidRedSelected; }
        else if (P2SelectedTile.Type == Tile.TileType.Player1) { P2SelectedTile.Renderer.material = assets.blueOwnedRedSelected; }
        else if (P2SelectedTile.Type == Tile.TileType.Player2) { P2SelectedTile.Renderer.material = assets.redOwnedRedSelected; }

        Tile.changedTiles.Clear();
    }
}
