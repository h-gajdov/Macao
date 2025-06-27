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

    public Sprite[] medals;

    public void SetValues(Player player, int place) {
        username.text = player.username;
        avatarImage.sprite = Global.AvatarSprites[player.avatarIdx];
        if (place <= 3) {
            medal.sprite = medals[place - 1];
        } else medal.enabled = false;
    }
}
