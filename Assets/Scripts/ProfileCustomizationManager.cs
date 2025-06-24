using System.Collections;
using System.Collections.Generic;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ProfileCustomizationManager : MonoBehaviour {
    public Image arrowImage;
    public Image buttonImage;
    public Image avatar;
    public RectTransform profileCustomizationPanel;
    public GameObject avatarsScrollView;
    public float smoothness = 10f;

    private MainMenuTransparentButton transparentButton;
    private RectTransform localRect;
    private bool isOpen = false;

    private void Start() {
        localRect = GetComponent<RectTransform>();
        transparentButton = GetComponentInChildren<MainMenuTransparentButton>();
        avatarsScrollView.SetActive(false);

        localRect.anchoredPosition = Vector2.up * profileCustomizationPanel.rect.height;
        isOpen = false;
        arrowImage.transform.eulerAngles = Vector3.zero;
        transparentButton.enabled = true;
    }

    private void Update() {
        arrowImage.color = buttonImage.color;
    }

    public void OpenAvatarsView() {
        avatarsScrollView.SetActive(true);
    }

    public void SelectAvatar() {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        Image selectedAvatar = clickedButton.GetComponent<Image>();
        avatar.sprite = selectedAvatar.sprite;
        avatarsScrollView.SetActive(false);
    }

    public void OpenOrCloseProfileCustomization() {
        StopAllCoroutines();
        if(!isOpen) {
            StartCoroutine(SetPosition(Vector2.zero));
            arrowImage.transform.eulerAngles = Vector3.forward * 180f;
        } else {
            StartCoroutine(SetPosition(Vector2.up * profileCustomizationPanel.rect.height));
            arrowImage.transform.eulerAngles = Vector3.zero;
        }
        isOpen = !isOpen;
        transparentButton.enabled = !isOpen;
    }

    private IEnumerator SetPosition(Vector2 target) {
        while(localRect.anchoredPosition != target) {
            localRect.anchoredPosition = Vector3.Lerp(localRect.anchoredPosition, target, smoothness * Time.deltaTime);
            yield return null;
        }
    }
}
