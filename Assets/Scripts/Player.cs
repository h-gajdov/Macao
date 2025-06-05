using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public PhotonView photonView;
    public CardArranger cardArranger;

    private void Start() {
        cardArranger = GetComponentInChildren<CardArranger>();

        if (photonView.IsMine) GameManager.Players.Insert(0, this);
        else GameManager.Players.Add(this);

        GameManager.AssignPositions();
    }
}