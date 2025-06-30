using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class RegionManager : MonoBehaviour {
    public TextMeshProUGUI pingText;
    public TMP_Dropdown regionsDropdown;

    private void Start() {
        if(!PlayerPrefs.HasKey("Region")) {
            regionsDropdown.value = 0;
            ChangeRegion();
        } else {
            string region = PlayerPrefs.GetString("Region").ToUpper();
            int value = 0;
            foreach(var item in regionsDropdown.options) {
                if(item.text == region) break;
                value++;
            }
            regionsDropdown.value = value;
            ChangeRegion();
        }
    }

    private void Update() {
        if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[0])
            pingText.text = $"Ping: {PhotonNetwork.GetPing()}";
        else
            pingText.text = $"����: {PhotonNetwork.GetPing()}";
    }

    public void ChangeRegion() {
        string region = regionsDropdown.options[regionsDropdown.value].text.ToLower();

        PhotonNetwork.Disconnect();
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = region;
        PhotonNetwork.ConnectUsingSettings();
        PlayerPrefs.SetString("Region", region);
    }
}
