using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class MainMenuWindow : MonoBehaviour {

    private enum Submenu {
        Main,
        HowToPlay
    }

    private static MainMenuWindow instance;

    private void Awake() {
        instance = this;

        // Centre submenues in canvas
        transform.Find("howToPlaySubmenu").GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        transform.Find("mainSubmenu").GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        // Set up buttons
        transform.Find("mainSubmenu").Find("playBtn").GetComponent<Button_UI>().ClickFunc =
            () => Loader.Load(Loader.Scene.GameScene);
        transform.Find("mainSubmenu").Find("playBtn").GetComponent<Button_UI>().AddButtonSounds();

        transform.Find("mainSubmenu").Find("quitBtn").GetComponent<Button_UI>().ClickFunc =
            () => Application.Quit();
        transform.Find("mainSubmenu").Find("quitBtn").GetComponent<Button_UI>().AddButtonSounds();

        transform.Find("mainSubmenu").Find("howToPlayBtn").GetComponent<Button_UI>().ClickFunc =
            () => ShowSubmenu(Submenu.HowToPlay);
        transform.Find("mainSubmenu").Find("howToPlayBtn").GetComponent<Button_UI>().AddButtonSounds();

        // Show main submenu by default
        ShowSubmenu(Submenu.Main);
    }

    private void ShowSubmenu(Submenu sub) {
        transform.Find("mainSubmenu").gameObject.SetActive(false);
        transform.Find("howToPlaySubmenu").gameObject.SetActive(false);

        switch (sub) {
            case Submenu.Main:
                transform.Find("mainSubmenu").gameObject.SetActive(true);
                break;
            case Submenu.HowToPlay:
                transform.Find("howToPlaySubmenu").gameObject.SetActive(true);
                break;
        }
    }

    public static void ShowMainMenuStatic() {
        instance.ShowSubmenu(Submenu.Main);
    }
}
