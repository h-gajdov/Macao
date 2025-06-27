using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGameManager : MonoBehaviour {
    public GameObject pausePanel;

    private void Start() {
        pausePanel.SetActive(false);
    }

    public void Resume() {
        pausePanel.SetActive(false);
    }

    public void ReturnToMenu() {
        PhotonNetwork.LeaveRoom();
    }

    private void Update() {
        if (!GameManager.GameHasStarted) return;

        if (Input.GetKeyDown(KeyCode.Escape)) pausePanel.SetActive(!pausePanel.activeInHierarchy);
    }
}
