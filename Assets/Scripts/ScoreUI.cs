using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour {
    private int score = 0;
    private TextMeshProUGUI scoreText;
    private readonly string baseText = "Score: ";

    void Start () {
        scoreText = GetComponent<TextMeshProUGUI> ();
        scoreText.text = FormatScore (this);
    }

    public void IncrementScore (int amount) {
        score += amount;
        scoreText.text = FormatScore (this);
    }

    public void Undo (Snapshot snapshot) {
        score = snapshot.Score;
        scoreText.text = FormatScore (this);
    }

    public void Restart () {
        score = 0;
        scoreText.text = FormatScore (this);
    }

    public string FormatScore (ScoreUI scoreUI) {
        return string.Format ("{0}{1}", baseText, score);
    }

    /*
    Properties
     */

    public int Score {
        get => score;
        set => score = value;
    }
}