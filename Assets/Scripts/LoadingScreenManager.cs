using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    private string joiningRoomTMPBase = "Joining Room";
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
        mainMenuContent.SetActive(true);
        loadingScreen.SetActive(false);
    }

    public void StartLoading(bool hostingGame = false) {
        mainMenuContent.SetActive(false);
        loadingScreen.SetActive(true);

        if (hostingGame) joiningRoomTMPBase = "Hosting Room";
        joiningRoomTMP.text = joiningRoomTMPBase;
    }

    public void SetProgress(float progress) {
        loadingSlider.value = progress;
        loadingFrame.fillAmount = progress;
        progressText.text = $"{progress * 100f}%";
    }

    private void DotAnimation() {
        dotCount = (dotCount + 1) % 4;
        string dots = new string('.', dotCount);
        joiningRoomTMP.text = joiningRoomTMPBase + dots;
        loadingPlaceholderText.text = "Loading" + dots;
    }
}
