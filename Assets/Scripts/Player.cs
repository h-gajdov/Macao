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

    //TODO: Try moving the code for dealing cards to GameManager
    public List<string> ShuffleCards() {
        System.Random prng = new System.Random(0);
        //List<string> shuffeledDeck = Global.AllCardStrings.OrderBy(i => Guid.NewGuid()).ToList();
        List<string> shuffeledDeck = Global.AllCardStrings.OrderBy(i => prng.Next()).ToList();
        return shuffeledDeck;
    }

    [PunRPC]
    public void DealCards() {
        List<string> deck = ShuffleCards();

        int rrIndex = 0;
        int count = 0;

        while (count != 7) {
            int idx = rrIndex + count * GameManager.Players.Count;
            GameManager.Players[rrIndex].cardArranger.SpawnCard(deck[idx]);
            rrIndex = (rrIndex + 1) % GameManager.Players.Count;
            if (rrIndex == 0) count++;
        }

        Card firstCardInPool = cardArranger.SpawnCard(deck.Last(), GameManager.instance.cardsPool);
        GameManager.CurrentSuit = firstCardInPool.suit;
        firstCardInPool.Throw();
        firstCardInPool.transform.position = Vector3.zero;
        Debug.Log(GameManager.CurrentSuit);
    }
}