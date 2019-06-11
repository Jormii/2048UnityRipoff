using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    public int value;

    private TextMeshPro tileText;

    void Start()
    {
        tileText = GetComponentInChildren<TextMeshPro>();
        tileText.text = value.ToString();
    }
}
