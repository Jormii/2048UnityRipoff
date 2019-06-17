using UnityEngine;

/// <summary>
/// Class that stores the values and prefabs used to instantiate the grid.
/// Prefabs must be provided.
/// </summary>
[System.Serializable]
public class GridProperties {
    /// <summary>
    /// N x N dimensions of the grid.
    /// </summary>
    public int gridLength = 4;

    /// <summary>
    /// How many tiles will be instantiated at the beginning of the game.
    /// </summary>
    public int initialTiles = 2;

    /// <summary>
    /// The separation between tiles and squares.
    /// </summary>
    public float squareOffset = 0.1f;

    /// <summary>
    /// The chance a tile with value 2 is spawned each time the player moves the
    /// tiles.
    /// </summary>
    public double tile2SpawnChance = 0.7;

    /// <summary>
    /// A prefab used as squares of the grid.
    /// </summary>
    public GameObject squarePrefab;

    /// <summary>
    /// A prefab of a tile with value 2.
    /// </summary>
    public GameObject tile2Prefab;

    /// <summary>
    /// A prefab of a tile with value 2.
    /// </summary>
    public GameObject tile4Prefab;
}

public class GameController : MonoBehaviour {
    /// <summary>
    /// Properties of the grid.
    /// </summary>
    public GridProperties gridProperties = new GridProperties ();

    /// <summary>
    /// The GameObject in the scene that will display the player's score.
    /// </summary>
    public ScoreUI scoreUI;

    /// <summary>
    /// The GameObject that will pop-up when no moves are available. Initially
    /// inactive.
    /// </summary>
    public GameObject gameOverGameObject;

    /// <summary>
    /// An object that manages the grid and the tiles in it.
    /// </summary>
    private Grid2048 grid;

    /// <summary>
    /// Stores the state of the game previous movement. Used to undo movements.
    /// Undo function is available once every move. A null value indicates no
    /// snapshot is available.
    /// </summary>
    private Snapshot snapshot;

    /// <summary>
    /// Used to calculate if a 2 or 4 valued tile is spawned.
    /// </summary>
    private System.Random rng = new System.Random ();

    /// <summary>
    /// On GameObject start, the Grid2048 is created and initial tiles are spawned.
    /// </summary>
    void Start () {
        gameOverGameObject.SetActive (false);

        // (x, y) camera's position is considered the center of the grid.
        Vector2 gridOrigin = Camera.main.transform.position;
        Camera.main.orthographicSize = gridProperties.gridLength + 1; // Camera size is adjusted so tiles are not off frame.
        grid = new Grid2048 (gridProperties.gridLength, gridProperties.squareOffset,
            gridOrigin, gridProperties.squarePrefab);

        for (int i = 0; i < gridProperties.initialTiles; ++i) {
            SpawnTile ();
        }
    }

    /// <summary>
    /// Every frame, the game checks if the grid has movements available. In this 
    /// case, tiles are moved if the player entered any input. Either case, game
    /// over GameObject is activated and input is ignored.
    /// </summary>
    void Update () {
        if (!grid.MovementsAvailable ()) {
            gameOverGameObject.SetActive (true);
            return;
        }

        // If the player inputs any direction, the game state is saved and the
        // tiles moved. If no changes were made in the grid, nothing happens.
        Enums.Direction inputDirection = GetPlayerInput ();
        if (inputDirection != Enums.Direction.None) {
            grid.ResetGrid ();
            TakeSnapshot ();
            if (grid.MoveTiles (inputDirection)) {
                SpawnTile ();
            }
        }
    }

    /// <summary>
    /// Checks if any of the keys binded to the movement are pressed. In case the
    /// player presses any of the keys, a <see cref="Enums.Direction"/> value is
    /// returned. If not <see cref="Enums.Direction.None"/> is returned.
    /// </summary>
    /// <returns>The player input, represented with the <see cref="Enums.Direction"/>
    /// enum.</returns>
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

    /// <summary>
    /// Spawns a tile in the grid.
    /// </summary>
    private void SpawnTile () {
        GameObject tilePrefab;
        if (rng.NextDouble () < gridProperties.tile2SpawnChance) {
            tilePrefab = gridProperties.tile2Prefab;
        } else {
            tilePrefab = gridProperties.tile4Prefab;
        }

        grid.SpawnTile (tilePrefab);
    }

    /// <summary>
    /// Increases the player score.
    /// </summary>
    /// <param name="amount">Amount to increase the score.</param>
    public void IncrementScore (int amount) {
        scoreUI.IncrementScore (amount);
    }

    /// <summary>
    /// Stores the current state of the game in the snapshot value.
    /// </summary>
    private void TakeSnapshot () {
        snapshot = new Snapshot (grid, scoreUI.Score);
    }

    /// <summary>
    /// Function called when the Undo button in the UI is pressed. Restores the
    /// state of the game to that stores in the snapshot, if exists.
    /// </summary>
    public void Undo () {
        // No snapshot exists if this equals to null
        if (snapshot != null) {
            grid.Undo (snapshot);
            scoreUI.Undo (snapshot);
            snapshot = null;
        }
    }

    /// <summary>
    /// Function called when the Restart button in the UI is pressed. Restores
    /// the state of the game to its initial state.
    /// </summary>
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