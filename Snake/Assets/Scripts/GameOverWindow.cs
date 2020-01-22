using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class GameOverWindow : MonoBehaviour {

    private static GameOverWindow instance;

    private void Awake() {
        instance = this;

        transform.Find("restartBtn").GetComponent<Button_UI>().ClickFunc = () => {
            Loader.Load(Loader.Scene.GameScene);
        };
        transform.Find("restartBtn").GetComponent<Button_UI>().AddButtonSounds();

        Hide(); // Game Over window is hidden by default
    }

    private void Show(bool isNewHighScore) {
        gameObject.SetActive(true);

        // Show congratulatory text iff new highscore
        transform.Find("congratulatoryText").gameObject.SetActive(isNewHighScore);

        // Show old highscore iff highscore not beaten
        transform.Find("currHighScoreText").gameObject.SetActive(!isNewHighScore);
        if (!isNewHighScore)
            transform.Find("currHighScoreText").GetComponent<Text>().text =
                "HIGHSCORE: " + Score.GetHighscore().ToString();

        transform.Find("currScoreText").GetComponent<Text>().text = Score.GetScore().ToString();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    public static void ShowStatic(bool isNewHighScore) {
        instance.Show(isNewHighScore);
    }
}
