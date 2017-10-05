using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameView : NetworkBehaviour
{
    AssetData assets;
    public GameController gameController;
    public Stage stage;
    public Player p1, p2;

    void Start()
    {
        gameController = (GameController)GameObject.FindObjectOfType(typeof(GameController));
        assets = (AssetData)GameObject.FindObjectOfType(typeof(AssetData));
    }


    [ClientRpc]
    public void RpcUpdateTileAppearance()
    {
        Debug.Log("rpc client updating tile appearance");

        Tile[] allTiles = GameObject.FindObjectsOfType<Tile>();
        Debug.Log("GameView found lots of tiles : # = " + allTiles.Length);

        foreach (Tile tile in allTiles)
        {
            switch (tile.Type)
            {
                case TileType.Void:
                    tile.Renderer.material = assets.voidMat;
                    break;
                case TileType.Player1:
                    switch (tile.condition)
                    {
                        case TileCondition.Normal:
                            tile.Renderer.material = assets.blueMat;
                            break;
                        case TileCondition.Damaged:
                            tile.Renderer.material = assets.blueDamaged;
                            break;
                    }
                    break;
                case TileType.Player2:
                    switch (tile.condition)
                    {
                        case TileCondition.Normal:
                            tile.Renderer.material = assets.redMat;
                            break;
                        case TileCondition.Damaged:
                            tile.Renderer.material = assets.redDamaged;
                            break;
                    }
                    break;
            }
        }

        /*
         
        foreach (Tile tile in Tile.changedCondition)
        {
            switch (tile.Condition)
            {
                case TileCondition.Damaged:
                    switch (tile.Type)
                    {
                        case TileType.Player1:
                            tile.Renderer.material = assets.blueDamaged;
                            break;
                        case TileType.Player2:
                            tile.Renderer.material = assets.redDamaged;
                            break;
                    }
                    break;
                case TileCondition.Normal:
                    switch (tile.Type)
                    {
                        case TileType.Player1:
                            tile.Renderer.material = assets.blueMat;
                            break;
                        case TileType.Player2:
                            tile.Renderer.material = assets.redMat;
                            break;
                    }
                    break;
            }
        }

        
         
        Debug.Log("p1 x = " + p1.X + " p1 y = " + p1.Y);
        Debug.Log("p2 x = " + p2.X + " p2 y = " + p2.Y);

        Tile P1SelectedTile = stage.tiles[p1.X, p1.Y];
        Tile P2SelectedTile = stage.tiles[p2.X, p2.Y];
        Debug.Log("numer of tiles (or columns ? ) in stage is : " + stage.tiles.Length);

        if (P1SelectedTile.Type == TileType.Void)
        {
            P1SelectedTile.Renderer.material = assets.voidBlueSelected;
        }
        else if (P1SelectedTile.Type == TileType.Player1)
        {
            if (P1SelectedTile.Condition == TileCondition.Normal)
                P1SelectedTile.Renderer.material = assets.blueOwnedBlueSelected;
            if (P1SelectedTile.Condition == TileCondition.Damaged)
                P1SelectedTile.Renderer.material = assets.blueOnBlueDamaged;
        }
        else if (P1SelectedTile.Type == TileType.Player2)
        {
            if (P1SelectedTile.Condition == TileCondition.Normal)
                P1SelectedTile.Renderer.material = assets.redOwnedBlueSelected;
            if (P1SelectedTile.Condition == TileCondition.Damaged)
                P1SelectedTile.Renderer.material = assets.blueOnRedDamaged;
        }


        if (P2SelectedTile.Type == TileType.Void)
        {
            P2SelectedTile.Renderer.material = assets.voidRedSelected;
        }
        else if (P2SelectedTile.Type == TileType.Player1)
        {
            if (P2SelectedTile.Condition == TileCondition.Normal)
                P2SelectedTile.Renderer.material = assets.blueOwnedRedSelected;
            if (P2SelectedTile.Condition == TileCondition.Damaged)
                P2SelectedTile.Renderer.material = assets.redOnBlueDamaged;
        }
        else if (P2SelectedTile.Type == TileType.Player2)
        {
            if (P2SelectedTile.Condition == TileCondition.Normal)
                P2SelectedTile.Renderer.material = assets.redOwnedRedSelected;
            if (P2SelectedTile.Condition == TileCondition.Damaged)
                P2SelectedTile.Renderer.material = assets.redOnRedDamaged;
        }

        Tile.changedTiles.Clear();
        Tile.changedCondition.Clear();

    */
    }
}













