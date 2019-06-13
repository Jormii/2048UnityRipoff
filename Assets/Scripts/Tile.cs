using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour {

    public int initialValue;

    private int realValue;
    private bool mergedThisTurn;
    private Vector2Int coordinatesInGrid = new Vector2Int ();
    private TextMeshPro tileText;
    private Material tileMaterial;
    private GameController gameController;

    private void Start () {
        realValue = initialValue;
        tileText = GetComponentInChildren<TextMeshPro> ();
        tileMaterial = GetComponent<Renderer> ().material;
        gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
        UpdateTile ();
    }

    public void ResetTile () {
        mergedThisTurn = false;
    }

    public void Move (Vector2Int newCoordinates, Vector2 newPosition) {
        coordinatesInGrid.Set (newCoordinates.x, newCoordinates.y);
        Vector3 newPosition3D = new Vector3 (newPosition.x, newPosition.y, transform.position.z);
        transform.position = newPosition3D;
    }

    public bool MergeTile () {
        if (mergedThisTurn) {
            return false;
        }

        realValue += realValue;
        gameController.IncrementScore (realValue);
        UpdateTile ();
        return true;
    }

    private void UpdateTile () {
        tileText.text = realValue.ToString ();
        tileMaterial.color = TileColors.GetTileColor (realValue);
        mergedThisTurn = true;
    }

    public static int SortHorizontally (Tile t1, Tile t2) {
        Vector3 t1Pos = t1.transform.position;
        Vector3 t2Pos = t2.transform.position;

        if (t1Pos.x > t2Pos.x) {
            return -1;
        } else if (t1Pos.x < t2Pos.x) {
            return 1;
        }
        return 0;
    }

    public static int SortVertically (Tile t1, Tile t2) {
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
        return string.Format ("Tile at coordinates {0}, with value {1}", coordinatesInGrid, realValue);
    }

    /*
     * Properties
     */

    public int Value {
        get => realValue;
    }

    public Vector2Int Coordinates {
        get => new Vector2Int (coordinatesInGrid.x, coordinatesInGrid.y);
    }

}