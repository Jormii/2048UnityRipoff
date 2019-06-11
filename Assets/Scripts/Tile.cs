using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour {
    public int value;

    private TextMeshPro tileText;
    private Material tileMaterial;

    void Start () {
        tileText = GetComponentInChildren<TextMeshPro> ();
        tileMaterial = GetComponent<Renderer> ().material;
        UpdateTile ();
    }

    public void Merge () {
        value += value;
        UpdateTile ();
    }

    private void UpdateTile () {
        tileText.text = value.ToString ();
        tileMaterial.color = TileColors.GetTileColor (value);
    }

    public override string ToString () {
        return string.Format ("Tile at position {0}, with value {1}", new Vector2 (transform.position.x, transform.position.y), value);
    }

}