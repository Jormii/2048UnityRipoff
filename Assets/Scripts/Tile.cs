using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour {
    public int value;

    private TextMeshPro tileText;

    void Start () {
        tileText = GetComponentInChildren<TextMeshPro> ();
        tileText.text = value.ToString ();
    }

    public override string ToString () {
        return string.Format ("Tile at position {0}, with value {1}", new Vector2 (transform.position.x, transform.position.y), value);
    }

}