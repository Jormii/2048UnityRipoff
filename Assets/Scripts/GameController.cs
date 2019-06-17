using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class GridProperties {
    public int gridLength = 4;
    public int initialTiles = 2;
    public float squareOffset = 0.1f;
    public double tile2SpawnChance = 0.7;
    public GameObject squarePrefab;
    public GameObject tile2Prefab;
    public GameObject tile4Prefab;
}

public class GameController : MonoBehaviour {

    public GridProperties gridProperties = new GridProperties ();
    public ScoreUI scoreUI;
    public GameObject gameOverGameObject;

    private Grid2048 grid;
    private Snapshot snapshot;
    private System.Random rng = new System.Random ();

    void Start () {
        gameOverGameObject.SetActive (false);

        Vector2 gridOrigin = Camera.main.transform.position;
        Camera.main.orthographicSize = gridProperties.gridLength + 1;
        grid = new Grid2048 (gridProperties.gridLength, gridProperties.squareOffset, gridOrigin, gridProperties.squarePrefab);

        for (int i = 0; i < gridProperties.initialTiles; ++i) {
            SpawnTile ();
        }
    }

    void Update () {
        if (!grid.MovementsAvailable ()) {
            gameOverGameObject.SetActive (true);
            GameObject.Destroy (gameObject); // Problema ?
        }

        Enums.Direction inputDirection = GetPlayerInput ();
        if (inputDirection != Enums.Direction.None) {
            grid.ResetGrid ();
            TakeSnapshot ();
            if (grid.MoveTiles (inputDirection)) {
                SpawnTile ();
            }
        }
    }

    private Enums.Direction GetPlayerInput () {
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

        return inputDirection;
    }

    private void SpawnTile () {
        GameObject tilePrefab = (rng.NextDouble () < gridProperties.tile2SpawnChance) ? gridProperties.tile2Prefab : gridProperties.tile4Prefab;
        grid.SpawnTile (tilePrefab);
    }

    public void IncrementScore (int amount) {
        scoreUI.IncrementScore (amount);
    }

    private void TakeSnapshot () {
        snapshot = new Snapshot (grid, scoreUI.Score);
    }

    public void Undo () {
        if (snapshot != null) {
            grid.Undo (snapshot);
            scoreUI.Undo (snapshot);
            snapshot = null;
        }
    }

    public void Restart () {
        grid.Restart ();
        scoreUI.Restart ();
        snapshot.Restart ();

        for (int i = 0; i < gridProperties.initialTiles; ++i) {
            SpawnTile ();
        }

        gameOverGameObject.SetActive (false);
        snapshot = null;
    }

}