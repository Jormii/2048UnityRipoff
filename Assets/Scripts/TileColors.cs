using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileColors {

    private const int MAX_VALUE = 2048;
    private static readonly Color postMaxValueColor = NewColor (60.0f, 58.0f, 50.0f);
    private static readonly Dictionary<int, Color> colors = new Dictionary<int, Color> { { 2, NewColor (238.0f, 228.0f, 218.0f) },
        { 4, NewColor (237.0f, 224.0f, 200.0f) },
        { 8, NewColor (242.0f, 177.0f, 121.0f) },
        { 16, NewColor (245.0f, 149.0f, 99.0f) },
        { 32, NewColor (246.0f, 124.0f, 95.0f) },
        { 64, NewColor (246.0f, 94.0f, 59.0f) },
        { 128, NewColor (237.0f, 207.0f, 114.0f) },
        { 256, NewColor (237.0f, 204.0f, 97.0f) },
        { 512, NewColor (237.0f, 200.0f, 80.0f) },
        { 1024, NewColor (237.0f, 197.0f, 63.0f) },
        { 2048, NewColor (237.0f, 194.0f, 46.0f) }
    };

    public static Color GetTileColor (int tileValue) {
        return (KeyExistsInColorMap (tileValue)) ? colors[tileValue] : postMaxValueColor;
    }

    private static bool KeyExistsInColorMap (int tileValue) {
        return tileValue < MAX_VALUE;
    }

    private static Color NewColor (float r, float g, float b) {
        Vector3 rgb = new Vector3 (r, g, b) / 255.0f;
        return new Color (rgb.x, rgb.y, rgb.z, 1.0f);
    }
}