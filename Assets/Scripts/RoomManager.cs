using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks {
    public static RoomManager instance;

    public static int NumberOfDecks = 2, TimePerTurn = 40;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(this);
            return;
        }
    }

    private void Start() {
        Debug.Log("Connecting...");

        PhotonNetwork.ConnectUsingSettings();
        DontDestroyOnLoad(this);
    }

    public override void OnConnectedToMaster() {
        base.OnConnectedToMaster();

        Debug.Log("Connected to Master");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() {
        base.OnJoinedLobby();

        Debug.Log("We are connected to a lobby!");
    }

    public override void OnJoinedRoom() {
        base.OnJoinedRoom();

        Debug.Log($"We have joined the room {PhotonNetwork.CurrentRoom.Name}");
        StartCoroutine(LoadSceneAsync(1));
    }

    private IEnumerator LoadSceneAsync(int index) {
        PhotonNetwork.IsMessageQueueRunning = false;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);

        LoadingScreenManager.instance.loadingBar.SetActive(true);
        LoadingScreenManager.instance.joiningRoomText.SetActive(false);

        while (!asyncLoad.isDone) {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            LoadingScreenManager.instance.SetProgress(progress);
            yield return null;
        }

        PhotonNetwork.IsMessageQueueRunning = true;

        yield return new WaitWhile(() => PlayerManager.Players.Count < PhotonNetwork.CurrentRoom.Players.Count - 1);

        UIManager.SetRoomCode(PhotonNetwork.CurrentRoom.Name);
        if (PhotonNetwork.IsMasterClient) {
            int seed = (int)DateTime.UtcNow.Ticks;
            RPCManager.RPC("RPC_ShuffleCharacterMaterialIndices", RpcTarget.AllBuffered, seed);
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            RPCManager.RPC("RPC_SetDeckAndTimerSettings", RpcTarget.AllBuffered, RoomManager.NumberOfDecks, RoomManager.TimePerTurn);

        PlayerManager.SpawnPlayer();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) {
        Debug.Log("Player has disconnected: " + otherPlayer.NickName);
        int index = PlayerManager.Players.FindIndex(p => p != null && p.PV.OwnerActorNr == otherPlayer.ActorNumber);
        int indexInitial = GameManager.InitialPlayerList.FindIndex(p => p != null && p.PV.OwnerActorNr == otherPlayer.ActorNumber);
        if (index == -1) return;

        PlayerManager.instance.characterTransform.GetChild(index).gameObject.SetActive(false);
        Destroy(PlayerManager.Players[index].playerInLobbyPanel.gameObject);

        Player leftPlayer = PlayerManager.Players[index];
        PlayerManager.Players[index] = null;

        if(GameManager.GameHasFinished) {
            foreach(Player p in GameManager.InitialPlayerList) {
                if (p != null) p.PV.RPC("RPC_Leave", p.PV.Owner);
            }
        }

        if(!GameManager.GameHasStarted) {
            PlayerManager.Players.RemoveAt(index);
            PlayerManager.AssignPositions();
            UIManager.UpdatePlayersInLobby();
        } else {
            if (GameManager.PlayerOnTurn == leftPlayer) RPCManager.RPC("RPC_ChangeTurn", RpcTarget.All, false);

            GameManager.InitialPlayerList.RemoveAt(indexInitial);
            leftPlayer.cardArranger.ReturnCards();

            int count = 0;
            foreach (Player p in PlayerManager.Players) {
                if (p == null) continue;
                if (p.playerInLeaderboard != null) Destroy(p.playerInLeaderboard.gameObject);
                count++;
            }

            if (count <= 1) {
                PhotonNetwork.LeaveRoom();
            }
        }
    }

    public override void OnLeftRoom() {
        SceneManager.LoadScene(0);
    }
}
