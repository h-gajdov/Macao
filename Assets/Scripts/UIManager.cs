using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public Image frameOfCurrentSuit;
    public Image currentSuit;
    public PlayerPanel localPlayerPanel;
    public Gradient timeFrameGradient;

    public GameObject selectSuitButtons;
    public GameObject playerInLobbyPrefab;
    public GameObject winningPanel;
    public Transform lobbyContent;

    public Button skipTurnButton;
    public Button takeCards;
    public Button lastCard;
    public Button replenishCardStack;
    public TextMeshProUGUI roomCodeText;
    public PhotonView PV;

    public static UIManager instance;

    bool pressed = false;

    private void Awake() {
        if (instance == null) instance = this;
        else {
            Destroy(this);
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
            frameOfCurrentSuit.color = (toSuit == Suit.Diamonds || toSuit == Suit.Hearts) ? Color.red : Color.black;
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
        RPCManager.RPC("RPC_ChangeTurn", RpcTarget.AllBuffered);
    }

    public void DisableButtons() {
        skipTurnButton.interactable = false;
        lastCard.interactable = false;
        replenishCardStack.interactable = false;
        takeCards.gameObject.SetActive(false);
        selectSuitButtons.SetActive(false);
    }

    public void TakeCardsFromPoolOfForcedPickup() {
        RPCManager.RPC("RPC_PickUpCardsFromPoolOfForcedPickup", RpcTarget.AllBuffered);
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

    public void CopyToClipboard() {
        GUIUtility.systemCopyBuffer = roomCodeText.text.Split(' ')[1];
    }

    public static void UpdatePlayersInLobby() {
        foreach (Transform child in instance.lobbyContent) Destroy(child.gameObject);

        foreach(Player player in PlayerManager.Players) {
            GameObject pLobby = Instantiate(instance.playerInLobbyPrefab, instance.lobbyContent, false);
            PlayerInLobby lobbyObject = pLobby.GetComponent<PlayerInLobby>();
            player.playerInLobbyPanel = lobbyObject;
            lobbyObject.username.text = player.username;
            lobbyObject.avatarImage.sprite = Global.AvatarSprites[player.avatarIdx];

            if(player.PV.Owner == PhotonNetwork.MasterClient) {
                lobbyObject.hostText.SetActive(true);
                player.ready = true;
            }
            lobbyObject.readyTick.SetActive(player.ready);
        }
    }

    public static void SetRoomCode(string code) {
        instance.roomCodeText.text = "Code: " + code;
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
            RPCManager.RPC("RPC_PickUpCard", RpcTarget.AllBuffered);
        }
        lastCard.interactable = false;
        pressed = false;
        RPCManager.RPC("RPC_UnlockPlayers", RpcTarget.OthersBuffered);
        yield return null;
    }
}
