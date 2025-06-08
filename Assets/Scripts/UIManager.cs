using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public Image currentSuit;
    public GameObject selectSuitButtons;

    public static UIManager instance;

    private void Start() {
        if (instance == null) instance = this;
        else {
            Destroy(this);
            return;
        }
    }

    public void ChangeSuit(string suit) {
        Suit toSuit = Card.StringToSuit(suit);
        currentSuit.sprite = Global.SuitSprites[toSuit];
        selectSuitButtons.SetActive(false);

        if(GameManager.pendingCard != null) {
            GameManager.pendingCard.thrownByPlayer.cardArranger.EnableCards();
            GameManager.CurrentCard = GameManager.pendingCard;
            GameManager.pendingCard = null;
        }

        GameManager.CurrentCard.suit = toSuit;
    }
}
