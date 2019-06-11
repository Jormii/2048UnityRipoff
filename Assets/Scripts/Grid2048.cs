using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Grid2048 {

    private int gridLength;
    private Vector3 origin;
    private Vector2 tile2DSize;
    private float tileOffset;
    private Dictionary<Vector2Int, Tile> tiles;
    private List<Vector2Int> freeSquares;
    private System.Random rng;
    private GameObject boardGameObject;
    private GameObject squaresContainer;
    private GameObject tilesContainer;
    private readonly Dictionary<Enums.Direction, Vector2Int> directionVectors = new Dictionary<Enums.Direction, Vector2Int> { { Enums.Direction.Up, Vector2Int.up },
        { Enums.Direction.Down, Vector2Int.down },
        { Enums.Direction.Left, Vector2Int.left },
        { Enums.Direction.Right, Vector2Int.right }
    };

    public Grid2048 (int gridLength) {
        this.gridLength = gridLength;

        int gridLengthSquare = gridLength * gridLength;
        tiles = new Dictionary<Vector2Int, Tile> (gridLengthSquare);
        freeSquares = new List<Vector2Int> (gridLengthSquare);
        rng = new System.Random ();
        
        boardGameObject = new GameObject ("Board");
        squaresContainer = new GameObject ("Squares");
        squaresContainer.transform.parent = boardGameObject.transform;
        tilesContainer = new GameObject ("Tiles");
        tilesContainer.transform.parent = boardGameObject.transform;
    }

    public void InitializeGrid (Vector2 origin, float offset, Vector3 tileSize, GameObject squarePrefab) {
        this.origin = origin;
        this.tile2DSize = new Vector2 (tileSize.x, tileSize.z) / 5.0f;
        this.tileOffset = offset;

        for (int x = 0; x < gridLength; ++x) {
            for (int y = 0; y < gridLength; ++y) {
                Vector2 position = CalculatePosition (x, y);
                freeSquares.Add (new Vector2Int (x, y));

                Vector3 position3D = new Vector3 (position.x, position.y, 0.1f);
                Quaternion rotation = squarePrefab.transform.rotation;
                Transform parent = squaresContainer.transform;
                GameObject.Instantiate (squarePrefab, position3D, rotation, parent);
            }
        }
    }

    private Vector2 CalculatePosition (int x, int y) {
        return new Vector2 (
            x * (origin.x + tile2DSize.x + tileOffset),
            y * (origin.y + tile2DSize.y + tileOffset));
    }

    public void ResetGrid () {
        foreach (Tile t in tiles.Values) {
            t.Reset ();
        }
    }

    public bool MoveTiles (Enums.Direction direction) {
        if (direction == Enums.Direction.None) {
            return false;
        }

        bool gridChanged = false;

        List<Tile> sortedTiles = new List<Tile> (tiles.Values);
        switch (direction) {
            case Enums.Direction.Up:
            case Enums.Direction.Down:
                sortedTiles.Sort ((Tile t1, Tile t2) => sortVertically (t1, t2));
                break;
            case Enums.Direction.Left:
            case Enums.Direction.Right:
                sortedTiles.Sort ((Tile t1, Tile t2) => sortHorizontally (t1, t2));
                break;
        }

        if (direction == Enums.Direction.Down || direction == Enums.Direction.Left) {
            sortedTiles.Reverse ();
        }

        foreach (Tile t in sortedTiles) {
            Vector2Int nextSquare = DisplaceTile (t, direction);
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

    private Vector2Int DisplaceTile (Tile tile, Enums.Direction direction) {
        Vector2Int directionVector = directionVectors[direction];
        Vector2Int nextSquare = tile.coordinatesInGrid + directionVector;

        for (; CoordinateBelongsToGrid (nextSquare); nextSquare += directionVector) {
            if (tiles.ContainsKey (nextSquare)) {
                Tile tileInNextSquare = tiles[nextSquare];
                if (tile.value == tileInNextSquare.value) {
                    MergeTiles (tileInNextSquare, tile, directionVector);
                    return tileInNextSquare.coordinatesInGrid;
                } else {
                    MoveTile (tile, nextSquare - directionVector);
                    return nextSquare;
                }
            }
        }

        nextSquare -= directionVector;
        MoveTile (tile, nextSquare);
        return nextSquare;
    }

    private bool CoordinateBelongsToGrid (Vector2Int coordinate) {
        return (coordinate.x >= 0 && coordinate.x < gridLength) && (coordinate.y >= 0 && coordinate.y < gridLength);
    }

    private void MergeTiles (Tile tileToMerge, Tile tileToRemove, Vector2Int direction) {
        if (tileToMerge.Merge ()) {
            tiles.Remove (tileToRemove.coordinatesInGrid);
            freeSquares.Add (tileToRemove.coordinatesInGrid);
        } else {
            Vector2Int newTileToRemoveCoordinates = tileToMerge.coordinatesInGrid - direction;
            MoveTile (tileToRemove, newTileToRemoveCoordinates);
        }

    }

    private void MoveTile (Tile tile, Vector2Int newCoordinates) {
        tiles.Remove (tile.coordinatesInGrid);

        tiles.Remove (tile.coordinatesInGrid);
        tiles.Add (newCoordinates, tile);

        freeSquares.Add (tile.coordinatesInGrid);
        freeSquares.Remove (newCoordinates);

        tile.Move (newCoordinates.x, newCoordinates.y);
    }

    public void SpawnTile (GameObject tilePrefab) {
        Vector2Int freeCoordinate = GetFreePosition ();

        Vector2 position = CalculatePosition (freeCoordinate.x, freeCoordinate.y);
        Quaternion rotation = tilePrefab.transform.rotation;
        GameObject newTile = GameObject.Instantiate (tilePrefab, position, rotation, tilesContainer.transform);
        Tile tileComponent = newTile.GetComponent<Tile> ();

        tiles.Add (freeCoordinate, tileComponent);
        freeSquares.Remove (freeCoordinate);
    }

    private Vector2Int GetFreePosition () {
        return freeSquares[rng.Next (freeSquares.Count)];
    }

    public bool NoMovesPossible () {
        // TODO
        return false;
    }

}