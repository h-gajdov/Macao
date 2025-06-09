using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UIManager : MonoBehaviour {
    public Image currentSuit;
    public GameObject selectSuitButtons;
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
        currentSuit.sprite = Global.SuitSprites[toSuit];
        selectSuitButtons.SetActive(false);

        if (GameManager.pendingCard != null) {
            GameManager.pendingCard.thrownByPlayer.cardArranger.EnableCards();
            GameManager.CurrentCard = GameManager.pendingCard;
            GameManager.pendingCard = null;
            GameManager.ChangeTurn();
        }

        GameManager.CurrentCard.data.suit = toSuit;
    }
}
