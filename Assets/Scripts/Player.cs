using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour {
    public string username;
    public bool ready = false;
    public bool finished = false;
    [HideInInspector] public int avatarIdx;
    [HideInInspector] public PlayerInLobby playerInLobbyPanel;
    [HideInInspector] public PlayerInLeaderboard playerInLeaderboard;
    public PlayerPanel playerPanel;

    public PhotonView PV;
    public CardArranger cardArranger;

    private void Start() {
        if (GameManager.GameHasStarted) {
            PhotonNetwork.LeaveRoom();
            return;
        }

        cardArranger = GetComponentInChildren<CardArranger>();
        if (PV.IsMine) {
            PlayerManager.LocalPlayer = this;
            PV.RPC("RPC_SpawnPlayer", RpcTarget.AllBuffered, PlayerPrefs.GetString("Username"), PlayerPrefs.GetInt("AvatarIndex"));
        }
    }

    public void ResetSettings() {
        finished = false;
        ready = false;
        foreach (Transform card in cardArranger.transform)
            Destroy(card.gameObject);

        playerPanel.StopAllCoroutines();
        cardArranger.cardsInHand.Clear();
    }

    public void Finish() {
        GameManager.FinishedPlayers.Add(this);
        finished = true;

        playerPanel.ActivateMedal(this, GameManager.FinishedPlayers.Count - 1);

        PlayerManager.Players.Remove(this);
        if (PlayerManager.Players.Count <= 1) GameManager.FinishGame();
    }

    [PunRPC]
    private void RPC_SpawnPlayer(string username, int avatarIdx) {
        this.username = username;
        this.avatarIdx = avatarIdx;
        if(PV.IsMine) {
            UIManager.instance.localPlayerPanel.SetValues(this);
            transform.parent = PlayerManager.instance.pivot;
        }

        if (!PlayerManager.Players.Contains(this)) {
            PlayerManager.Players.Add(this);
            UIManager.UpdatePlayersInLobby();
        }

        PlayerManager.AssignPositions();
    }

    [PunRPC]
    private void RPC_SyncDealtCards(string cardDatasJson, string currentCard, string[] undealtCards) {
        CardData[] cardDatas = JsonUtility.FromJson<CardDataArrayWrapper>(cardDatasJson).cardDatas;
        cardArranger.SpawnCards(cardDatas);
        CardStackManager.SetUndealtCards(undealtCards);

        GameManager.SetFirstCard(currentCard);
        PV.RPC("RPC_SyncPlayerData", RpcTarget.OthersBuffered, cardDatasJson);
    }

    [PunRPC]
    private void RPC_SyncPlayerData(string cardDatasJson) {
        CardData[] cardDatas = JsonUtility.FromJson<CardDataArrayWrapper>(cardDatasJson).cardDatas;
        cardArranger.SpawnCards(cardDatas);
    }

    [PunRPC]
    private void RPC_Throw(int cardIndex) {
        Card card = cardArranger.cardsInHand[cardIndex];
        card.StartCoroutine(card.Throw(this));
    }

    [PunRPC]
    private void RPC_ReadyToggle() {
        ready = !ready;
        playerInLobbyPanel.readyTick.SetActive(ready);
    }

    [PunRPC]
    private void RPC_TogglePlayAgain() {
        ready = !ready;
        playerInLeaderboard.readyTick.SetActive(ready);
    }
}