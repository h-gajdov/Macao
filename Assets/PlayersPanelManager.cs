using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayersPanelManager : MonoBehaviour
{
    public static PlayersPanelManager instance;

    private void Awake() {
        if (instance == null) instance = this;
        else {
            Destroy(this);
            return;
        }
    }

    public static void SetValues(int startIdx) {
        int count = PlayerManager.Players.Count;

        if(count == 2) {
            Player other = (!PlayerManager.Players[0].PV.IsMine) ? PlayerManager.Players[0] : PlayerManager.Players[1];
            other.playerPanel = instance.transform.GetChild(2).GetComponent<PlayerPanel>();
            other.playerPanel.SetValues(other);
            other.playerPanel.gameObject.SetActive(true);
        } else {
            int childIdx = ((-startIdx % 4) + 4) % 4;
            int[] characterIndices = PlayerManager.ActiveCharacterObjectsIndex[count - 1];
            foreach (Player player in PlayerManager.Players) {
                if (childIdx == 0) {
                    childIdx = (childIdx + 1) % 4;
                    continue;
                }
                player.playerPanel = instance.transform.GetChild(childIdx).GetComponent<PlayerPanel>();
                player.playerPanel.SetValues(player);
                player.playerPanel.gameObject.SetActive(true);
                childIdx = (childIdx + 1) % 4;
            }
        }

        foreach (Player player in PlayerManager.Players) {
            if(player.PV.IsMine) {
                UIManager.instance.localPlayerPanel.SetValues(player);
                break;
            }
        }
    }
}
