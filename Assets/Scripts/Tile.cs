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

    void Start () {
        tileText = GetComponentInChildren<TextMeshPro> ();
        tileMaterial = GetComponent<Renderer> ().material;
        UpdateTile ();
    }

    public void Reset () {
        mergedThisTurn = false;
    }

    public void Move (int x, int y) {
        coordinatesInGrid.Set (x, y);
        transform.position = new Vector3 (x, y, transform.position.z);
    }

    public bool Merge () {
        if (mergedThisTurn) {
            return false;
        }

        value += value;
        UpdateTile ();
        return true;
    }

    private void UpdateTile () {
        tileText.text = value.ToString ();
        tileMaterial.color = TileColors.GetTileColor (value);
        mergedThisTurn = true;
    }

    public override string ToString () {
        return string.Format ("Tile at position {0}, with value {1}", new Vector2 (transform.position.x, transform.position.y), value);
    }

}