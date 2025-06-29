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
    public GameObject aboutPanel;
    public GameObject errorPanel;
    public TextMeshProUGUI errorText;
    public TMP_InputField joinRoomField;

    public TMP_Dropdown numberOfDecksDropdown;
    public TMP_Dropdown timePerTurnDropdown;

    public static MainMenuManager instance;

    private void Awake() {
        if (instance == null) instance = this;
        else {
            Destroy(this);
            return;
        }
    }

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
        if (!PhotonNetwork.InLobby) {
            SetError("You still haven't connected to a lobby! Wait or check your internet connection!");
            return;
        }

        string roomCode = GenerateRoomNumber();

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;

        PhotonNetwork.CreateRoom(roomCode, roomOptions, null);
        LoadingScreenManager.instance.StartLoading(true);
    }

    public void ChangeNumberOfDecks() {
        string text = numberOfDecksDropdown.options[numberOfDecksDropdown.value].text;
        int value = int.Parse(text);
        RoomManager.NumberOfDecks = value;
    }
    
    public void ChangeTimePerTurn() {
        string text = timePerTurnDropdown.options[timePerTurnDropdown.value].text;
        int value = int.Parse(text);
        RoomManager.TimePerTurn = value;
    }

    public void SetError(string msg) {
        errorPanel.SetActive(true);
        errorText.text = msg;
    }

    public void JoinGame() {
        if (!PhotonNetwork.InLobby) {
            SetError("You still haven't connected to a lobby! Wait or check your internet connection!");
            return;
        }

        if(joinRoomField.text.Length == 0) {
            SetError("Enter code of room!");
            return;
        }

        Debug.Log("Joining room...");
        LoadingScreenManager.instance.StartLoading();
        PhotonNetwork.JoinRoom(joinRoomField.text);
    }

    public void HowToPlay() {
        DisableAllPanels();
        howToPlayPanel.SetActive(true);
    }

    public void About() {
        DisableAllPanels();
        aboutPanel.SetActive(true);
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
        aboutPanel.SetActive(false);
    }
}
