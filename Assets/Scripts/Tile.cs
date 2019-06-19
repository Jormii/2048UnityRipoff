using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour {

    public int initialValue;

    private int realValue;
    private bool mergedThisTurn;
    private Vector2Int coordinateInGrid = new Vector2Int ();
    private TextMeshPro tileText;
    private Material tileMaterial;
    private GameController gameController;

    private void Start () {
        if (TileWasJustCreated ()) {
            realValue = initialValue;
        }

        tileText = GetComponentInChildren<TextMeshPro> ();
        tileMaterial = GetComponent<Renderer> ().material;
        gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
        UpdateTile ();
    }

    private bool TileWasJustCreated () {
        return realValue == 0;
    }

    public void ResetTile () {
        mergedThisTurn = false;
    }

    public void Move (Vector2Int newCoordinate, Vector2 newPosition) {
        coordinateInGrid.Set (newCoordinate.x, newCoordinate.y);
        Vector3 newPosition3D = new Vector3 (newPosition.x, newPosition.y, transform.position.z);
        transform.position = newPosition3D;
    }

    public void MergeTile () {
        realValue += realValue;
        gameController.IncrementScore (realValue);
        UpdateTile ();
    }

    private void UpdateTile () {
        tileText.text = realValue.ToString ();
        tileMaterial.color = TileColors.GetTileColor (realValue);
        name = string.Format ("{0} = {1}", coordinateInGrid, realValue);
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

    public GameObject Clone () {
        GameObject clone = GameObject.Instantiate (gameObject);
        Tile tileComponent = clone.GetComponent<Tile> ();

        tileComponent.realValue = realValue;
        tileComponent.mergedThisTurn = mergedThisTurn;
        tileComponent.coordinateInGrid = new Vector2Int (coordinateInGrid.x, coordinateInGrid.y);

        return clone;
    }

    public override string ToString () {
        return string.Format ("Tile at coordinate {0}, with value {1}", coordinateInGrid, realValue);
    }

    /*
     * Properties
     */

    public int Value {
        get => realValue;
    }

    public Vector2Int Coordinate {
        get => new Vector2Int (coordinateInGrid.x, coordinateInGrid.y);
    }

    public bool MergedThisTurn {
        get => mergedThisTurn;
    }

}