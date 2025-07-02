using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.UI.Image;

public class GameManager : MonoBehaviour {
    public static List<Card> CardPoolList = new List<Card>();
    public static List<Player> FinishedPlayers = new List<Player>();
    public static List<Player> InitialPlayerList = new List<Player>();

    public static Player PlayerOnTurn { get; set; }
    public static int playerTurnIndex = 0;
    public static int NumberOfDecks = 1;

    public static Card CurrentCard;
    public static Card pendingCard;

    public Transform cardsPool;

    public List<Player> debug;

    public static Camera MainCamera;

    public static GameManager instance;
    public static bool Locked = false;
    public static bool CanPickUpCard = true;
    public static bool GameHasStarted = false;
    public static bool GameHasFinished = false;

    public void ResetValues() {
        CardPoolList = new List<Card>();
        PlayerOnTurn = null;
        playerTurnIndex = 0;
        NumberOfDecks = 1;
        CurrentCard = null;
        pendingCard = null;
        Locked = false;
        GameHasStarted = false;
        CanPickUpCard = true;
        GameHasFinished = false;
    }

    private static List<Player> Players {
        get {
            return PlayerManager.Players;
        }
    }

    private void OnValidate() {
        Global.Initialize();
    }

    public static void ForcePickUp(Player next) {
        if (!CanThrow()) {
            if (CardStackManager.PoolOfForcedPickup != 0) {
                CardStackManager.PickUpCardsFromPoolOfForcedPickup();
                if(!CanThrow()) {
                    CardStackManager.instance.PickUpCard(false, next);
                }
            } else 
                CardStackManager.instance.PickUpCard();
        }

        if (!CanThrow()) { //This is if can't throw after pick up change turn
            instance.StartCoroutine(WaitBeforeChangeOfTurn());
        }
    }

    public static bool CanThrow() {
        int value = CurrentCard.data.value;
        Suit suit = CurrentCard.data.suit;
        CardArranger cardArranger = PlayerOnTurn.cardArranger;
        return suit == Suit.All ||
               cardArranger.Contains(value) ||
               cardArranger.Contains(suit) ||
               cardArranger.Contains(11) ||
               cardArranger.Contains(14) ||
               cardArranger.Contains(15);
    }

    private static Player HandleTurnTransition() {
        CanPickUpCard = true;
        if (PlayerOnTurn != null && PlayerOnTurn.PV.IsMine) {
            PlayerOnTurn.cardArranger.DisableAllCards();
            UIManager.instance.DisableButtons();
        }

        do {
            playerTurnIndex = (playerTurnIndex + 1) % Players.Count;
            if (GameHasFinished) return null;
        } while (Players[playerTurnIndex] == null || Players[playerTurnIndex].finished == true);

        Player nextPlayer = Players[playerTurnIndex];
        PlayerOnTurn.playerPanel.DeactivatePanel();
        PlayerOnTurn.playerPanel.timeFrame.fillAmount = 0;
        PlayerOnTurn.playerPanel.StopAllCoroutines();
        PlayerOnTurn = nextPlayer;
        PlayerOnTurn.playerPanel.ActivatePanel();
        PlayerOnTurn.playerPanel.StartCountingTime();

        PlayerOnTurn.cardArranger.EnableCards();

        if (PlayerOnTurn.PV.IsMine) UIManager.instance.replenishCardStack.interactable = true;
        return nextPlayer;
    }

    private static Player HandleTurnTransition(bool toggleButtons) {
        CanPickUpCard = true;
        if (PlayerOnTurn.PV.IsMine && toggleButtons) {
            PlayerOnTurn.cardArranger.DisableAllCards();
            UIManager.instance.DisableButtons();
        }

        do {
            playerTurnIndex = (playerTurnIndex + 1) % Players.Count;
            if (GameHasFinished) return null;
        } while (Players[playerTurnIndex] == null || Players[playerTurnIndex].finished == true);

        Player nextPlayer = Players[playerTurnIndex];
        PlayerOnTurn.playerPanel.DeactivatePanel();
        PlayerOnTurn.playerPanel.timeFrame.fillAmount = 0;
        PlayerOnTurn.playerPanel.StopAllCoroutines();
        PlayerOnTurn = nextPlayer;
        PlayerOnTurn.playerPanel.ActivatePanel();
        PlayerOnTurn.playerPanel.StartCountingTime();

        PlayerOnTurn.cardArranger.EnableCards();

        if (PlayerOnTurn.PV.IsMine) UIManager.instance.replenishCardStack.interactable = true;
        return nextPlayer;
    }

