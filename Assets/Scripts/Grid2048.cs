using System.Collections.Generic;
using UnityEngine;

public class Grid2048 {

    private int gridLength;
    private Vector2 gridOrigin;
    private float squareOffset;
    private bool checkingIfMovementsAreAvailable;
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

    public Grid2048 (GridProperties gridProperties, Vector2 gridOrigin) {
        this.gridLength = gridProperties.gridLength;
        this.squareOffset = gridProperties.squareOffset;
        this.gridOrigin = gridOrigin;
        this.squarePrefab = gridProperties.squarePrefab;

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
                Vector2Int coordinate = new Vector2Int (x, y);
                Vector2 position = CalculatePosition (coordinate);
                freeSquares.Add (coordinate);

                InstantiateSquarePrefab (position);
            }
        }
    }

    private void InstantiateSquarePrefab (Vector2 position) {
        Vector3 position3D = new Vector3 (position.x, position.y, 0.1f);
        Quaternion rotation = squarePrefab.transform.rotation;
        Transform parent = squaresContainer.transform;
        GameObject.Instantiate (squarePrefab, position3D, rotation, parent);
    }

    private Vector2 CalculatePosition (Vector2Int coordinate) {
        return new Vector2 (
            CalculateXPosition (coordinate.x),
            CalculateYPosition (coordinate.y));
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
        foreach (Tile tile in tiles.Values) {
            tile.ResetTile ();
        }
    }

    public bool MoveTilesInGrid (Enums.Direction direction) {
        checkingIfMovementsAreAvailable = false;
        return MoveTiles (direction);
    }

    private bool MoveTiles (Enums.Direction direction) {
        bool gridChanged = false;

        List<Tile> sortedTiles = SortTilesAccordingToDirection (direction);
        foreach (Tile tile in sortedTiles) {
            bool tileMoved = DisplaceTile (tile, direction);

            if (tileMoved) {
                gridChanged = true;
            }
        }

        return gridChanged;
    }

    private List<Tile> SortTilesAccordingToDirection (Enums.Direction direction) {
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

        return sortedTiles;
    }

    private bool DisplaceTile (Tile tile, Enums.Direction direction) {
        Vector2Int currentSquare = tile.Coordinate;
        Vector2Int directionVector = directionVectors[direction];
        Vector2Int nextSquare = tile.Coordinate + directionVector;

        while (CoordinateBelongsToGrid (nextSquare)) {
            if (TileExistsInSquare (nextSquare)) {
                Tile tileInNextSquare = tiles[nextSquare];

                if (TilesCanMerge (tile, tileInNextSquare)) {
                    return MergeTiles (tileInNextSquare, tile);
                } else {
                    break; // Equivalent of facing a wall
                }
            }

            nextSquare += directionVector;
        }

        nextSquare -= directionVector;
        return MoveTile (tile, nextSquare);
    }

    private bool TileExistsInSquare (Vector2Int coordinate) {
        return tiles.ContainsKey (coordinate);
    }

    private bool TilesCanMerge (Tile tile, Tile otherTile) {
        return TilesHaveTheSameValue (tile, otherTile) &&
            TileWasNotMergedThisTurn (otherTile);
    }

    private bool TileWasNotMergedThisTurn (Tile tile) {
        return !tile.MergedThisTurn;
    }

    private bool TilesHaveTheSameValue (Tile tile, Tile otherTile) {
        return tile.Value == otherTile.Value;
    }

    private bool CoordinateBelongsToGrid (Vector2Int coordinate) {
        return (coordinate.x >= 0 && coordinate.x < gridLength) && (coordinate.y >= 0 && coordinate.y < gridLength);
    }

    private bool MergeTiles (Tile tileToMerge, Tile tileToRemove) {
        if (checkingIfMovementsAreAvailable) {
            return false;
        }

        tileToMerge.MergeTile ();
        tiles.Remove (tileToRemove.Coordinate);
        freeSquares.Add (tileToRemove.Coordinate);
        GameObject.Destroy (tileToRemove.gameObject);
        return true;
    }

    private bool MoveTile (Tile tile, Vector2Int newCoordinate) {
        if (checkingIfMovementsAreAvailable) {
            return false;
        }

        if (NewCoordinateIsTilesCoordinate (tile, newCoordinate)) {
            return false;
        }

        tiles.Remove (tile.Coordinate);
        tiles.Add (newCoordinate, tile);

        freeSquares.Add (tile.Coordinate);
        freeSquares.Remove (newCoordinate);

        Vector2 newPosition = CalculatePosition (newCoordinate);
        tile.Move (newCoordinate, newPosition);

        return true;
    }

    private bool NewCoordinateIsTilesCoordinate (Tile tile, Vector2Int newCoordinate) {
        return tile.Coordinate.Equals (newCoordinate);
    }

    public void SpawnTileAtRandomPosition (GameObject tilePrefab) {
        SpawnTile (GetFreeCoordinate (), tilePrefab);
    }

    private void SpawnTile (Vector2Int coordinate, GameObject tilePrefab) {
        Vector2 position = CalculatePosition (coordinate);
        Quaternion rotation = tilePrefab.transform.rotation;
        GameObject newTile = GameObject.Instantiate (tilePrefab, position, rotation, tilesContainer.transform);

        Tile tileComponent = newTile.GetComponent<Tile> ();
        tileComponent.Move (coordinate, position);
        tiles.Add (coordinate, tileComponent);
        freeSquares.Remove (coordinate);
    }

    private Vector2Int GetFreeCoordinate () {
        return freeSquares[rng.Next (freeSquares.Count)];
    }

    public bool ThereAreMovementsAvailable () {
        checkingIfMovementsAreAvailable = true;
        return MoveTiles (Enums.Direction.Up) || MoveTiles (Enums.Direction.Right) ||
            MoveTiles (Enums.Direction.Down) || MoveTiles (Enums.Direction.Left);
    }

    public void Undo (Snapshot snapshot) {
        DestroyTileGameObjects ();
        RollbackToSnapshot (snapshot);
    }

    private void DestroyTileGameObjects () {
        foreach (Tile tile in tiles.Values) {
            GameObject.Destroy (tile.gameObject);
        }
    }

    private void RollbackToSnapshot (Snapshot snapshot) {
        tiles = new Dictionary<Vector2Int, Tile> (snapshot.Tiles);
        freeSquares = new List<Vector2Int> (snapshot.FreeSquares);

        foreach (Tile tile in tiles.Values) {
            tile.gameObject.SetActive (true);
            tile.gameObject.transform.parent = tilesContainer.transform;
        }
    }

    public void Restart () {
        DestroyTileGameObjects ();

        tiles.Clear ();
        foreach (KeyValuePair<Vector2Int, Tile> entry in tiles) {
            freeSquares.Add (entry.Key);
        }
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