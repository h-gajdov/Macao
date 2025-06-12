using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Transform cardsPool;
    public float boundSlider = 0.1f;

    public List<Card> debug;

    public static Camera MainCamera;

    public static Vector3[][] PlayerPositions = new Vector3[4][];
    public static Vector3[][] PlayerRotations = new Vector3[4][];

    public static GameManager instance;
    public static bool Locked = false;

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
        if (PlayerOnTurn.PV.IsMine) {
            PlayerOnTurn.cardArranger.DisableAllCards();
            UIManager.instance.DisableButtons();
        }

        playerTurnIndex = (playerTurnIndex + 1) % Players.Count;
        PlayerOnTurn = Players[playerTurnIndex];

        PlayerOnTurn.cardArranger.EnableCards();

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
            Camera.main.ScreenToWorldPoint(new Vector3(halfWidth, boundSlider, distanceCameraToOrigin)),
        };

        PlayerPositions[1] = new Vector3[] {
            Camera.main.ScreenToWorldPoint(new Vector3(halfWidth, boundSlider, distanceCameraToOrigin)),
            Camera.main.ScreenToWorldPoint(new Vector3(halfWidth, Screen.height - boundSlider, distanceCameraToOrigin)),
        };

        PlayerPositions[2] = new Vector3[] {
            Camera.main.ScreenToWorldPoint(new Vector3(halfWidth, boundSlider, distanceCameraToOrigin)),
            Camera.main.ScreenToWorldPoint(new Vector3(boundSlider, halfHeight, distanceCameraToOrigin)),
            Camera.main.ScreenToWorldPoint(new Vector3(halfWidth, Screen.height - boundSlider, distanceCameraToOrigin)),
        };

        PlayerPositions[3] = new Vector3[] {
            Camera.main.ScreenToWorldPoint(new Vector3(halfWidth, boundSlider, distanceCameraToOrigin)),
            Camera.main.ScreenToWorldPoint(new Vector3(boundSlider, halfHeight, distanceCameraToOrigin)),
            Camera.main.ScreenToWorldPoint(new Vector3(halfWidth, Screen.height - boundSlider, distanceCameraToOrigin)),
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.width - boundSlider, halfHeight, distanceCameraToOrigin)),
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
            Vector3.up * 90,
            Vector3.up * 180,
        };

        PlayerRotations[3] = new Vector3[] {
            Vector3.zero,
            Vector3.up * 90,
            Vector3.up * 180,
            Vector3.up * -90
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
        //List<string> shuffeledDeck = Global.AllCardStrings.OrderBy(i => Guid.NewGuid()).ToList();
        List<string> shuffeledDeck = Global.AllCardStrings.OrderBy(i => prng.Next()).ToList();
        return shuffeledDeck;
    }

    public void DealCards() {
        List<string> deck = ShuffleCards();
        List<string> toRemove = new List<string>();
        Dictionary<Player, List<CardData>> map = new Dictionary<Player, List<CardData>>();

        foreach (Player p in Players) {
            map.Add(p, new List<CardData>());
        }

        int rrIndex = 0;
        int count = 0;

        while (count != 7) {
            int idx = rrIndex + count * Players.Count;
            map[Players[rrIndex]].Add(CardData.ConvertValueStringToCardData(deck[idx]));
            toRemove.Add(deck[idx]);
            rrIndex = (rrIndex + 1) % Players.Count;
            if (rrIndex == 0) count++;
        }

        SetFirstCard(deck.Last());
        toRemove.Add(deck.Last());

        deck.RemoveAll((card) => toRemove.Contains(card));
        CardStackManager.SetUndealtCards(deck);

        int playerOnTurnIdx = Random.Range(0, Players.Count);
        PV.RPC("RPC_SetPlayerOnTurn", RpcTarget.AllBuffered, playerOnTurnIdx);

        foreach(Player player in Players) {
            PhotonView pv = player.PV;
            CardDataArrayWrapper cardDatas = new CardDataArrayWrapper(map[player].ToArray());
            string cardDatasJson = JsonUtility.ToJson(cardDatas);
            player.PV.RPC("RPC_SyncDealtCards", player.PV.Owner, cardDatasJson, CurrentCard.GetValueString(), deck.ToArray());
        }
    }

    public static void SetFirstCard(string value) {
        Card firstCardInPool = Players[0].cardArranger.SpawnCard(value, instance.cardsPool);
        CurrentCard = firstCardInPool;
        UIManager.instance.currentSuit.sprite = Global.SuitSprites[CurrentCard.data.suit];
        //firstCardInPool.Throw(null);
        firstCardInPool.StartCoroutine(firstCardInPool.Throw(null));
        firstCardInPool.transform.position = Vector3.zero;
    }

    public static void AssignPositions() {
        if (Players.Count == 0) return;

        int startIdx = 0;
        for(int i = 0; i < Players.Count; i++) {
            if (!Players[i].PV.IsMine) continue;
            startIdx = i;
            break;
        }

        Vector3[] positions = PlayerPositions[Players.Count - 1];
        Vector3[] rotations = PlayerRotations[Players.Count - 1];

        for (int i = 0; i < Players.Count; i++) {
            int playerIdx = (startIdx + i) % Players.Count;
            Players[playerIdx].transform.position = positions[i];
            Players[playerIdx].transform.eulerAngles = rotations[i];
        }
    }

    public void SpawnPlayer() {
        Player player = PhotonNetwork.Instantiate("Prefabs/" + playerPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<Player>();
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
    private void RPC_UnlockPlayers() {
        Locked = false;
    }

    private static IEnumerator WaitBeforeChangeOfTurn() {
        yield return new WaitForSecondsRealtime(1f);
        ChangeTurn();
    }
}
