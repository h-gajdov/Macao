using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static List<Player> Players = new List<Player>();
    public static Player PlayerOnTurn { get; private set; }
    private static int playerTurnIndex = 0;

    public static Card CurrentCard;
    public static Card pendingCard;
    public static Player localPlayer;

    public GameObject playerPrefab;
    public Transform cardsPool;
    public float boundSlider = 0.1f;

    public List<Player> debug;

    public static Camera MainCamera;

    public static Vector3[] PlayerPositions;
    public static Vector3[] PlayerRotations = {
        Vector3.zero,
        Vector3.up * 180,
        Vector3.up * 90,
        Vector3.up * -90
    };

    public static GameManager instance;

    private void OnValidate() {
        Global.Initialize();
    }

    public static void ChangeTurn() {
        playerTurnIndex = (playerTurnIndex + 1) % Players.Count;
        PlayerOnTurn = Players[playerTurnIndex];
    }

    public static void SetPendingCard(Card card) {
        pendingCard = card;
        card.thrownByPlayer.cardArranger.DisableAllCards();
    }

    public static void SetCurrentCard(Card card) {
        CurrentCard = card;
        UIManager.instance.ChangeSuit(card.suit.ToString());
    }

    private void Start() {
        Global.Initialize();

        MainCamera = Camera.main;
        float distanceCameraToOrigin = Vector3.Distance(MainCamera.transform.position, Vector3.zero);
        float halfWidth = Screen.width / 2;
        float halfHeight = Screen.height / 2;
        PlayerPositions = new Vector3[] {
            Camera.main.ScreenToWorldPoint(new Vector3(halfWidth, boundSlider, distanceCameraToOrigin)),
            Camera.main.ScreenToWorldPoint(new Vector3(halfWidth, Screen.height - boundSlider, distanceCameraToOrigin)),
            Camera.main.ScreenToWorldPoint(new Vector3(boundSlider, halfHeight, distanceCameraToOrigin)),
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.width - boundSlider, halfHeight, distanceCameraToOrigin)),
        };

        if (instance == null) instance = this;
        else {
            Destroy(this);
            return;
        }
    }

    private void Update() {
        debug = Players;

        AssignPositions();

        if (Input.GetKeyDown(KeyCode.K)) {
            localPlayer.photonView.RPC("DealCards", RpcTarget.All);
        }
    }

    public static void AssignPositions() {
        for (int i = 0; i < Players.Count; i++) {
            Players[i].transform.position = PlayerPositions[i];
            Players[i].transform.eulerAngles = PlayerRotations[i];
        }
    }

    public void SpawnPlayer() {
        Player player = PhotonNetwork.Instantiate("Prefabs/" + playerPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<Player>();
    }
}
