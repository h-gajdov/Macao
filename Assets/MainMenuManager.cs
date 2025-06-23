using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour {
    public GameObject optionsPanel;

    private void Start() {
        optionsPanel.SetActive(false);
    }

    public void Quit() {
        Application.Quit();
        Debug.Log("Quiting..");
    }

    public void Options() {
        optionsPanel.SetActive(true);
    }
}
