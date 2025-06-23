using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuTransparentButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private Image image;

    private static float transitionSpeed = 10f;

    private void Start() {
        image = GetComponent<Image>();
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
        targetColor.a = value;
        while (image.color.a != value) {
            image.color = Color.Lerp(image.color, targetColor, transitionSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
