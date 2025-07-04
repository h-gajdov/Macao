using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RPCManager : MonoBehaviour {
    public static PhotonView PV;

    private void Awake() {
        PV = GetComponent<PhotonView>();
    }

    public static void RPC(string functionName, RpcTarget target, params object[] parameters) {
        PV.RPC(functionName, target, parameters);
    }

    [PunRPC]
    private void RPC_SetPlayerOnTurn(int value) {
        AudioManager.Play("ShuffelingDeck");

        GameManager.playerTurnIndex = value;

        GameManager.PlayerOnTurn = PlayerManager.Players[value];
        GameManager.PlayerOnTurn.cardArranger.EnableCards();

        if (GameManager.PlayerOnTurn.PV.IsMine) UIManager.instance.replenishCardStack.interactable = true;
    }

    [PunRPC]
    private void RPC_StartCountingTime() {
        GameManager.PlayerOnTurn.playerPanel.StartCountingTime();
    }

    [PunRPC]
    private void RPC_InitializeAvailabilityOfCards() {
        foreach (Player p in PlayerManager.Players) {
            if (p == null || !p.PV.IsMine) continue;
            p.cardArranger.DisableAllCards();
        }

        GameManager.PlayerOnTurn.cardArranger.EnableCards();
    }

    [PunRPC]
    private void RPC_PickUpCard() {
        CardStackManager.instance.PickUpCard();
    }

    [PunRPC]
    private void RPC_PickUpCard(bool forced) {
        CardStackManager.instance.PickUpCard(forced);
    }

    [PunRPC]
    private void RPC_PickUpCardsFromPoolOfForcedPickup() {
        CardStackManager.PickUpCardsFromPoolOfForcedPickup();
    }

    [PunRPC]
    private void RPC_ChangeTurn() {
        GameManager.ChangeTurn();
    }

    [PunRPC]
    private void RPC_ChangeTurn(bool forcePickUp) {
        GameManager.ChangeTurn(forcePickUp);
    }

    [PunRPC]
    private void RPC_ChangeTurn(bool forcePickUp, bool toggleButtons) {
        GameManager.ChangeTurn(forcePickUp, toggleButtons);
    }

    [PunRPC]
    private void RPC_UnlockPlayers() {
        GameManager.Locked = false;
    }

    [PunRPC]
    private void RPC_SetUndealtCards(string[] deck) {
        CardStackManager.SetUndealtCards(new List<string>(deck));
    }

    [PunRPC]
    private void RPC_SetFirstCard(string value) {
        GameManager.SetFirstCard(value);
    }

    [PunRPC]
    private void RPC_ShuffleCharacterMaterialIndices(int seed) {
        if (PlayerManager.materialIndicesShuffled) return;
        PlayerManager.characterMaterialIndices = GameMath.ShuffleList(PlayerManager.characterMaterialIndices, seed);
        PlayerManager.materialIndicesShuffled = true;
    }

    [PunRPC]
    private void RPC_GameHasStarted() {
        GameManager.GameHasStarted = true;
    }

    [PunRPC]
    private void RPC_DisableLobbyPanel() {
        LobbyManager.instance.gameObject.SetActive(false);
        UIManager.instance.localPlayerPanel.gameObject.SetActive(true);
    }

    [PunRPC]
    private void RPC_DisableWinningPanel() {
        UIManager.instance.winningPanel.SetActive(false);
    }

    [PunRPC]
    private void RPC_ForcePickUp() {
        GameManager.ForcePickUp(null);
    }

    [PunRPC]
    private void RPC_SpawnPlayerInLobby(string username, int avatarIdx) {
        GameObject pLobby = Instantiate(UIManager.instance.playerInLobbyPrefab, UIManager.instance.lobbyContent);
        TextMeshProUGUI usernameTMP = pLobby.GetComponentInChildren<TextMeshProUGUI>();
        Image avatarImage = pLobby.GetComponentInChildren<Image>();
        usernameTMP.text = username;
        avatarImage.sprite = Global.AvatarSprites[avatarIdx];
    }

    [PunRPC]
    private void RPC_SyncInitialPlayerList() {
        GameManager.InitialPlayerList = new List<Player>(PlayerManager.Players);
    }

    [PunRPC]
    private void RPC_ResetPlayers() {
        PlayerManager.Players = GameManager.InitialPlayerList;
        GameManager.FinishedPlayers.Clear();
        foreach (Player p in PlayerManager.Players) {
            p.ResetSettings();
        }

        foreach(Transform panel in WinningPanelManager.instance.content) {
            Destroy(panel.gameObject);
        }

        foreach (Transform card in GameManager.instance.cardsPool) Destroy(card.gameObject);

        CardStackManager.instance.ResetValues();
        GameManager.instance.ResetValues();
    }

    [PunRPC]
    private void RPC_SetDeckAndTimerSettings(int numberOfDecks, int timePerTurn) {
        GameManager.NumberOfDecks = numberOfDecks;
        PlayerPanel.TimeOfTurn = timePerTurn;

        Debug.Log("Decks: " + GameManager.NumberOfDecks);
        Debug.Log("Time per turn: " + PlayerPanel.TimeOfTurn);
    }

    [PunRPC]
    private void RPC_ReplenishCardStack() {
        Card lastCard = GameManager.CardPoolList.Last();
        GameManager.CardPoolList.Remove(lastCard);
        GameManager.CardPoolList.Reverse();
        foreach (Card card in GameManager.CardPoolList) {
            CardStackManager.UndealtCards.Push(card.GetValueString());
            Destroy(card.gameObject);
        }

        UIManager.instance.replenishCardStack.gameObject.SetActive(false);
        CardStackManager.instance.cardStackCube.gameObject.SetActive(true);
        GameManager.CardPoolList.Clear();
        GameManager.CardPoolList.Add(lastCard);
        CardStackManager.instance.SetCardCubeTransform(CardStackManager.UndealtCards.Count);
    }
}
