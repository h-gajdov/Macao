using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RegionManager : MonoBehaviour {
    public TextMeshProUGUI pingText;
    public TMP_Dropdown regionsDropdown;

    private void Update() {
        pingText.text = $"Ping: {PhotonNetwork.GetPing()}";
    }

    public void ChangeRegion(int index) {
        string region = regionsDropdown.options[regionsDropdown.value].text.ToLower();

        PhotonNetwork.Disconnect();
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = region;
        PhotonNetwork.ConnectUsingSettings();
    }
}
