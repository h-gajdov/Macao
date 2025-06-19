using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static List<Card> CardPoolList = new List<Card>();
    public static Player PlayerOnTurn { get; set; }
    public static int playerTurnIndex = 0;

    public static Card CurrentCard;
    public static Card pendingCard;

    public Transform cardsPool;

    public List<Player> debug;

    public static Camera MainCamera;

    public static GameManager instance;
    public static bool Locked = false;
    public static bool CanPickUpCard = true;
    public static bool GameHasStarted = false;

    private static List<Player> Players {
        get {
            return PlayerManager.Players;
        }
    }

    private void OnValidate() {
        Global.Initialize();
    }

    public static void ForcePickUp() {
        if (!CanThrow()) {
            if (CardStackManager.PoolOfForcedPickup != 0) {
                CardStackManager.PickUpCardsFromPoolOfForcedPickup();
                if(!CanThrow()) {
                    CardStackManager.instance.PickUpCard();
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

    private static void HandleTurnTransition() {
        CanPickUpCard = true;
        if (PlayerOnTurn.PV.IsMine) {
            PlayerOnTurn.cardArranger.DisableAllCards();
            UIManager.instance.DisableButtons();
        }

        playerTurnIndex = (playerTurnIndex + 1) % Players.Count;
        PlayerOnTurn = Players[playerTurnIndex];

        PlayerOnTurn.cardArranger.EnableCards();

        if (PlayerOnTurn.PV.IsMine) UIManager.instance.replenishCardStack.interactable = true;
    }

    private static void HandleTurnTransition(bool toggleButtons) {
        CanPickUpCard = true;
        if (PlayerOnTurn.PV.IsMine && toggleButtons) {
            PlayerOnTurn.cardArranger.DisableAllCards();
            UIManager.instance.DisableButtons();
        }

        playerTurnIndex = (playerTurnIndex + 1) % Players.Count;
        PlayerOnTurn = Players[playerTurnIndex];

        if(toggleButtons) PlayerOnTurn.cardArranger.EnableCards();

        if (PlayerOnTurn.PV.IsMine) UIManager.instance.replenishCardStack.interactable = true;
    }

    public static void ChangeTurn() {
        HandleTurnTransition();
        ForcePickUp();
    }

    public static void ChangeTurn(bool forcePickUp) {
        HandleTurnTransition();
        if(forcePickUp) ForcePickUp();
    }

    public static void ChangeTurn(bool forcePickUp, bool toggleButtons) {
        HandleTurnTransition(toggleButtons);
        if (forcePickUp) ForcePickUp();
    }

    public static void SetPendingCard(Card card) {
        pendingCard = card;
        if(card.thrownByPlayer.PV.IsMine) card.thrownByPlayer.cardArranger.DisableAllCards();
    }

    public static void SetCurrentCard(Card card) {
        CurrentCard = card;
        UIManager.instance.ChangeSuit(card.data.suit.ToString());
    }

    

    private void Awake() {
        Global.Initialize();

        MainCamera = Camera.main;

        if (instance == null) instance = this;
        else {
            Destroy(gameObject);
            return;
        }
    }

    private void Update() {
        debug = Players;

        if(Input.GetKeyDown(KeyCode.G)) {
            PlayerManager.SpawnPlayer();
        }

        if (Input.GetKeyDown(KeyCode.K)) {
            DealCards();
        }
    }

    public List<string> ShuffleCards() {
        //return GameMath.ShuffleList(Global.AllCardStrings);
        //return GameMath.ShuffleListDebug(Global.AllCardStrings, 0);
        return GameMath.ShuffleList(Global.AllCardStrings, 0);
    }

    private IEnumerator DealingAnimation(string last) {
        int rrIndex = 0;
        int count = 0;

        while (count != 7) {
            RPCManager.RPC("RPC_PickUpCard", RpcTarget.AllBuffered);
            yield return new WaitForSecondsRealtime(0.2f);
            RPCManager.RPC("RPC_ChangeTurn", RpcTarget.AllBuffered, false, false);
            rrIndex = (rrIndex + 1) % Players.Count;
            if (rrIndex == 0) count++;
        }

        RPCManager.PV.RPC("RPC_SetFirstCard", RpcTarget.AllBuffered, last);
        RPCManager.RPC("RPC_GameHasStarted", RpcTarget.AllBuffered);
        RPCManager.RPC("RPC_InitializeAvailabilityOfCards", RpcTarget.AllBuffered);
        RPCManager.RPC("RPC_ForcePickUp", RpcTarget.AllBuffered);
    }

    public void DealCards() {
        List<string> deck = ShuffleCards();
        List<string> toRemove = new List<string>();
        Dictionary<Player, List<CardData>> map = new Dictionary<Player, List<CardData>>();

        foreach (Player p in Players) {
            map.Add(p, new List<CardData>());
        }

        string last = deck.Last();
        deck.Remove(last);

        int playerOnTurnIdx = 0; //Random.Range(0, Players.Count);
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
