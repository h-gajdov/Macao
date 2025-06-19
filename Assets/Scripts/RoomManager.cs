using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks {
    public static RoomOptions options;

    private void Start() {
        options = new RoomOptions();
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
            RPCManager.RPC("RPC_ShuffleCharacterMaterialIndices", RpcTarget.AllBuffered, seed);
        }
        PlayerManager.SpawnPlayer();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) {
        Debug.Log("Player has disconnected: " + otherPlayer.NickName);
        int index = PlayerManager.Players.FindIndex(p => p != null && p.PV.OwnerActorNr == otherPlayer.ActorNumber);
        if (index == -1) return;

        PlayerManager.DisableCharacters();
        PlayerManager.Players[index] = null;

        if(!GameManager.GameHasStarted) {
            PlayerManager.Players.RemoveAt(index);
            PlayerManager.AssignPositions();
        }
    }

    public override void OnLeftRoom() {
        PhotonNetwork.Disconnect();
    }
}
