using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningPanelManager : MonoBehaviour {
    public GameObject playerLeaderboardPrefab;
    public Transform content;

    public static WinningPanelManager instance;

    private void OnEnable() {
        int place = 1;
        foreach(Player player in GameManager.FinishedPlayers) {
            GameObject p = Instantiate(playerLeaderboardPrefab, content, false);
            p.GetComponent<PlayerInLeaderboard>().SetValues(player, place);
            place++;
        }

        foreach(Player player in PlayerManager.Players) {
            GameObject p = Instantiate(playerLeaderboardPrefab, content, false);
            p.GetComponent<PlayerInLeaderboard>().SetValues(player, place);
            place++;
        }
    }
}
