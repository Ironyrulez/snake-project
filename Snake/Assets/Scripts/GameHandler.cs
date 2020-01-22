using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour {

    private static GameHandler instance;

    [SerializeField] private Snake snake;
    private LevelGrid levelGrid;
    private bool paused;

    private void Awake() {
        instance = this;
        paused = false;
        Score.InitScore();
    }

    private void Start() {
        Debug.Log("GameHandler.Start");

        levelGrid = new LevelGrid(20, 20);

        snake.Setup(levelGrid);
        levelGrid.Setup(snake);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (paused) {
                ResumeGame();
            } else {
                PauseGame();
            }
        }
    }

    public static void SnakeDied() {
        bool isNewHighScore = Score.TrySetNewHighscore();
        GameOverWindow.ShowStatic(isNewHighScore);
        ScoreWindow.HideStatic();
    }

    public static void PauseGame() {
        PausedWindow.ShowStatic();
        Time.timeScale = 0f;    // Modifier effecting Time.deltaTime
        instance.paused = true;
    }

    public static void ResumeGame() {
        PausedWindow.HideStatic();
        Time.timeScale = 1f;    // Modifier effecting Time.deltaTime
        instance.paused = false;
    }
}
