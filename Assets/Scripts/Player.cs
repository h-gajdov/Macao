using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour {
    public PhotonView PV;
    public CardArranger cardArranger;
    [HideInInspector] public int characterMaterialIndex = -1;

    private static List<int> takenCharacterIndexes = new List<int>();

    private void Start() {
        cardArranger = GetComponentInChildren<CardArranger>();
        if (PV.IsMine) {
            do { //TODO: Refactor this
                characterMaterialIndex = Random.Range(0, 4);
            } while (takenCharacterIndexes.Contains(characterMaterialIndex));
            PV.RPC("RPC_SpawnPlayer", RpcTarget.AllBuffered, characterMaterialIndex);
            GameManager.LocalPlayer = this;
        }
    }

    [PunRPC]
    private void RPC_SpawnPlayer(int characterMaterialIndex) {
        if(!GameManager.Players.Contains(this)) GameManager.Players.Add(this);

        this.characterMaterialIndex = characterMaterialIndex;
        takenCharacterIndexes.Add(characterMaterialIndex);

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
        card.StartCoroutine(card.Throw(this));
    }
}