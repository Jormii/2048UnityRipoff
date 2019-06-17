using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interface : MonoBehaviour {

    private GameController gameController;

    private void Start () {
        gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
    }

    public void Undo () {
        gameController.Undo ();
    }

    public void Restart () {
        gameController.Restart ();
    }

}