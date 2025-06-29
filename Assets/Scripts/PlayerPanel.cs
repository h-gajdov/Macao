using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerPanel : MonoBehaviour {
    public TextMeshProUGUI username;
    public Image avatarImage;
    public Image timeFrame;
    public Image medal;

    public static int TimeOfTurn = 20; //in seconds

    private void Start() {
        medal.enabled = false;
        timeFrame.fillAmount = 0f;
        //StartCoroutine(StartCountingTime());
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
