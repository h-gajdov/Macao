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

    public void PickUpCard() {
        //if (!GameManager.CanPickUpCard) return;
        GameManager.CanPickUpCard = false;

        if(UndealtCards.Count == 0) {
            ReplenishCardStack();
        }

        UIManager.instance.skipTurnButton.interactable = GameManager.PlayerOnTurn.PV.IsMine;

        Card card = GameManager.PlayerOnTurn.cardArranger.SpawnCard(UndealtCards.Pop());
        if (UndealtCards.Count == 0) {
            cardStackCube.gameObject.SetActive(false);
            UIManager.instance.replenishCardStack.gameObject.SetActive(true);
        }
        card.transform.position = transform.position;
        card.transform.localEulerAngles = new Vector3(-90f, 180f, 0f);

        SetCardCubeTransform(UndealtCards.Count);
    }
    
    private void SetCardCubeTransform(int count) {
        float factor = count / 54f;
        cardStackCube.localScale = new Vector3(2.5f, factor, 3.65f);

        Vector3 stackPosition = cardStackCube.position;
        stackPosition.y = initialYCoordForStackCube + factor / 2f;
        cardStackCube.position = stackPosition;
    }

    public void ReplenishCardStack() {
        Card lastCard = GameManager.CardPoolList.Last();
        GameManager.CardPoolList.Remove(lastCard);
        GameManager.CardPoolList.Reverse();
        foreach (Card card in GameManager.CardPoolList) {
            UndealtCards.Push(card.GetValueString());
            Destroy(card.gameObject);
        }

        UIManager.instance.replenishCardStack.gameObject.SetActive(false);
        cardStackCube.gameObject.SetActive(true);
        GameManager.CardPoolList.Clear();
        GameManager.CardPoolList.Add(lastCard);
        SetCardCubeTransform(UndealtCards.Count);
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
            instance.PickUpCard();
        }
    }
}
