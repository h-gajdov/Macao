using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CardStackManager : MonoBehaviour {
    public static Stack<string> UndealtCards = new Stack<string>();
    public static int PoolOfForcedPickup = 0;

    public SpriteRenderer spriteRenderer;
    public Color selectedColor;
    public float smoothness = 5f;

    public static CardStackManager instance;

    private void Awake() {
        if (instance == null) instance = this;
        else {
            Destroy(gameObject);
            return;
        }
    }

    private void OnMouseEnter() {
        StopAllCoroutines();
        StartCoroutine(LerpToColor(selectedColor));
    }

    private void OnMouseOver() {
        if(Input.GetMouseButtonDown(0) && UndealtCards.Count > 0 && GameManager.PlayerOnTurn == GameManager.LocalPlayer) {
            GameManager.PV.RPC("RPC_PickUpCard", RpcTarget.AllBuffered);
        }
    }

    private void OnMouseExit() {
        StopAllCoroutines();
        StartCoroutine(LerpToColor(Color.white));
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
            spriteRenderer.gameObject.SetActive(false);
            UIManager.instance.replenishCardStack.gameObject.SetActive(true);
        }
        card.transform.position = transform.position;
        card.transform.eulerAngles = Vector3.right * -90f;
    }
    
    public void ReplenishCardStack() {
        Card lastCard = GameManager.CardPoolList.Last();
        GameManager.CardPoolList.Remove(lastCard);
        GameManager.CardPoolList.Reverse();
        foreach (Card card in GameManager.CardPoolList) {
            UndealtCards.Push(card.GetValueString());
        }

        UIManager.instance.replenishCardStack.gameObject.SetActive(false);
        spriteRenderer.gameObject.SetActive(true);
        GameManager.CardPoolList.Clear();
        GameManager.CardPoolList.Add(lastCard);
    }

    private IEnumerator LerpToColor(Color target) {
        while(spriteRenderer.color != target) {
            Color currentColor = spriteRenderer.color;
            spriteRenderer.color = Color.Lerp(currentColor, target, smoothness * Time.deltaTime);
            yield return null;
        }
    }

    public static void SetUndealtCards(List<string> listOfUndealtCards) {
        UndealtCards.Clear();
        SetUndealtCards(listOfUndealtCards.ToArray());
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
