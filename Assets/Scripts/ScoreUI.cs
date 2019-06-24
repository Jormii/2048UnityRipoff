using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour {
    private int score = 0;
    private TextMeshProUGUI scoreText;

    void Start () {
        scoreText = GetComponent<TextMeshProUGUI> ();
        UpdateScoreText ();
    }

    public void IncrementScore (int amount) {
        score += amount;
        UpdateScoreText ();
    }

    public void Undo (Snapshot snapshot) {
        RollbackToSnapshot (snapshot);
        UpdateScoreText ();
    }

    private void RollbackToSnapshot (Snapshot snapshot) {
        score = snapshot.Score;
        UpdateScoreText ();
    }

    public void Restart () {
        score = 0;
        UpdateScoreText ();
    }

    private void UpdateScoreText () {
        scoreText.text = string.Format ("Score: {0}", score);
    }

    /*
    Properties
     */

    public int Score {
        get => score;
        set => score = value;
    }
}