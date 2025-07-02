using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
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

    public Sprite[] medals;

    public static UIManager instance;

    bool pressed = false;

    private void Awake() {
        if (instance == null) instance = this;
        else {
            Destroy(this);
            return;
        }
    }

    private void Start() {
        AudioManager.ChangeVolume("MainSong", 0.25f);
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
            Player player = GameManager.PlayerOnTurn;
            if (player.cardArranger.cardsInHand.Count == 0) player.Finish();
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
            CardStackManager.instance.PickUpCard(true);
        }
        lastCard.interactable = false;
    }

    public void CopyToClipboard() {
        GUIUtility.systemCopyBuffer = roomCodeText.text.Split(' ')[1];
    }

    public void PlayAgain() {
        PlayerManager.LocalPlayer.PV.RPC("RPC_TogglePlayAgain", RpcTarget.All);
        bool allReady = true;
        foreach(Player p in GameManager.InitialPlayerList) {
            if (p.ready) continue;
            allReady = false;
            break;
        }
        if (!allReady) return;

        RPCManager.RPC("RPC_DisableWinningPanel", RpcTarget.All);
        RPCManager.RPC("RPC_ResetPlayers", RpcTarget.All);
        StartCoroutine(WaitTilAllAreReset());
    }

    private IEnumerator WaitTilAllAreReset() {
        yield return new WaitForSecondsRealtime(2f);
        GameManager.instance.DealCards();
    }

    public static void UpdatePlayersInLobby() {
        foreach (Transform child in instance.lobbyContent) Destroy(child.gameObject);

        bool first = true;
        foreach(Player player in PlayerManager.Players) {
            GameObject pLobby = Instantiate(instance.playerInLobbyPrefab, instance.lobbyContent, false);
            PlayerInLobby lobbyObject = pLobby.GetComponent<PlayerInLobby>();
            player.playerInLobbyPanel = lobbyObject;
            lobbyObject.username.text = player.username;
            lobbyObject.avatarImage.sprite = Global.AvatarSprites[player.avatarIdx];

            if (first) {
                if(player.PV.IsMine) {
                    LobbyManager.instance.readyButton.gameObject.SetActive(false);
                    LobbyManager.instance.playButton.gameObject.SetActive(true);
                }
                lobbyObject.hostText.SetActive(true);
                player.ready = true;
                first = false;
            } else if (player.PV.IsMine) { 
                LobbyManager.instance.readyButton.gameObject.SetActive(true);
                LobbyManager.instance.playButton.gameObject.SetActive(false);
            }
            lobbyObject.readyTick.SetActive(player.ready);
        }
    }

    public static void SetRoomCode(string code) {
        if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[0])
            instance.roomCodeText.text = "Code: " + code;
        else instance.roomCodeText.text = "Код: " + code;
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
            RPCManager.RPC("RPC_PickUpCard", RpcTarget.AllBuffered, true);
        }
        lastCard.interactable = false;
        pressed = false;
        RPCManager.RPC("RPC_UnlockPlayers", RpcTarget.OthersBuffered);
        yield return null;
    }
}
