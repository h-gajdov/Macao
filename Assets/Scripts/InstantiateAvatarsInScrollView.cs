using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstantiateAvatarsInScrollView : MonoBehaviour {
    public GameObject avatarButtonPrefab;
    private RectTransform localRect;

    private void OnValidate() {
        if (avatarButtonPrefab == null) return;
        localRect = GetComponent<RectTransform>();

        //foreach (Transform child in transform) Destroy(child.gameObject);

        Global.Initialize();
        Debug.Log(Global.AvatarSprites.Count);
        foreach(Sprite avatar in Global.AvatarSprites) {
            Image img = Instantiate(avatarButtonPrefab, transform, false).GetComponent<Image>();
            img.sprite = avatar;
        }

        Vector2 size = new Vector2(16 * avatarButtonPrefab.GetComponent<RectTransform>().rect.width, localRect.sizeDelta.y);
        localRect.sizeDelta = size;
    }
}
