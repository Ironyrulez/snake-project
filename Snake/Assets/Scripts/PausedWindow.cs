using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class PausedWindow : MonoBehaviour {

    private static PausedWindow instance;

    private void Awake() {
        instance = this;

        transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        transform.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

        transform.Find("resumeBtn").GetComponent<Button_UI>().ClickFunc = 
            () => GameHandler.ResumeGame();
        transform.Find("resumeBtn").GetComponent<Button_UI>().AddButtonSounds();

        transform.Find("returnBtn").GetComponent<Button_UI>().ClickFunc =
            () => {
                Time.timeScale = 1f;    // Reset time scale
                Loader.Load(Loader.Scene.MainMenu);
            };
        transform.Find("returnBtn").GetComponent<Button_UI>().AddButtonSounds();

        Hide(); // Paused window is hidden by default
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    public static void ShowStatic() {
        instance.Show();
    }

    public static void HideStatic() {
        instance.Hide();
    }
}
