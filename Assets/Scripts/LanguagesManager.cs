using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class LanguagesManager : MonoBehaviour
{
    public GameObject languagesPanel;
    public Image selectedLanguageImage;
    public Transform languagesLayout;
    public static string SelectedLanguage = "English";

    private bool languagesOpen = false;

    private void Start() {
        languagesPanel.SetActive(false);
        if (!PlayerPrefs.HasKey("Language"))
            ChangeLanguage(0);
        else {
            SelectedLanguage = PlayerPrefs.GetString("SelectedLanguageHash");
            selectedLanguageImage.sprite = Global.CountryFlags[SelectedLanguage];
            ChangeLanguage(PlayerPrefs.GetInt("Language"));
        }
    }

    public void OpenLanguages() {
        languagesOpen = !languagesOpen;
        if(!languagesOpen) {
            languagesPanel.SetActive(false);
            return;
        }

        languagesPanel.SetActive(true);
        Transform selected = null;
        foreach(Transform child in languagesLayout) {
            if (!child.gameObject.name.StartsWith(SelectedLanguage)) continue;
            selected = child;
            break;
        }
        selected.SetAsLastSibling();
    }

    public void SelectLanguage(TextMeshProUGUI text) {
        SelectedLanguage = text.text;
        languagesPanel.SetActive(false);
        selectedLanguageImage.sprite = Global.CountryFlags[SelectedLanguage];
        languagesOpen = false;
        PlayerPrefs.SetString("SelectedLanguageHash", SelectedLanguage);
    }

    public void ChangeLanguage(int index) {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        PlayerPrefs.SetInt("Language", index);
    }
}
