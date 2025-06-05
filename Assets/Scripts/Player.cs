using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public PhotonView photonView;

    private void Start() {
        if (photonView.IsMine) GameManager.Players.Insert(0, this);
        else GameManager.Players.Add(this);

        GameManager.AssignPositions();
    }
}