using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameButtonsAlphaToggler : MonoBehaviour {
    private Button button;
    private TextMeshProUGUI text;

    private static float smoothness = 10f;

    private void Start() {
        button = GetComponent<Button>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update() {
        float value = (button.interactable) ? 1f : 0.5f;
        Color target = text.color;
        target.a = value;
        text.color = Color.Lerp(text.color, target, smoothness * Time.deltaTime);
    }
}
