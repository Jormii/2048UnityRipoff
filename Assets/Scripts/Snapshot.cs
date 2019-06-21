using System.Collections.Generic;
using UnityEngine;

public class Snapshot {

    private Dictionary<Vector2Int, Tile> tiles;
    private List<Vector2Int> freeSquares;
    private int score;
    private static GameObject snapshotContainer;

    public Snapshot (Grid2048 grid, int score) {
        this.tiles = new Dictionary<Vector2Int, Tile> (grid.GridLength);
        this.freeSquares = new List<Vector2Int> (grid.FreeSquares);
        this.score = score;

        DestroySnapshotTiles ();
        SnapshotTileGameObjects (grid);
    }

    private void SnapshotTileGameObjects (Grid2048 grid) {
        foreach (KeyValuePair<Vector2Int, Tile> entry in grid.Tiles) {
            GameObject clonedGameObject = entry.Value.Clone ();

            clonedGameObject.SetActive (false);
            clonedGameObject.transform.parent = snapshotContainer.transform;
            Tile tileComponent = clonedGameObject.GetComponent<Tile> ();
            tiles.Add (entry.Key, tileComponent);
        }
    }

    private static void DestroySnapshotTiles () {
        if (snapshotContainer == null) {
            snapshotContainer = new GameObject ("SnapshotContainer");
        }

        for (int c = 0; c < snapshotContainer.transform.childCount; ++c) {
            GameObject child = snapshotContainer.transform.GetChild (c).gameObject;
            GameObject.Destroy (child);
        }
    }

    public void Restart () {
        DestroySnapshotTiles ();
    }

    /*
    Properties
     */

    public Dictionary<Vector2Int, Tile> Tiles {
        get => tiles;
    }

    public List<Vector2Int> FreeSquares {
        get => freeSquares;
    }

    public int Score {
        get => score;
    }
}