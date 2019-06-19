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
        InitializeGrid ();
    }

    // TODO
    private void InitializeGrid () {
        Camera mainCamera = Camera.main;

        Vector2 gridOrigin = mainCamera.transform.position;
        mainCamera.orthographicSize = gridProperties.gridLength + 1; // Camera size is adjusted so tiles stay in frame.
        grid = new Grid2048 (gridProperties, gridOrigin);

        SpawnTiles ();
    }

    private void SpawnTiles () {
        for (int i = 0; i < gridProperties.initialTiles; ++i) {
            SpawnTile ();
        }
    }

    private void SpawnTile () {
        grid.SpawnTileAtRandomPosition (GetTileToSpawn ());
    }

    private GameObject GetTileToSpawn () {
        return (Spawn2Tile ()) ? gridProperties.tile2Prefab : gridProperties.tile4Prefab;
    }

    private bool Spawn2Tile () {
        return rng.NextDouble () < gridProperties.tile2SpawnChance;
    }

    void Update () {
        if (GameOverStateReached ()) {
            GameOver ();
            return;
        }

        HandlePlayerInput ();
    }

    private bool GameOverStateReached () {
        return !grid.ThereAreMovementsAvailable ();
    }

    private void GameOver () {
        gameOverGameObject.SetActive (true);
    }

    private void HandlePlayerInput () {
        Enums.Direction inputDirection = GetPlayerInput ();
        if (inputDirection != Enums.Direction.None) {
            MoveTiles (inputDirection);
        }
    }

    private void MoveTiles (Enums.Direction direction) {
        MovementSetup ();
        bool gridChanged = grid.MoveTilesInGrid (direction);
        PostMovement (gridChanged);
    }

    private void MovementSetup () {
        grid.ResetGrid ();
        TakeSnapshot ();
    }

    private void PostMovement (bool gridChanged) {
        if (gridChanged) {
            SpawnTile ();
        }
    }

    private Enums.Direction GetPlayerInput () {
        if (Input.GetKeyDown (KeyCode.UpArrow)) {
            return Enums.Direction.Up;
        } else if (Input.GetKeyDown (KeyCode.DownArrow)) {
            return Enums.Direction.Down;
        } else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
            return Enums.Direction.Left;
        } else if (Input.GetKeyDown (KeyCode.RightArrow)) {
            return Enums.Direction.Right;
        }

        return Enums.Direction.None;
    }

    public void IncrementScore (int amount) {
        scoreUI.IncrementScore (amount);
    }

    private void TakeSnapshot () {
        snapshot = new Snapshot (grid, scoreUI.Score);
    }

    public void Undo () {
        if (SnapshotExists ()) {
            grid.Undo (snapshot);
            scoreUI.Undo (snapshot);
            snapshot = null;
        }
    }

    private bool SnapshotExists () {
        return snapshot != null;
    }

    public void RestartGame () {
        grid.Restart ();
        scoreUI.Restart ();
        if (SnapshotExists ()) {
            snapshot.Restart ();
        }

        SpawnTiles ();

        gameOverGameObject.SetActive (false);
        snapshot = null;
    }

}