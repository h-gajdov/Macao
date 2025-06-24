using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuTransparentButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private Image image;
    private TextMeshProUGUI text;

    private static float transitionSpeed = 10f;

    private void Start() {
        image = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();

        StartCoroutine(SetTransparency(0.5f));
    }

    public void OnPointerEnter(PointerEventData eventData) {
        StopAllCoroutines();
        StartCoroutine(SetTransparency(1f));
    }

    public void OnPointerExit(PointerEventData eventData) {
        StopAllCoroutines();
        StartCoroutine(SetTransparency(0.5f));
    }

    private IEnumerator SetTransparency(float value) {
        Color targetColor = image.color;
        bool hasText = text != null;

        targetColor.a = value;
        while (Mathf.Abs(image.color.a - value) > 0.01f) {
            image.color = Color.Lerp(image.color, targetColor, transitionSpeed * Time.deltaTime);
            if (hasText) {
                Color textTarget = text.color;
                textTarget.a = value;
                text.color = Color.Lerp(text.color, textTarget, transitionSpeed * Time.deltaTime);
            }
            yield return null;
        }
    }
}
