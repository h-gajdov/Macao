using Photon.Chat.Demo;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    public GameObject mainMenuContent;
    public GameObject loadingScreen;
    public GameObject loadingBar;
    public GameObject joiningRoomText;

    public Slider loadingSlider;
    public Image loadingFrame;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI loadingPlaceholderText;

    private TextMeshProUGUI joiningRoomTMP;

    public static LoadingScreenManager instance;

    private string joiningRoomTMPBase;
    private int dotCount = 0;

    private void Awake() {
        if (instance == null) instance = this;
        else {
            Destroy(this);
            return;
        }

        joiningRoomTMP = joiningRoomText.GetComponent<TextMeshProUGUI>();
        InvokeRepeating(nameof(DotAnimation), 0f, .5f);
    }

    private void Start() {
        joiningRoomTMPBase = (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[0]) ? "Joining Room" : "Вклучување во Соба";
        mainMenuContent.SetActive(true);
        loadingScreen.SetActive(false);
    }

    public void StartLoading(bool hostingGame = false) {
        mainMenuContent.SetActive(false);
        loadingScreen.SetActive(true);

        StartCoroutine(WaitForTimeout());

        if (hostingGame) joiningRoomTMPBase = (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[0]) ? "Hosting Room" : "Правење на Соба";
        joiningRoomTMP.text = joiningRoomTMPBase;
    }

    public void SetProgress(float progress) {
        loadingSlider.value = progress;
        loadingFrame.fillAmount = progress;
        progressText.text = $"{(int)(progress * 100f)}%";
    }

    private void DotAnimation() {
        dotCount = (dotCount + 1) % 4;
        string dots = new string('.', dotCount);
        joiningRoomTMP.text = joiningRoomTMPBase + dots;

        if  (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[0])
                loadingPlaceholderText.text = "Loading" + dots;
        else 
                loadingPlaceholderText.text = "Вчитување" + dots;
    }

    private IEnumerator WaitForTimeout() {
        yield return new WaitForSecondsRealtime(30f);

        if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[0])
            MainMenuManager.instance.SetError("Connection time out!");
        else 
            MainMenuManager.instance.SetError("Времето за поврзување истече!");

        mainMenuContent.SetActive(true);
        loadingScreen.SetActive(false);
        PhotonNetwork.Disconnect();
        PhotonNetwork.ConnectUsingSettings();
    }
}
