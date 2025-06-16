using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStackCubeHover : MonoBehaviour {
    public float smoothness = 5f;
    public Color selectedColor;

    private new Renderer renderer;

    private void Awake() {
        renderer = GetComponent<Renderer>();
    }

    private void OnMouseEnter() {
        StopAllCoroutines();
        StartCoroutine(LerpToValue(0.25f));
    }

    private void OnMouseExit() {
        StopAllCoroutines();
        StartCoroutine(LerpToValue(0));
    }

    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(0) && CardStackManager.UndealtCards.Count > 0 && GameManager.PlayerOnTurn == GameManager.LocalPlayer) {
            GameManager.PV.RPC("RPC_PickUpCard", RpcTarget.AllBuffered);
        }
    }

    private IEnumerator LerpToValue(float target) {
        Material material = renderer.sharedMaterial;
        float colorStrength = material.GetFloat("_ColorStrength");
        Debug.Log(colorStrength);
        while (colorStrength != target) {
            colorStrength = Mathf.Lerp(colorStrength, target, smoothness * Time.deltaTime);
            material.SetFloat("_ColorStrength", colorStrength);
            yield return null;
        }
    }
}
