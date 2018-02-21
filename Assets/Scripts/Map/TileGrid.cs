using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[System.Serializable]
public struct TileInfo
{
    public TileBase tile;
    public Tilemap layer;
    public int orderLayer;

}
/// <summary>
/// This scripts creates a 2D array of the top tiles across all Tilemaps in the child's component.
/// <para>Place this script on a GameObject with a Grid component.</para>
/// <remarks>When working with multiple tilemaps use the same layer and the order in layer to sort.</remarks>
/// </summary>
[RequireComponent(typeof(Grid))]
public class TileGrid : MonoBehaviour
{
    public int sizeX = 20;
    public int sizeY = 20;
    public TileInfo[,] tiles;
    // Use this for initialization
    void Awake()
    {

    }

    private void OnValidate()
    {
        sizeX = MathOperations.ClampMin(sizeX, 1);
        sizeY = MathOperations.ClampMin(sizeY, 1);
    }

    public void CreateTileMap()
    {
        tiles = new TileInfo[sizeX, sizeY];
        foreach (Tilemap t in GetComponentsInChildren<Tilemap>())
        {
            BoundsInt bounds = new BoundsInt(Vector3Int.zero, new Vector3Int(sizeX, sizeY, 1));
            TileBase[] allTiles = t.GetTilesBlock(bounds);
            TilemapRenderer tr = t.GetComponent<TilemapRenderer>();

            for (int x = 0; x < bounds.size.x; x++)
            {
                for (int y = 0; y < bounds.size.y; y++)
                {
                    TileBase tile = allTiles[x + y * bounds.size.x];
                    if (tile != null)
                    {
                        if (tiles[x, y].tile == null)
                        {
                            tiles[x, y].tile = tile;
                            tiles[x, y].orderLayer = tr.sortingOrder;
                            tiles[x, y].layer = t;
                        }
                        else if (tr != null)
                        {
                            if (tr.sortingOrder > tiles[x, y].orderLayer)
                            {
                                tiles[x, y].tile = tile;
                                tiles[x, y].orderLayer = tr.sortingOrder;
                                tiles[x, y].layer = t;
                            }
                        }
                    }

                }

            }

        }
    }

    /// <summary>
    /// Removes a tile from coordinate and finds the new top tile.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>Returns the removed tile.</returns>
    public TileBase RemoveTile(int x, int y)
    {
        if (tiles != null ? tiles.GetLength(0) <= 0 || tiles.GetLength(1) <= 0 : true)
            return null;

        if (!ValidCoordinate(x, y))
            return null;
        if (tiles[x, y].tile == null)
            return null;

        TileBase removed = tiles[x, y].tile;
        tiles[x, y].tile = null;
        tiles[x, y].layer.SetTile(new Vector3Int(x, y, 0), null);
        tiles[x, y].layer = null;
        tiles[x, y].orderLayer = 0;

        foreach (Tilemap t in GetComponentsInChildren<Tilemap>())
        {
            BoundsInt bounds = new BoundsInt(Vector3Int.zero, new Vector3Int(sizeX, sizeY, 1));
            TileBase[] allTiles = t.GetTilesBlock(bounds);
            TilemapRenderer tr = t.GetComponent<TilemapRenderer>();
            TileBase tile = allTiles[x + y * bounds.size.x];
            if (tile != null)
            {
                if (tiles[x, y].tile == null)
                {
                    tiles[x, y].tile = tile;
                    tiles[x, y].orderLayer = tr.sortingOrder;
                    tiles[x, y].layer = t;
                }
                else if (tr.sortingOrder > tiles[x, y].orderLayer)
                {
                    tiles[x, y].tile = tile;
                    tiles[x, y].orderLayer = tr.sortingOrder;
                    tiles[x, y].layer = t;
                }
            }
        }
        return removed;
    }

    /// <summary>
    /// Adds a tile to a coordinate.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void AddTile(TileBase tile, int x, int y)
    {
        if (tiles != null ? tiles.GetLength(0) > 0 && tiles.GetLength(1) > 0 : true)
            return;
        if (!ValidCoordinate(x, y))
            return;

        tiles[x, y].layer.SetTile(new Vector3Int(x, y, 0), tile);
        tiles[x, y].tile = tile;
    }

    public bool ValidCoordinate(int x, int y)
    {
        if (tiles == null)
            return false;
        if (x < 0 || x >= tiles.GetLength(0))
            return false;
        if (y < 0 || y >= tiles.GetLength(1))
            return false;

        return true;
    }
#if UNITY_EDITOR
    public void PrintGrid()
    {
        if (tiles == null)
            return;
        string result = "";
        for (int y = tiles.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < tiles.GetLength(0); x++)
            {

                result += tiles[x, y].tile.name[0];
                if (x != tiles.GetLength(0) - 1)
                    result += "|";
                else
                    result += "\n";
            }
        }
        print(result);
    }
#endif
}

