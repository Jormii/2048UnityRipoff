using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snapshot {

    private Dictionary<Vector2Int, Tile> tiles;
    private List<Vector2Int> freeSquares;
    private int score;

    private Snapshot (Grid2048 grid, int score) {
        this.tiles = new Dictionary<Vector2Int, Tile> (grid.Tiles);
        this.freeSquares = new List<Vector2Int> (grid.FreeSquares);
        this.score = score;
    }

    public static Snapshot TakeSnapshot (Grid2048 grid, int score) {
        Snapshot snapshot = new Snapshot (grid, score);

        foreach (Tile t in snapshot.tiles.Values) {
            t.gameObject.SetActive (false);
        }

        return snapshot;
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