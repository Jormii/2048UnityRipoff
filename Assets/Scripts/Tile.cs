using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour {
    public int value;
    public bool mergedThisTurn;
    public Vector2Int coordinatesInGrid = new Vector2Int ();

    private TextMeshPro tileText;
    private Material tileMaterial;
    private GameController gameController;

    void Start () {
        tileText = GetComponentInChildren<TextMeshPro> ();
        tileMaterial = GetComponent<Renderer> ().material;
        gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
        UpdateTile ();
    }

    public void Reset () {
        mergedThisTurn = false;
    }

    public void Move (Vector2Int coordinates, Vector2 position) {
        coordinatesInGrid.Set (coordinates.x, coordinates.y);
        transform.position = new Vector3 (position.x, position.y, transform.position.z);
    }

    public bool Merge () {
        if (mergedThisTurn) {
            return false;
        }

        value += value;
        gameController.IncrementScore (value);
        UpdateTile ();
        return true;
    }

    private void UpdateTile () {
        tileText.text = value.ToString ();
        tileMaterial.color = TileColors.GetTileColor (value);
        mergedThisTurn = true;
    }

    public static int sortHorizontally (Tile t1, Tile t2) {
        Vector3 t1Pos = t1.transform.position;
        Vector3 t2Pos = t2.transform.position;

        if (t1Pos.x > t2Pos.x) {
            return -1;
        } else if (t1Pos.x < t2Pos.x) {
            return 1;
        }
        return 0;
    }

    public static int sortVertically (Tile t1, Tile t2) {
        Vector3 t1Pos = t1.transform.position;
        Vector3 t2Pos = t2.transform.position;

        if (t1Pos.y > t2Pos.y) {
            return -1;
        } else if (t1Pos.y < t2Pos.y) {
            return 1;
        }
        return 0;
    }

    public override string ToString () {
        return string.Format ("Tile at position {0}, with value {1}", new Vector2 (transform.position.x, transform.position.y), value);
    }

}