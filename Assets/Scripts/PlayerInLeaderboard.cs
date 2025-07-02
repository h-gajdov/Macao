using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInLeaderboard : MonoBehaviour
{
    public TextMeshProUGUI username;
    public Image avatarImage;
    public Image medal;
    public GameObject readyTick;

    public void SetValues(Player player, int place) {
        if (player == null) return;
        username.text = player.username;
        avatarImage.sprite = Global.AvatarSprites[player.avatarIdx];
        if (place <= 3) {
            medal.sprite = UIManager.instance.medals[place - 1];
        } else medal.enabled = false;
        player.playerInLeaderboard = this;
    }
}
