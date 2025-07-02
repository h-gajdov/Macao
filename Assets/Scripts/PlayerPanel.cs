using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerPanel : MonoBehaviour {
    public Image panelImage;
    public TextMeshProUGUI username;
    public Image avatarImage;
    public Image timeFrame;
    public Image medal;

    public static int TimeOfTurn = 20; //in seconds

    private void Start() {
        medal.enabled = false;
        timeFrame.fillAmount = 0f;
        //StartCoroutine(StartCountingTime());
        DeactivatePanel();
    }

    public void SetValues(Player player) {
        username.text = player.username;
        avatarImage.sprite = Global.AvatarSprites[player.avatarIdx];
        player.playerPanel = this;
    }

    public void StartCountingTime() {
        StartCoroutine(StartCountingTime_Coroutine());
    }

    public void ActivateMedal(Player player, int medalIndex) {
        medal.enabled = true;
        medal.sprite = UIManager.instance.medals[medalIndex];
    }

    public void ActivatePanel() {
        SetTransparency(1f);
    }

    public void DeactivatePanel() {
        SetTransparency(0.5f);
    }

    private void SetTransparency(float value) {
        panelImage.color = new Color(panelImage.color.r, panelImage.color.b, panelImage.color.g, value);
        username.color = new Color(username.color.r, username.color.g, username.color.b, value);
        avatarImage.color = new Color(avatarImage.color.r, avatarImage.color.g, avatarImage.color.b, value);
        timeFrame.color = new Color(timeFrame.color.r, timeFrame.color.g, timeFrame.color.b, value);
    }

    private IEnumerator StartCountingTime_Coroutine() {
        float timeLeft = TimeOfTurn;
        timeFrame.fillAmount = 1f;

        while (timeLeft > 0) {
            timeLeft -= Time.deltaTime;
            float percent = timeLeft / TimeOfTurn;
            timeFrame.fillAmount = percent;
            timeFrame.color = UIManager.instance.timeFrameGradient.Evaluate(percent);
            yield return null;
        }

        timeFrame.fillAmount = 0f;
        if (GameManager.PlayerOnTurn.PV.IsMine) {
            if (GameManager.CanPickUpCard) {
                RPCManager.RPC("RPC_PickUpCard", RpcTarget.All);
            }
            RPCManager.RPC("RPC_ChangeTurn", RpcTarget.All);
        }
    }
}
