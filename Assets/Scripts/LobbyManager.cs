using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour {
    public Button readyButton;
    public Button playButton;
    private TextMeshProUGUI playButtonText;

    public static int ReadyCount { get; private set; }
    public static LobbyManager instance;

    private void Awake() {
        if (instance == null) instance = this;
        else {
            Destroy(this);
            return;
        }

        playButtonText = playButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void FixedUpdate() {
        ReadyCount = 0;
        foreach(Player player in PlayerManager.Players) {
            if (!player.ready) continue;
            ReadyCount++;
        }

        Color textColor = playButtonText.color;
        bool canPlay = ReadyCount > 1 && ReadyCount == PlayerManager.Players.Count;
        playButton.interactable = canPlay;

        textColor.a = (canPlay) ? 1f : 0.5f;
        playButtonText.color = textColor;
    }

    public static void CheckIfLocalMine(Player p) {
        if(p.PV.Owner == PhotonNetwork.MasterClient) {
            instance.readyButton.gameObject.SetActive(false);
            instance.playButton.gameObject.SetActive(true);
        } else {
            instance.readyButton.gameObject.SetActive(true);
            instance.playButton.gameObject.SetActive(false);
        }
    }

    public void ReadyToggle() {
        PlayerManager.LocalPlayer.PV.RPC("RPC_ReadyToggle", RpcTarget.AllBuffered);
        readyButton.GetComponentInChildren<TextMeshProUGUI>().text = (PlayerManager.LocalPlayer.ready) ? "Unready" : "Ready";
    }

    public void Play() {
        RPCManager.RPC("RPC_DisableLobbyPanel", RpcTarget.All);
        GameManager.instance.DealCards();
    }

    public void Quit() {
        PhotonNetwork.LeaveRoom();
    }
}
