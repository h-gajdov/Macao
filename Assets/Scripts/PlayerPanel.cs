using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour {
    public TextMeshProUGUI username;
    public Image avatarImage;
    public Image timeFrame;

    public static int TimeOfTurn = 20; //in seconds

    private void Start() {
        StartCoroutine(StartCountingTime());
    }

    public void SetValues(Player player) {
        username.text = player.username;
        avatarImage.sprite = Global.AvatarSprites[player.avatarIdx];
        player.playerPanel = this;
    }

    private IEnumerator StartCountingTime() {
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
    }
}
