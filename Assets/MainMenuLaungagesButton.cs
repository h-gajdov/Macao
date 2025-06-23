using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuLaungagesButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private Button button;

    private static float transitionSpeed = 10f;

    private void Start() {
        button = GetComponent<Button>();
        StartCoroutine(SetColor(Color.white));
    }

    public void OnPointerEnter(PointerEventData eventData) {
        StopAllCoroutines();
        StartCoroutine(SetColor(button.colors.highlightedColor));
    }

    public void OnPointerExit(PointerEventData eventData) {
        StopAllCoroutines();
        StartCoroutine(SetColor(Color.white));
    }

    private IEnumerator SetColor(Color color) {
        Image image = transform.GetChild(0).GetComponent<Image>();//GetComponentInChildren<Image>();
        while (image.color != color) {
            image.color = Color.Lerp(image.color, color, transitionSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
