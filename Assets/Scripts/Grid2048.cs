using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Grid2048 {

    private int gridLength;
    private Dictionary<Vector2, Vector2Int> gridCoordinates;
    private Dictionary<Vector2Int, Vector2> coordinatesPositions;
    private Dictionary<Vector2, Tile> tiles;
    private List<Vector2> freeSquares;
    private System.Random rng;
    private readonly Dictionary<Enums.Direction, Vector2Int> directionVectors = new Dictionary<Enums.Direction, Vector2Int> { { Enums.Direction.Up, Vector2Int.up },
        { Enums.Direction.Down, Vector2Int.down },
        { Enums.Direction.Left, Vector2Int.left },
        { Enums.Direction.Right, Vector2Int.right }
    };

    public Grid2048 (int gridLength) {
        this.gridLength = gridLength;

        int gridLengthSquare = gridLength * gridLength;
        gridCoordinates = new Dictionary<Vector2, Vector2Int> (gridLengthSquare);
        coordinatesPositions = new Dictionary<Vector2Int, Vector2> (gridLengthSquare);
        tiles = new Dictionary<Vector2, Tile> (gridLengthSquare);
        freeSquares = new List<Vector2> (gridLengthSquare);
        rng = new System.Random ();
    }

    public void InitializeGrid (Vector2 origin, float offset, Vector3 tileSize, GameObject squarePrefab) {
        Vector3 tile2DSize = new Vector3 (tileSize.x, tileSize.z, 0.0f) / 5.0f;

        for (int x = 0; x < gridLength; ++x) {
            for (int y = 0; y < gridLength; ++y) {
                Vector2 position = new Vector2 (
                    x * (origin.x + tile2DSize.x + offset),
                    y * (origin.y + tile2DSize.y + offset));

                gridCoordinates.Add (position, new Vector2Int (x, y));
                coordinatesPositions.Add (new Vector2Int (x, y), position);
                freeSquares.Add (position);
                GameObject.Instantiate (squarePrefab, new Vector3 (position.x, position.y, 0.1f), squarePrefab.transform.rotation);
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

        foreach (Tile t in tilesInGrid) {
            Debug.LogWarning ("CURRENT TILES ON BOARD");
            foreach (KeyValuePair<Vector2, Tile> entry in tiles) {
                Debug.LogFormat ("{0}/{1}", gridCoordinates[entry.Key], entry.Value.value);
            }

            Debug.LogWarning ("CURRENT FREE SQUARES");
            foreach (Vector2 freeSquare in freeSquares) {
                Debug.LogFormat ("{0}", gridCoordinates[freeSquare]);
            }

            Vector2Int currentSquare = gridCoordinates[new Vector2 (t.transform.position.x, t.transform.position.y)];
            Vector2Int nextSquare = MoveTile (t, direction);

            if (!currentSquare.Equals (nextSquare)) {
                gridChanged = true;
            }
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

    private Vector2Int MoveTile (Tile tile, Enums.Direction direction) {
        Vector2 tilePosition = new Vector2 (tile.transform.position.x, tile.transform.position.y);

        Vector2Int directionVector = directionVectors[direction];
        Vector2Int nextSquare = gridCoordinates[tilePosition] + directionVector;

        Debug.LogFormat ("Moving tile at square {0}. Direction: {1}", nextSquare - directionVector, direction.ToString ());

        while (coordinatesPositions.ContainsKey (nextSquare)) {
            Debug.LogFormat ("Checking square {0}", nextSquare);

            Vector2 nextSquarePosition = coordinatesPositions[nextSquare];

            if (tiles.ContainsKey (nextSquarePosition)) {
                Debug.LogFormat ("Said square contains another tile");

                Tile tileInNextSquare = tiles[nextSquarePosition];
                if (tile.value == tileInNextSquare.value) {
                    Debug.LogFormat ("That tile has the same value. Merging");
                    tileInNextSquare.Merge ();
                    tiles.Remove (tilePosition);
                    GameObject.Destroy (tile.gameObject);

                    return nextSquare;
                } else {
                    Debug.LogFormat ("That square has a tile with a different value");
                    nextSquare -= directionVector;
                    Vector2 newTilePosition = coordinatesPositions[nextSquare];
                    tiles.Remove (tilePosition);
                    tiles.Add (newTilePosition, tile);
                    tile.transform.position = newTilePosition;
                    freeSquares.Remove (newTilePosition);
                    freeSquares.Add (tilePosition);

                    return nextSquare;
                }
            }

            nextSquare += directionVector;
        }

        nextSquare -= directionVector;
        Vector2 newPosition = coordinatesPositions[nextSquare];
        tiles.Remove (tilePosition);
        tiles.Add (newPosition, tile);
        tile.transform.position = newPosition;
        freeSquares.Remove (newPosition);
        freeSquares.Add (tilePosition);

        Debug.LogFormat ("Tile ran into a wall. New square: {0}", nextSquare);
        return nextSquare;
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