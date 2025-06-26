using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuManager : MonoBehaviour {
    public GameObject optionsPanel;
    public GameObject playPanel;
    public GameObject howToPlayPanel;
    public TMP_InputField joinRoomField;

    private void Start() {
        Global.Initialize();
        DisableAllPanels();
    }

    public void Quit() {
        Application.Quit();
        Debug.Log("Quiting..");
    }

    public void Options() {
        DisableAllPanels();
        optionsPanel.SetActive(true);
    }

    public void Play() {
        DisableAllPanels();
        playPanel.SetActive(true);
    }

    public void HostGame() {
        string roomCode = GenerateRoomNumber();

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;

        PhotonNetwork.CreateRoom(roomCode, roomOptions, null);
    }

    public void JoinGame() {
        Debug.Log("Joining room...");
        PhotonNetwork.JoinRoom(joinRoomField.text);
    }

    public void HowToPlay() {
        DisableAllPanels();
        howToPlayPanel.SetActive(true);
    }

    private static string GenerateRoomNumber() {
        string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string code = "";
        for (int i = 0; i < 10; i++) code += characters[Random.Range(0, characters.Length)];
        return code;
    }

    public void Close(GameObject panel) {
        panel.SetActive(false);
    }

    public void DisableAllPanels() {
        optionsPanel.SetActive(false);
        playPanel.SetActive(false);
        howToPlayPanel.SetActive(false);
    }
}