/* old 
 * 
 * 
[ClientRpc]
public void RpcUpdateTileAppearance()
{
    Debug.Log("rpc client updating tile appearance");

    foreach (Tile tile in Tile.changedTiles)
    {
        switch (tile.Type)
        {
            case TileType.Void:
                tile.Renderer.material = assets.voidMat;
                break;
            case TileType.Player1:
                switch (tile.Condition)
                {
                    case TileCondition.Normal:
                        tile.Renderer.material = assets.blueMat;
                        break;
                    case TileCondition.Damaged:
                        tile.Renderer.material = assets.blueDamaged;
                        break;
                }
                break;
            case TileType.Player2:
                switch (tile.Condition)
                {
                    case TileCondition.Normal:
                        tile.Renderer.material = assets.redMat;
                        break;
                    case TileCondition.Damaged:
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
            case TileCondition.Damaged:
                switch (tile.Type)
                {
                    case TileType.Player1:
                        tile.Renderer.material = assets.blueDamaged;
                        break;
                    case TileType.Player2:
                        tile.Renderer.material = assets.redDamaged;
                        break;
                }
                break;
            case TileCondition.Normal:
                switch (tile.Type)
                {
                    case TileType.Player1:
                        tile.Renderer.material = assets.blueMat;
                        break;
                    case TileType.Player2:
                        tile.Renderer.material = assets.redMat;
                        break;
                }
                break;
        }
    }

    Debug.Log("p1 x = " + p1.X + " p1 y = " + p1.Y);
    Debug.Log("p2 x = " + p2.X + " p2 y = " + p2.Y);

    Tile P1SelectedTile = stage.tiles[p1.X, p1.Y];
    Tile P2SelectedTile = stage.tiles[p2.X, p2.Y];
    Debug.Log("numer of tiles (or columns ? ) in stage is : " + stage.tiles.Length);

    if (P1SelectedTile.Type == TileType.Void)
    {
        P1SelectedTile.Renderer.material = assets.voidBlueSelected;
    }
    else if (P1SelectedTile.Type == TileType.Player1)
    {
        if (P1SelectedTile.Condition == TileCondition.Normal)
            P1SelectedTile.Renderer.material = assets.blueOwnedBlueSelected;
        if (P1SelectedTile.Condition == TileCondition.Damaged)
            P1SelectedTile.Renderer.material = assets.blueOnBlueDamaged;
    }
    else if (P1SelectedTile.Type == TileType.Player2)
    {
        if (P1SelectedTile.Condition == TileCondition.Normal)
            P1SelectedTile.Renderer.material = assets.redOwnedBlueSelected;
        if (P1SelectedTile.Condition == TileCondition.Damaged)
            P1SelectedTile.Renderer.material = assets.blueOnRedDamaged;
    }


    if (P2SelectedTile.Type == TileType.Void)
    {
        P2SelectedTile.Renderer.material = assets.voidRedSelected;
    }
    else if (P2SelectedTile.Type == TileType.Player1)
    {
        if (P2SelectedTile.Condition == TileCondition.Normal)
            P2SelectedTile.Renderer.material = assets.blueOwnedRedSelected;
        if (P2SelectedTile.Condition == TileCondition.Damaged)
            P2SelectedTile.Renderer.material = assets.redOnBlueDamaged;
    }
    else if (P2SelectedTile.Type == TileType.Player2)
    {
        if (P2SelectedTile.Condition == TileCondition.Normal)
            P2SelectedTile.Renderer.material = assets.redOwnedRedSelected;
        if (P2SelectedTile.Condition == TileCondition.Damaged)
            P2SelectedTile.Renderer.material = assets.redOnRedDamaged;
    }

    Tile.changedTiles.Clear();
    Tile.changedCondition.Clear();
}
}

*/
