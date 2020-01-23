using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Score {

    public static event EventHandler OnHighScoreChanged;
    private static int score;

    public static void InitScore() {
        score = 0;
        OnHighScoreChanged = null;
    }

    public static int GetScore() {
        return score;
    }

    public static void IncrementScore() {
        score += 1;
    }

    public static int GetHighscore() {
        // Get saved highscore from playerprefs, return 0 if null
        return PlayerPrefs.GetInt("highscore", 0);
    }

    // Test if current score beats the highscore, if so - overwrite
    public static bool TrySetNewHighscore() {
        int highscore = GetHighscore();
        if (score > highscore) {
            PlayerPrefs.SetInt("highscore", score);
            PlayerPrefs.Save();
            if (OnHighScoreChanged != null)
                OnHighScoreChanged(null, EventArgs.Empty);
            return true;
        }
        else {
            return false;
        }
    }
}
