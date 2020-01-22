using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader {

    public enum Scene {
        GameScene,
        Loading,
        MainMenu
    }

    private static Action loaderCallbackAction;

    public static void Load(Scene scene) {
        // Set up callback action that will trigger once Loading scene is loaded
        loaderCallbackAction = () => {
            // Load target scene once Loading scene is loaded
            SceneManager.LoadScene(scene.ToString());
        };

        // Load the Loading scene
        SceneManager.LoadScene(Scene.Loading.ToString());
    }

    public static void LoaderCallback() {
        if (loaderCallbackAction != null) {
            loaderCallbackAction();
            loaderCallbackAction = null;
        }
    }
}
