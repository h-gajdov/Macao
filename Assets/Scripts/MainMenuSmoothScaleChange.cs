using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuSmoothScaleChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public float scaleMultiplier = 1.25f;
    public float smoothness = 5f;

    public void OnPointerEnter(PointerEventData eventData) {
        StopAllCoroutines();
        StartCoroutine(SmoothScale(Vector3.one * scaleMultiplier));
    }

    public void OnPointerExit(PointerEventData eventData) {
        StopAllCoroutines();
        StartCoroutine(SmoothScale(Vector3.one));
    }

    private IEnumerator SmoothScale(Vector3 scale) {
        while(transform.localScale != scale) {
            transform.localScale = Vector3.Lerp(transform.localScale, scale, smoothness * Time.deltaTime);
            yield return null;
        }
    }
}
