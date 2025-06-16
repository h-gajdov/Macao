using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour {
    public PhotonView PV;
    public CardArranger cardArranger;

    private void Start() {
        cardArranger = GetComponentInChildren<CardArranger>();
        PV.RPC("RPC_SpawnPlayer", RpcTarget.AllBuffered);
        if (PV.IsMine) GameManager.LocalPlayer = this;
    }

    [PunRPC]
    private void RPC_SpawnPlayer() {
        if(!GameManager.Players.Contains(this)) GameManager.Players.Add(this);

        transform.parent = GameManager.instance.pivot;
        //GameManager.AssignPositions();
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
        //card.Throw(this);
        card.StartCoroutine(card.Throw(this));
    }
}