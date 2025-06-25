using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LanguagesManager : MonoBehaviour
{
    public GameObject languagesPanel;
    public Image selectedLanguageImage;
    public Transform languagesLayout;
    public static string SelectedLanguage = "English";

    private bool languagesOpen = false;

    private void Start() {
        languagesPanel.SetActive(false);
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
    }
}
