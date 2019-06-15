using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoButton : MonoBehaviour {

    private GameController gameController;

    private void Start () {
        gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
    }

    public void Undo () {
        gameController.Undo ();
    }

}