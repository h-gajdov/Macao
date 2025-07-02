using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class ProfileCustomizationManager : MonoBehaviour {
    public Image arrowImage;
    public Image buttonImage;
    public Image avatar;
    public RectTransform profileCustomizationPanel;
    public TMP_InputField usernameInput;
    public GameObject avatarsScrollView;
    public float smoothness = 10f;

    private MainMenuTransparentButton transparentButton;
    private RectTransform localRect;
    private bool isOpen = false;

    private void Start() {
        localRect = GetComponent<RectTransform>();
        transparentButton = GetComponentInChildren<MainMenuTransparentButton>();
        avatarsScrollView.SetActive(false);

        InitializeValues();

        localRect.anchoredPosition = Vector2.up * profileCustomizationPanel.rect.height;
        isOpen = false;
        arrowImage.transform.eulerAngles = Vector3.zero;
        transparentButton.enabled = true;
    }

    private void Update() {
        arrowImage.color = buttonImage.color;
    }

    public void Randomize() {
        SetUsername(UsernameGenerator.GenerateUsername());
        SetAvatarImage(Random.Range(0, Global.AvatarSprites.Count));
    }

    public void OpenAvatarsView() {
        avatarsScrollView.SetActive(true);
    }

    public void SelectAvatar() {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        Image selectedAvatar = clickedButton.GetComponent<Image>();
        SetAvatarImage(Global.AvatarSprites.IndexOf(selectedAvatar.sprite));
        avatarsScrollView.SetActive(false);
    }

    public void OpenOrCloseProfileCustomization() {
        StopAllCoroutines();
        if(!isOpen) {
            StartCoroutine(SetPosition(Vector2.zero));
            arrowImage.transform.eulerAngles = Vector3.forward * 180f;
        } else {
            StartCoroutine(SetPosition(Vector2.up * profileCustomizationPanel.rect.height));
            arrowImage.transform.eulerAngles = Vector3.zero;
        }
        isOpen = !isOpen;
        transparentButton.enabled = !isOpen;
    }

    private IEnumerator SetPosition(Vector2 target) {
        while(localRect.anchoredPosition != target) {
            localRect.anchoredPosition = Vector3.Lerp(localRect.anchoredPosition, target, smoothness * Time.deltaTime);
            yield return null;
        }
    }

    private void InitializeValues() {
        if (!PlayerPrefs.HasKey("Username"))
            SetUsername(UsernameGenerator.GenerateUsername());
        else SetUsername(PlayerPrefs.GetString("Username"));

        if (!PlayerPrefs.HasKey("AvatarIndex"))
            SetAvatarImage(Random.Range(0, Global.AvatarSprites.Count));
        else SetAvatarImage(PlayerPrefs.GetInt("AvatarIndex"));
    }

    private void SetAvatarImage(int index) {
        avatar.sprite = Global.AvatarSprites[index];
        PlayerPrefs.SetInt("AvatarIndex", index);
        PlayerPrefs.Save();
    }

    public void SetUsername() {
        SetUsername(usernameInput.text);
    }

    private void SetUsername(string username) {
        if(username.Length < 3) {
            if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[0])
                MainMenuManager.instance.SetError("Your username should containt at least 3 and a maximum of 23 characters");
            else
                MainMenuManager.instance.SetError("Вашето корисничко име треба да содржи најмалку 3 и максимум 23 знаци");
            usernameInput.text = PlayerPrefs.GetString("Username");
            return;
        }

        usernameInput.text = username;
        PlayerPrefs.SetString("Username", username);
        PlayerPrefs.Save();
    }
}
