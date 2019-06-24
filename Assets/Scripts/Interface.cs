using UnityEngine;
using UnityEngine.SceneManagement;

public class Interface : MonoBehaviour {

    private GameController gameController;

    private void Start () {
        GameObject gameControllerGO = GameObject.FindGameObjectWithTag ("GameController");
        if (gameControllerGO != null) {
            gameController = gameControllerGO.GetComponent<GameController> ();
        }
    }

    public void Undo () {
        gameController.Undo ();
    }

    public void Restart () {
        gameController.RestartGame ();
    }

    public void ChangeScene (string sceneName) {
        SceneManager.LoadScene (sceneName);
    }

}