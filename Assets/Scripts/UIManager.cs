using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UIManager : MonoBehaviour {
    public Image currentSuit;
    public GameObject selectSuitButtons;
    public Button skipTurnButton;
    public Button takeCards;
    public Button lastCard;
    public PhotonView PV;

    public static UIManager instance;

    private void Start() {
        if (instance == null) instance = this;
        else {
            Destroy(gameObject);
            return;
        }
    }

    public void ChangeSuit(string suit) {
        PV.RPC("RPC_ChangeSuit", RpcTarget.AllBuffered, suit);
    }

    [PunRPC]
    private void RPC_ChangeSuit(string suit) {
        Suit toSuit = Card.StringToSuit(suit);
        if(toSuit != Suit.All) {
            currentSuit.sprite = Global.SuitSprites[toSuit];
        }
        selectSuitButtons.SetActive(false);

        if (GameManager.pendingCard != null) {
            GameManager.pendingCard.thrownByPlayer.cardArranger.EnableCards();
            GameManager.CurrentCard = GameManager.pendingCard;
            GameManager.pendingCard = null;
            GameManager.CurrentCard.data.suit = toSuit;
            GameManager.ChangeTurn();
        }

        if(toSuit != Suit.All) {
            GameManager.CurrentCard.data.suit = toSuit;
        }
    }

    public void SkipTurn() {
        GameManager.PV.RPC("RPC_ChangeTurn", RpcTarget.AllBuffered);
    }

    public void DisableButtons() {
        skipTurnButton.interactable = false;
        lastCard.interactable = false;
        takeCards.gameObject.SetActive(false);
        selectSuitButtons.SetActive(false);
    }

    public void TakeCardsFromPoolOfForcedPickup() {
        GameManager.PV.RPC("RPC_PickUpCardsFromPoolOfForcedPickup", RpcTarget.AllBuffered);
        takeCards.gameObject.SetActive(false);
    }

    public void LastCard() {
        if(GameManager.PlayerOnTurn.cardArranger.cardsInHand.Count == 1) {
            StopAllCoroutines();
        } else {
            CardStackManager.instance.PickUpCard();
        }
        lastCard.interactable = false;
    }

    public IEnumerator WaitForLastCardButtonPress() {
        yield return new WaitForSeconds(2f);
        GameManager.PV.RPC("RPC_PickUpCard", RpcTarget.AllBuffered);
        lastCard.interactable = false;
    }
}
