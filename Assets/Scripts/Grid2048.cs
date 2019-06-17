using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid2048 {

    private int gridLength;
    private Vector2 gridOrigin;
    private float squareOffset;
    private Dictionary<Vector2Int, Tile> tiles;
    private List<Vector2Int> freeSquares;

    private System.Random rng;
    private GameObject squarePrefab;
    private GameObject boardGameObject;
    private GameObject squaresContainer;
    private GameObject tilesContainer;
    private readonly Dictionary<Enums.Direction, Vector2Int> directionVectors = new Dictionary<Enums.Direction, Vector2Int> { { Enums.Direction.Up, Vector2Int.up },
        { Enums.Direction.Down, Vector2Int.down },
        { Enums.Direction.Left, Vector2Int.left },
        { Enums.Direction.Right, Vector2Int.right }
    };

    public Grid2048 (int gridLength, float squareOffset, Vector2 gridOrigin, GameObject squarePrefab) {
        this.gridLength = gridLength;
        this.squareOffset = squareOffset;
        this.gridOrigin = gridOrigin;
        this.squarePrefab = squarePrefab;

        int gridLengthSquare = gridLength * gridLength;
        this.tiles = new Dictionary<Vector2Int, Tile> (gridLengthSquare);
        this.freeSquares = new List<Vector2Int> (gridLengthSquare);
        this.rng = new System.Random ();

        // The following GameObjects are created in order to maintain a structure in the scene tree.
        this.boardGameObject = new GameObject ("Board");
        this.squaresContainer = new GameObject ("Squares");
        this.squaresContainer.transform.parent = boardGameObject.transform;
        this.tilesContainer = new GameObject ("Tiles");
        this.tilesContainer.transform.parent = boardGameObject.transform;

        InitializeGrid ();
    }

    private void InitializeGrid () {
        for (int x = 0; x < gridLength; ++x) {
            for (int y = 0; y < gridLength; ++y) {
                Vector2Int coordinates = new Vector2Int (x, y);
                Vector2 position = CalculatePosition (coordinates);
                freeSquares.Add (coordinates);

                Vector3 position3D = new Vector3 (position.x, position.y, 0.1f);
                Quaternion rotation = squarePrefab.transform.rotation;
                Transform parent = squaresContainer.transform;
                GameObject.Instantiate (squarePrefab, position3D, rotation, parent);
            }
        }
    }

    private Vector2 CalculatePosition (Vector2Int coordinates) {
        return new Vector2 (
            CalculateXPosition (coordinates.x),
            CalculateYPosition (coordinates.y));
    }

    private float CalculateXPosition (int x) {
        float weight = (float) x / (float) (gridLength - 1);
        float origin = gridOrigin.x;
        int length = gridLength;
        float size = squarePrefab.GetComponent<Renderer> ().bounds.size.x;

        return CalculatePositionSquare (weight, origin, length, size);
    }

    private float CalculateYPosition (int h) {
        float weight = (float) h / (float) (gridLength - 1);
        float origin = gridOrigin.y;
        int length = gridLength;
        float size = squarePrefab.GetComponent<Renderer> ().bounds.size.y;

        return CalculatePositionSquare (weight, origin, length, size);
    }

    private float CalculatePositionSquare (float weight, float origin, int length, float size) {
        float constant = ((length - 1) / 2.0f) * (size + squareOffset);
        return (1 - weight) * (origin - constant) + weight * (origin + constant);
    }

    public void ResetGrid () {
        foreach (Tile t in tiles.Values) {
            t.ResetTile ();
        }
    }

    public bool MoveTiles (Enums.Direction direction) {
        return MoveTiles (direction, true);
    }

    private bool MoveTiles (Enums.Direction direction, bool moveActually) {
        if (direction == Enums.Direction.None) {
            return false;
        }

        bool gridChanged = false;

        List<Tile> sortedTiles = new List<Tile> (tiles.Values);
        switch (direction) {
            case Enums.Direction.Up:
            case Enums.Direction.Down:
                sortedTiles.Sort ((Tile t1, Tile t2) => Tile.SortVertically (t1, t2));
                break;
            case Enums.Direction.Left:
            case Enums.Direction.Right:
                sortedTiles.Sort ((Tile t1, Tile t2) => Tile.SortHorizontally (t1, t2));
                break;
        }

        if (direction == Enums.Direction.Down || direction == Enums.Direction.Left) {
            sortedTiles.Reverse ();
        }

        foreach (Tile t in sortedTiles) {
            Vector2Int currentSquare = t.Coordinates;
            Vector2Int nextSquare = DisplaceTile (t, direction, moveActually);

            if (!currentSquare.Equals (nextSquare)) {
                gridChanged = true;
            }
        }

        return gridChanged;
    }

    private Vector2Int DisplaceTile (Tile tile, Enums.Direction direction, bool moveActually) {
        Vector2Int directionVector = directionVectors[direction];
        Vector2Int nextSquare = tile.Coordinates + directionVector;

        for (; CoordinateBelongsToGrid (nextSquare); nextSquare += directionVector) {
            if (tiles.ContainsKey (nextSquare)) {
                Tile tileInNextSquare = tiles[nextSquare];
                if (tile.Value == tileInNextSquare.Value) {
                    if (moveActually) {
                        MergeTiles (tileInNextSquare, tile, directionVector);
                    }
                    return tileInNextSquare.Coordinates;
                } else {
                    break;
                }
            }
        }

        nextSquare -= directionVector;
        if (moveActually) {
            MoveTile (tile, nextSquare);
        }
        return nextSquare;
    }

    private bool CoordinateBelongsToGrid (Vector2Int coordinate) {
        return (coordinate.x >= 0 && coordinate.x < gridLength) && (coordinate.y >= 0 && coordinate.y < gridLength);
    }

    private void MergeTiles (Tile tileToMerge, Tile tileToRemove, Vector2Int direction) {
        if (tileToMerge.MergeTile ()) {
            tiles.Remove (tileToRemove.Coordinates);
            freeSquares.Add (tileToRemove.Coordinates);
            GameObject.Destroy (tileToRemove.gameObject);
        } else {
            Vector2Int newTileToRemoveCoordinates = tileToMerge.Coordinates - direction;
            MoveTile (tileToRemove, newTileToRemoveCoordinates);
        }

    }

    private void MoveTile (Tile tile, Vector2Int newCoordinates) {
        if (tile.Coordinates.Equals (newCoordinates)) {
            return;
        }

        tiles.Remove (tile.Coordinates);

        tiles.Remove (tile.Coordinates);
        tiles.Add (newCoordinates, tile);

        freeSquares.Add (tile.Coordinates);
        freeSquares.Remove (newCoordinates);

        Vector2 newPosition = CalculatePosition (newCoordinates);
        tile.Move (newCoordinates, newPosition);
    }

    public void SpawnTile (GameObject tilePrefab) {
        Vector2Int freeCoordinate = GetFreePosition ();

        Vector2 position = CalculatePosition (freeCoordinate);
        Quaternion rotation = tilePrefab.transform.rotation;
        GameObject newTile = GameObject.Instantiate (tilePrefab, position, rotation, tilesContainer.transform);
        Tile tileComponent = newTile.GetComponent<Tile> ();

        tileComponent.Move (freeCoordinate, position);
        tiles.Add (freeCoordinate, tileComponent);
        freeSquares.Remove (freeCoordinate);
    }

    private Vector2Int GetFreePosition () {
        return freeSquares[rng.Next (freeSquares.Count)];
    }

    public bool MovementsAvailable () {
        return MoveTiles (Enums.Direction.Up, false) || MoveTiles (Enums.Direction.Right, false) ||
            MoveTiles (Enums.Direction.Down, false) || MoveTiles (Enums.Direction.Left, false);
    }

    public void Undo (Snapshot snapshot) {
        foreach (Tile t in tiles.Values) {
            GameObject.Destroy (t.gameObject);
        }

        tiles = new Dictionary<Vector2Int, Tile> (snapshot.Tiles);
        foreach (Tile t in tiles.Values) {
            t.gameObject.SetActive (true);
            t.gameObject.transform.parent = tilesContainer.transform;
        }
        freeSquares = new List<Vector2Int> (snapshot.FreeSquares);
    }

    public void Restart () {
        foreach (KeyValuePair<Vector2Int, Tile> entry in tiles) {
            GameObject.Destroy (entry.Value.gameObject);
            freeSquares.Add (entry.Key);
        }

        tiles.Clear ();
    }

    /*
    Properties
     */

    public int GridLength {
        get => gridLength;
    }

    public Dictionary<Vector2Int, Tile> Tiles {
        get => tiles;
    }

    public List<Vector2Int> FreeSquares {
        get => freeSquares;
    }

}