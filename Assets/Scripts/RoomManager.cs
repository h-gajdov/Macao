using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks {
    private void Start() {
        Debug.Log("Connecting...");

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
        base.OnConnectedToMaster();

        Debug.Log("Connected to Master");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() {
        base.OnJoinedLobby();

        PhotonNetwork.JoinOrCreateRoom("test", null, null);

        Debug.Log("We are connected to a room!");
    }

    public override void OnJoinedRoom() {
        base.OnJoinedRoom();

        if (PhotonNetwork.IsMasterClient) {
            int seed = (int)DateTime.UtcNow.Ticks;
            GameManager.PV.RPC("RPC_ShuffleCharacterMaterialIndices", RpcTarget.AllBuffered, seed);
        }
        GameManager.SpawnPlayer();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) {
        Debug.Log("Player has disconnected!");
        int index = GameManager.Players.FindIndex(p => p != null && p.PV.OwnerActorNr == otherPlayer.ActorNumber);
        //GameManager.Players.RemoveAll(p => p.PV.OwnerActorNr == otherPlayer.ActorNumber);
        if (index == -1) return;

        GameManager.instance.characterTransform.GetChild(index).gameObject.SetActive(false);
        GameManager.Players[index] = null;
    }
}
