using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class CardsInHowToPlay : MonoBehaviour {
    public float addition = 1f;
    public Transform container;
    private RectTransform rect;

    private void Start() {
        rect = container.GetComponent<RectTransform>();
    }

    private void FixedUpdate() {
        if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[0]) {
            rect.localPosition = Vector3.zero;
        } else {
            rect.localPosition = Vector3.up * addition;
        }
    }
}