    public static void ChangeTurn() {
        Player next = HandleTurnTransition();
        ForcePickUp(next);
    }

    public static void ChangeTurn(bool forcePickUp) {
        Player next = HandleTurnTransition();
        if(forcePickUp) ForcePickUp(next);
    }

    public static void ChangeTurn(bool forcePickUp, bool toggleButtons) {
        Player next = HandleTurnTransition(toggleButtons);
        if (forcePickUp) ForcePickUp(next);
    }

    public static void SetPendingCard(Card card) {
        pendingCard = card;
        if(card.thrownByPlayer.PV.IsMine) card.thrownByPlayer.cardArranger.DisableAllCards();
    }

    public static void SetCurrentCard(Card card) {
        CurrentCard = card;
        UIManager.instance.ChangeSuit(card.data.suit.ToString());
    }

    public static void FinishGame() {
        GameHasFinished = true;

        LobbyManager.ReadyCount = 0;
        UIManager.instance.winningPanel.SetActive(true);

        foreach (Player p in FinishedPlayers) {
            p.playerPanel.StopAllCoroutines();
            p.ready = false;
        }
        foreach (Player p in Players) {
            if (p == null) continue;
            p.playerPanel.StopAllCoroutines();
            p.ready = false;
        }
    }

    private void Awake() {
        Global.Initialize();
        ResetValues();

        MainCamera = Camera.main;

        if (instance == null) instance = this;
        else {
            Destroy(this);
            return;
        }
    }

    private void Update() {
        debug = GameManager.InitialPlayerList;
    }

    public List<string> ShuffleCards() {
        return GameMath.ShuffleList(Global.AllCardStrings);
        //return GameMath.ShuffleListDebug(Global.AllCardStrings, 0);
        //return GameMath.ShuffleList(Global.AllCardStrings, 0);
    }

    private IEnumerator DealingAnimation(string last) {
        int rrIndex = 0;
        int count = 0;

        while (count != 7) {
            RPCManager.RPC("RPC_PickUpCard", RpcTarget.AllBuffered, true);
            yield return new WaitForSecondsRealtime(0.2f);
            RPCManager.RPC("RPC_ChangeTurn", RpcTarget.AllBuffered, false, false);
            rrIndex = (rrIndex + 1) % Players.Count;
            if (rrIndex == 0) count++;
        }

        RPCManager.RPC("RPC_SetFirstCard", RpcTarget.AllBuffered, last);
        RPCManager.RPC("RPC_GameHasStarted", RpcTarget.AllBuffered);
        RPCManager.RPC("RPC_InitializeAvailabilityOfCards", RpcTarget.AllBuffered);
        RPCManager.RPC("RPC_ForcePickUp", RpcTarget.AllBuffered);
        RPCManager.RPC("RPC_StartCountingTime", RpcTarget.AllBuffered);
        RPCManager.RPC("RPC_SyncInitialPlayerList", RpcTarget.AllBuffered);
    }

    public void DealCards() {
        List<string> deck = new List<string>();
        List<string> toRemove = new List<string>();
        Dictionary<Player, List<CardData>> map = new Dictionary<Player, List<CardData>>();

        for (int i = 0; i < NumberOfDecks; i++) deck.AddRange(ShuffleCards());

        foreach (Player p in Players) {
            map.Add(p, new List<CardData>());
        }

        string last = deck.Last();
        deck.Remove(last);

        int playerOnTurnIdx = Random.Range(0, Players.Count);
        RPCManager.RPC("RPC_SetPlayerOnTurn", RpcTarget.AllBuffered, playerOnTurnIdx);
        RPCManager.RPC("RPC_SetUndealtCards", RpcTarget.AllBuffered, deck.ToArray());
        StartCoroutine(DealingAnimation(last));
    }

    public static void SetFirstCard(string value) {
        Card firstCardInPool = Players[0].cardArranger.SpawnCard(value, instance.cardsPool);
        CurrentCard = firstCardInPool;
        UIManager.instance.currentSuit.sprite = Global.SuitSprites[CurrentCard.data.suit];
        firstCardInPool.StartCoroutine(firstCardInPool.Throw(null));
        firstCardInPool.transform.position = Vector3.zero;
    }

    private static IEnumerator WaitBeforeChangeOfTurn() {
        yield return new WaitForSecondsRealtime(1f);
        ChangeTurn();
    }
}
