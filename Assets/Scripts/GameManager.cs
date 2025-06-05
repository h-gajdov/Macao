using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static List<Player> Players = new List<Player>();
    public GameObject playerPrefab;
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

    private void OnValidate() {
        Global.Initialize();
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
    }

    private void Update() {
        debug = Players;

        AssignPositions();

        if (Input.GetKeyDown(KeyCode.K)) DealCards();
    }

    public List<string> ShuffleCards() {
        System.Random prng = new System.Random(0);
        //List<string> shuffeledDeck = Global.AllCardStrings.OrderBy(i => Guid.NewGuid()).ToList();
        List<string> shuffeledDeck = Global.AllCardStrings.OrderBy(i => prng.Next()).ToList();
        return shuffeledDeck;
    }

    public void DealCards() {
        List<string> deck = ShuffleCards();
        foreach(string card in deck) {
            Debug.Log(card);
        }

        int rrIndex = 0;
        int count = 0;

        while(count != 7) {
            int idx = rrIndex + count * Players.Count;
            Players[rrIndex].cardArranger.SpawnCard(deck[idx]);
            rrIndex = (rrIndex + 1) % Players.Count;
            if(rrIndex == 0) count++;
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
