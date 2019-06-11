using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour {

    public int gridLength = 4;
    public int initialTiles = 2;
    public float offsetBetweenTiles = 0.1f;
    public double tile2SpawnChance = 0.8f;
    public GameObject squarePrefab;
    public GameObject tile2Prefab;
    public GameObject tile4Prefab;

    private Grid2048 grid;
    private System.Random rng = new System.Random ();

    void Start () {
        grid = new Grid2048 (gridLength);
        grid.InitializeGrid (transform.position, offsetBetweenTiles, tile2Prefab.GetComponent<MeshFilter> ().sharedMesh.bounds.size, squarePrefab);

        for (int i = 0; i < initialTiles; ++i) {
            SpawnTile ();
        }
    }

    void Update () {
        if (grid.NoMovesPossible ()) {
            Debug.LogWarning ("Game Over");
            return;
        }

        Enums.Direction inputDirection = Enums.Direction.None;

        if (Input.GetKeyDown (KeyCode.UpArrow)) {
            inputDirection = Enums.Direction.Up;
        } else if (Input.GetKeyDown (KeyCode.DownArrow)) {
            inputDirection = Enums.Direction.Down;
        } else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
            inputDirection = Enums.Direction.Left;
        } else if (Input.GetKeyDown (KeyCode.RightArrow)) {
            inputDirection = Enums.Direction.Right;
        }

        if (inputDirection != Enums.Direction.None) {
            bool moved = grid.MoveTiles (inputDirection);
            if (moved) {
                SpawnTile ();
            }
        }
    }

    private void SpawnTile () {
        Vector2 position = grid.GetFreePosition ();

        GameObject tilePrefab = (rng.NextDouble () < tile2SpawnChance) ? tile2Prefab : tile4Prefab;
        GameObject newTile = Instantiate (tilePrefab, position, tilePrefab.transform.rotation, transform);
        grid.AddTile (newTile.GetComponent<Tile> ());
    }

}