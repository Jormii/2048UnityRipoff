using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Grid2048 {

    private int gridLength;
    private Dictionary<Vector2, Vector2Int> gridCoordinates;
    private Dictionary<Vector2, Tile> tiles;
    private List<Vector2> freeSquares;
    private System.Random rng;

    public Grid2048 (int gridLength) {
        this.gridLength = gridLength;

        int gridLengthSquare = gridLength * gridLength;
        gridCoordinates = new Dictionary<Vector2, Vector2Int> (gridLengthSquare);
        tiles = new Dictionary<Vector2, Tile> (gridLengthSquare);
        freeSquares = new List<Vector2> (gridLengthSquare);
        rng = new System.Random ();
    }

    public void InitializeGrid (Vector2 origin, float offset, Vector3 tileSize) {
        Vector3 tile2DSize = new Vector3 (tileSize.x, tileSize.z, 0.0f) / 5.0f;

        for (int x = 0; x < gridLength; ++x) {
            for (int y = 0; y < gridLength; ++y) {
                Vector2 position = new Vector2 (
                    x * (origin.x + tile2DSize.x + offset),
                    y * (origin.y + tile2DSize.y + offset));

                gridCoordinates.Add (position, new Vector2Int (x, y));
                freeSquares.Add (position);
            }
        }
    }

    public bool MoveTiles (Enums.Direction direction) {
        if (direction == Enums.Direction.None) {
            return false;
        }

        bool gridChanged = false;

        List<Tile> tilesInGrid = new List<Tile> (tiles.Values);
        switch (direction) {
            case Enums.Direction.Up:
            case Enums.Direction.Down:
                tilesInGrid.Sort ((Tile t1, Tile t2) => sortVertically (t1, t2));
                break;
            case Enums.Direction.Left:
            case Enums.Direction.Right:
                tilesInGrid.Sort ((Tile t1, Tile t2) => sortHorizontally (t1, t2));
                break;
        }

        if (direction == Enums.Direction.Down || direction == Enums.Direction.Left) {
            tilesInGrid.Reverse ();
        }

        return gridChanged;
    }

    private int sortHorizontally (Tile t1, Tile t2) {
        Vector3 t1Pos = t1.transform.position;
        Vector3 t2Pos = t2.transform.position;

        if (t1Pos.x > t2Pos.x) {
            return -1;
        } else if (t1Pos.x < t2Pos.x) {
            return 1;
        }
        return 0;
    }

    private int sortVertically (Tile t1, Tile t2) {
        Vector3 t1Pos = t1.transform.position;
        Vector3 t2Pos = t2.transform.position;

        if (t1Pos.y > t2Pos.y) {
            return -1;
        } else if (t1Pos.y < t2Pos.y) {
            return 1;
        }
        return 0;
    }

    public void AddTile (Tile tile) {
        Vector2 position = new Vector2 (tile.transform.position.x, tile.transform.position.y);
        tiles.Add (position, tile);

        if (freeSquares.Contains (position)) {
            freeSquares.Remove (position);
        }
    }

    public Vector2 GetFreePosition () {
        Vector2 position = freeSquares[rng.Next (freeSquares.Count)];
        freeSquares.Remove (position);
        return position;
    }

    public bool NoMovesPossible () {
        return false;
    }

}