using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CardStackManager : MonoBehaviour {
    public static Stack<string> UndealtCards = new Stack<string>();
    public static int PoolOfForcedPickup = 0;

    public Transform cardStackCube;
    private float initialYCoordForStackCube;

    public static CardStackManager instance;

    public void ResetValues() {
        UndealtCards = new Stack<string>();
        PoolOfForcedPickup = 0;
    }

    private void Awake() {
        ResetValues();

        if (instance == null) instance = this;
        else {
            Destroy(this);
            return;
        }

        initialYCoordForStackCube = transform.position.y;

        int dealtCards = PlayerManager.Players.Count * 7 + 1;
        SetCardCubeTransform(54 - dealtCards);
    }

    public void PickUpCard(bool forced = false) {
        if (GameManager.GameHasFinished) return;
        if (!forced) {
            if (!GameManager.CanPickUpCard) return;

            UIManager.instance.skipTurnButton.interactable = GameManager.PlayerOnTurn.PV.IsMine;
            GameManager.CanPickUpCard = false;
        }

        if(UndealtCards.Count == 0) {
            ReplenishCardStack();
        }


        Card card = GameManager.PlayerOnTurn.cardArranger.SpawnCard(UndealtCards.Pop());
        if (UndealtCards.Count == 0) {
            cardStackCube.gameObject.SetActive(false);
            UIManager.instance.replenishCardStack.gameObject.SetActive(true);
        }
        card.transform.position = transform.position;
        card.transform.localEulerAngles = new Vector3(-90f, 180f, 0f);

        SetCardCubeTransform(UndealtCards.Count);
        AudioManager.Play("PickingUpCard");
    }

    public void SetCardCubeTransform(int count) {
        float factor = count / (GameManager.NumberOfDecks * 54f);
        cardStackCube.localScale = new Vector3(2.5f, factor, 3.65f);

        Vector3 stackPosition = cardStackCube.position;
        stackPosition.y = initialYCoordForStackCube + factor / 2f;
        cardStackCube.position = stackPosition;
    }

    public void ReplenishCardStack() {
        RPCManager.RPC("RPC_ReplenishCardStack", RpcTarget.All);
    }

    public static void SetUndealtCards(List<string> listOfUndealtCards) {
        UndealtCards.Clear();
        SetUndealtCards(listOfUndealtCards.ToArray());
        instance.SetCardCubeTransform(UndealtCards.Count);
    }

    public static void SetUndealtCards(string[] listOfUndealtCards) {
        UndealtCards.Clear();
        for(int i = listOfUndealtCards.Length - 1; i >= 0; i--) {
            UndealtCards.Push(listOfUndealtCards[i]);
        }
    }

    public static void PickUpCardsFromPoolOfForcedPickup() {
        PickUpNCards(PoolOfForcedPickup);
        PoolOfForcedPickup = 0;
    }

    private static void PickUpNCards(int n) {
        while(n-- > 0) {
            instance.PickUpCard(true);
        }
    }

    public static void PrintStack() {
        foreach(string card in UndealtCards) {
            Debug.Log(card);
        }
    }
}
