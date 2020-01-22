using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreWindow : MonoBehaviour {

    private static ScoreWindow instance;

    private Text scoreText;

    private void Awake() {
        instance = this;
        scoreText = transform.Find("scoreText").GetComponent<Text>();

        Score.OnHighScoreChanged += Score_OnHighScoreChanged;
        UpdateDisplayedHighscore();
    }

    private void Update() {
        scoreText.text = Score.GetScore().ToString();
    }

    private void Score_OnHighScoreChanged(object sender, System.EventArgs e) {
        UpdateDisplayedHighscore();
    }

    private void UpdateDisplayedHighscore() {
        int highscore = Score.GetHighscore();
        transform.Find("highScoreText").GetComponent<Text>().text =
            "HIGHSCORE\n" + highscore.ToString();
    }

    public static void HideStatic() {
        instance.gameObject.SetActive(false);
    }
}
