using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuHandler : MonoBehaviour {

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            MainMenuWindow.ShowMainMenuStatic();
        }
    }
}
