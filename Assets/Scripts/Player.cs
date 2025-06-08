using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour {
    public PhotonView photonView;
    public CardArranger cardArranger;

    private void Start() {
        cardArranger = GetComponentInChildren<CardArranger>();

        if (photonView.IsMine) {
            GameManager.localPlayer = this;
            GameManager.Players.Insert(0, this);
        } else GameManager.Players.Add(this);

        GameManager.AssignPositions();
    }

    [PunRPC]
    private void RPC_SyncDealtCards(string cardDatasJson, string currentCard) {
        CardData[] cardDatas = JsonUtility.FromJson<CardDataArrayWrapper>(cardDatasJson).cardDatas;
        cardArranger.SpawnCards(cardDatas);

        GameManager.SetFirstCard(currentCard);
        photonView.RPC("RPC_SyncPlayerData", RpcTarget.OthersBuffered, cardDatasJson);
    }

    [PunRPC]
    private void RPC_SyncPlayerData(string cardDatasJson) {
        CardData[] cardDatas = JsonUtility.FromJson<CardDataArrayWrapper>(cardDatasJson).cardDatas;
        cardArranger.SpawnCards(cardDatas);
    }
}