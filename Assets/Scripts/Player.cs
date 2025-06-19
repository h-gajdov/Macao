using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour {
    public PhotonView PV;
    public CardArranger cardArranger;

    private void Start() {
        if (GameManager.GameHasStarted) {
            PhotonNetwork.LeaveRoom();
            return;
        }

        cardArranger = GetComponentInChildren<CardArranger>();
        if (PV.IsMine) {
            PV.RPC("RPC_SpawnPlayer", RpcTarget.AllBuffered);
            PlayerManager.LocalPlayer = this;
        }
    }

    [PunRPC]
    private void RPC_SpawnPlayer() {
        if(!PlayerManager.Players.Contains(this)) PlayerManager.Players.Add(this);

        transform.parent = PlayerManager.instance.pivot;
        PlayerManager.AssignPositions();
    }

    [PunRPC]
    private void RPC_SyncDealtCards(string cardDatasJson, string currentCard, string[] undealtCards) {
        CardData[] cardDatas = JsonUtility.FromJson<CardDataArrayWrapper>(cardDatasJson).cardDatas;
        cardArranger.SpawnCards(cardDatas);
        CardStackManager.SetUndealtCards(undealtCards);

        GameManager.SetFirstCard(currentCard);
        PV.RPC("RPC_SyncPlayerData", RpcTarget.OthersBuffered, cardDatasJson);
    }

    [PunRPC]
    private void RPC_SyncPlayerData(string cardDatasJson) {
        CardData[] cardDatas = JsonUtility.FromJson<CardDataArrayWrapper>(cardDatasJson).cardDatas;
        cardArranger.SpawnCards(cardDatas);
    }

    [PunRPC]
    private void RPC_Throw(int cardIndex) {
        Card card = cardArranger.cardsInHand[cardIndex];
        card.StartCoroutine(card.Throw(this));
    }
}