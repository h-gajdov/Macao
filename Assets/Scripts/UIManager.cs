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
    public Button replenishCardStack;
    public PhotonView PV;

    public static UIManager instance;

    bool pressed = false;

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

        if(toSuit != Suit.All && GameManager.CurrentCard != null) {
            GameManager.CurrentCard.data.suit = toSuit;
        }
    }

    public void SkipTurn() {
        GameManager.PV.RPC("RPC_ChangeTurn", RpcTarget.AllBuffered);
    }

    public void DisableButtons() {
        skipTurnButton.interactable = false;
        lastCard.interactable = false;
        replenishCardStack.interactable = false;
        takeCards.gameObject.SetActive(false);
        selectSuitButtons.SetActive(false);
    }

    public void TakeCardsFromPoolOfForcedPickup() {
        GameManager.PV.RPC("RPC_PickUpCardsFromPoolOfForcedPickup", RpcTarget.AllBuffered);
        takeCards.gameObject.SetActive(false);
    }

    public void LastCard() {
        if(GameManager.PlayerOnTurn.cardArranger.cardsInHand.Count == 1) {
            pressed = true;
        } else {
            CardStackManager.instance.PickUpCard();
        }
        lastCard.interactable = false;
    }

    public IEnumerator WaitForLastCardButtonPress() {
        if (!GameManager.PlayerOnTurn.PV.IsMine) {
            GameManager.Locked = true;
            while(GameManager.Locked) {
                yield return null;
            }
            yield break;
        }

        lastCard.interactable = true;
        yield return new WaitForSeconds(2f);
        if(!pressed) {
            GameManager.PV.RPC("RPC_PickUpCard", RpcTarget.AllBuffered);
        }
        lastCard.interactable = false;
        pressed = false;
        GameManager.PV.RPC("RPC_UnlockPlayers", RpcTarget.OthersBuffered);
        yield return null;
    }
}
