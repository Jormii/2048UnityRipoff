using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileColors {

    private const int MAX_VALUE = 2048;
    private static readonly Color postMaxValueColor = Color.black;
    private static readonly Dictionary<int, Color> colors = new Dictionary<int, Color> { { 2, Color.yellow },
        { 4, Color.blue },
        { 8, Color.cyan },
        { 16, Color.green },
        { 32, Color.magenta },
        { 64, Color.red },
        { 128, Color.clear },
        { 256, Color.gray },
        { 512, Color.grey },
        { 1024, Color.black },
        { 2048, Color.black }
    };

    public static Color GetTileColor (int tileValue) {
        if (tileValue > MAX_VALUE) {
            return postMaxValueColor;
        }

        return colors[tileValue];
    }
}