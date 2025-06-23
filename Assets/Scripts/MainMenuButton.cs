using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private Button button;
    private TextMeshProUGUI text;

    private void Start() {
        button = GetComponent<Button>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        text.color = button.colors.highlightedColor;
    }

    public void OnPointerExit(PointerEventData eventData) {
        text.color = button.colors.normalColor;
    }
}
