using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerPanelInGame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private Image buttonImage;
    public Image timeFrame;
    public Image avatarImage;
    private TextMeshProUGUI text;

    private static float transitionSpeed = 10f;

    private void Start() {
        buttonImage = GetComponent<Image>();
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
        Color targetColor = buttonImage.color;
        Color textTarget = text.color;
        Color avatarTarget = avatarImage.color;
        Color timeTarget = timeFrame.color;
        targetColor.a = value;
        textTarget.a = value;
        avatarTarget.a = value;
        timeTarget.a = value;

        while (Mathf.Abs(buttonImage.color.a - value) > 0.01f) {
            buttonImage.color = Color.Lerp(buttonImage.color, targetColor, transitionSpeed * Time.deltaTime);
            text.color = Color.Lerp(text.color, textTarget, transitionSpeed * Time.deltaTime);
            avatarImage.color = Color.Lerp(avatarImage.color, avatarTarget, transitionSpeed * Time.deltaTime);
            timeFrame.color = Color.Lerp(timeFrame.color, timeTarget, transitionSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
