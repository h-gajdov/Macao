using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour {
    public static List<Player> Players = new List<Player>();
    public static List<Card> CardPoolList = new List<Card>();
    public static PhotonView PV;
    public static Player PlayerOnTurn { get; private set; }
    private static int playerTurnIndex = 0;

    public static Card CurrentCard;
    public static Card pendingCard;
    public static Player LocalPlayer { get; set; }

    public GameObject playerPrefab;
    public Transform pivot;
    public Transform cardsPool;
    public float boundSlider = 0.1f;

    public List<Card> debug;

    public static Camera MainCamera;

    public static Vector3[][] PlayerPositions = new Vector3[4][];
    public static Vector3[][] PlayerRotations = new Vector3[4][];
    public static Vector3[][] PivotRotations = new Vector3[4][];

    public static GameManager instance;
    public static bool Locked = false;
    public static bool CanPickUpCard = true;

    private void OnValidate() {
        Global.Initialize();
        PV = GetComponent<PhotonView>();
    }

    private static void ForcePickUp() {
        if (!CanThrow()) {
            CardStackManager.instance.PickUpCard();
        }

        if (!CanThrow()) {
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

    private void InitializePositions() {
        float distanceCameraToOrigin = Vector3.Distance(MainCamera.transform.position, Vector3.zero);
        float halfWidth = Screen.width / 2;
        float halfHeight = Screen.height / 2;
        PlayerPositions[0] = new Vector3[] {
            Vector3.zero
        };

        PlayerPositions[1] = new Vector3[] {
            Vector3.zero,
            new Vector3(0, 0, 24f)
        };

        PlayerPositions[2] = new Vector3[] {
            Vector3.zero,
            new Vector3(-20f, 0, 10f),
            new Vector3(0, 0, 24f)
        };

        PlayerPositions[3] = new Vector3[] {
            Vector3.zero,
            new Vector3(-20f, 0, 10f),
            new Vector3(0, 0, 24f),
            new Vector3(20f, 0, 10f)
        };

        PlayerRotations[0] = new Vector3[] {
            Vector3.zero,
        };

        PlayerRotations[1] = new Vector3[] {
            Vector3.zero,
            Vector3.up * 180,
        };

        PlayerRotations[2] = new Vector3[] {
            Vector3.zero,
            Vector3.up * 120,
            Vector3.up * 180,
        };

        PlayerRotations[3] = new Vector3[] {
            Vector3.zero,
            Vector3.up * 120,
            Vector3.up * 180,
            -Vector3.up * 120,
        };

        PivotRotations[0] = new Vector3[] {
            Vector3.zero
        };

        PivotRotations[1] = new Vector3[] {
            Vector3.zero,
            Vector3.up * 180f
        };

        PivotRotations[2] = new Vector3[] {
            Vector3.zero,
            Vector3.up * 90f,
            Vector3.up * 180f
        };

        PivotRotations[3] = new Vector3[] {
            Vector3.zero,
            Vector3.up * 90f,
            Vector3.up * 180f,
            Vector3.up * 270f
        };
    }

    private void Awake() {
        Global.Initialize();

        MainCamera = Camera.main;
        InitializePositions();

        if (instance == null) instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        PV = GetComponent<PhotonView>();
    }

    private void Update() {
        debug = CardPoolList;

        if(Input.GetKeyDown(KeyCode.G)) {
            SpawnPlayer();
        }

        AssignPositions();

        if (Input.GetKeyDown(KeyCode.K)) {
            DealCards();
        }
    }

    public List<string> ShuffleCards() {
        System.Random prng = new System.Random(0);
        //List<string> shuffeledDeck = Global.AllCardStrings.OrderBy(i => System.Guid.NewGuid()).ToList();
        List<string> shuffeledDeck = Global.AllCardStrings.OrderBy(i => prng.Next()).ToList();
        return shuffeledDeck;
    }

    private IEnumerator DealingAnimation(string last) {
        int rrIndex = 0;
        int count = 0;

        while (count != 7) {
            PV.RPC("RPC_PickUpCard", RpcTarget.AllBuffered);
            yield return new WaitForSecondsRealtime(0.2f);
            PV.RPC("RPC_ChangeTurn", RpcTarget.AllBuffered, false, false);
            rrIndex = (rrIndex + 1) % Players.Count;
            if (rrIndex == 0) count++;
        }
        PV.RPC("RPC_SetFirstCard", RpcTarget.AllBuffered, last);
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

        int playerOnTurnIdx = Random.Range(0, Players.Count);
        PV.RPC("RPC_SetPlayerOnTurn", RpcTarget.AllBuffered, playerOnTurnIdx);
        PV.RPC("RPC_SetUndealtCards", RpcTarget.AllBuffered, deck.ToArray());
        StartCoroutine(DealingAnimation(last));
    }

    public static void SetFirstCard(string value) {
        Card firstCardInPool = Players[0].cardArranger.SpawnCard(value, instance.cardsPool);
        CurrentCard = firstCardInPool;
        UIManager.instance.currentSuit.sprite = Global.SuitSprites[CurrentCard.data.suit];
        firstCardInPool.StartCoroutine(firstCardInPool.Throw(null));
        firstCardInPool.transform.position = Vector3.zero;
    }

    public static void AssignPositions() {
        int count = Players.Count;
        if (count == 0) return;

        int startIdx = 0;
        for(int i = 0; i < count; i++) {
            if (!Players[i].PV.IsMine) continue;
            startIdx = i;
            break;
        }

        Vector3[] positions = PlayerPositions[count - 1];
        Vector3[] rotations = PlayerRotations[count - 1];

        for (int i = 1; i < count; i++) {
            int playerIdx = (startIdx + i) % count;
            Players[playerIdx].transform.localPosition = positions[i];
            Players[playerIdx].transform.localEulerAngles = rotations[i];
        }

        instance.pivot.eulerAngles = PivotRotations[count - 1][startIdx];
    }

    public void SpawnPlayer() {
        Player player = PhotonNetwork.Instantiate("Prefabs/" + playerPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<Player>();
        Transform cam = MainCamera.transform;

        if (player.PV.IsMine) {
            Vector3 viewportBottom = new Vector3(0.5f, boundSlider, 10f);
            Vector3 worldPosition = Camera.main.ViewportToWorldPoint(viewportBottom);

            player.transform.position = worldPosition;
            player.transform.LookAt(cam.position, Vector3.up);
            player.transform.eulerAngles = Vector3.right * player.transform.eulerAngles.x;
        }
    }

    [PunRPC]
    private void RPC_SetPlayerOnTurn(int value) {
        playerTurnIndex = value;
        PlayerOnTurn = Players[playerTurnIndex];

        //TODO: Make this code better. REFACTOR IT!
        foreach (Player player in Players) {
            player.cardArranger.DisableAllCards();
            if (player == PlayerOnTurn) player.cardArranger.EnableCards();
        }
    }

    [PunRPC]
    private void RPC_PickUpCard() {
        CardStackManager.instance.PickUpCard();
    }

    [PunRPC]
    private void RPC_PickUpCardsFromPoolOfForcedPickup() {
        CardStackManager.PickUpCardsFromPoolOfForcedPickup();
    }

    [PunRPC]
    private void RPC_ChangeTurn() {
        ChangeTurn();
    }

    [PunRPC]
    private void RPC_ChangeTurn(bool forcePickUp) {
        ChangeTurn(forcePickUp);
    }

    [PunRPC]
    private void RPC_ChangeTurn(bool forcePickUp, bool toggleButtons) {
        ChangeTurn(forcePickUp, toggleButtons);
    }

    [PunRPC]
    private void RPC_UnlockPlayers() {
        Locked = false;
    }

    [PunRPC]
    private void RPC_SetUndealtCards(string[] deck) {
        CardStackManager.SetUndealtCards(new List<string>(deck));
    }

    [PunRPC]
    private void RPC_SetFirstCard(string value) {
        SetFirstCard(value);
    }

    private static IEnumerator WaitBeforeChangeOfTurn() {
        yield return new WaitForSecondsRealtime(1f);
        ChangeTurn();
    }
}
