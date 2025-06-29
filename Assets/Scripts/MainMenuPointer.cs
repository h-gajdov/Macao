using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuPointer : MonoBehaviour {
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    private GameObject prevHoveredObject;

    private void Update() {
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        if(results.Count == 0) {
            prevHoveredObject = null;
            return;
        }
        
        foreach (var result in results) {
            GameObject curr = result.gameObject;
            if (prevHoveredObject == curr) break;

            if (curr.GetComponent<Button>() != null && prevHoveredObject != curr) {
                AudioManager.Play("ButtonHoverSound");
                prevHoveredObject = curr;
                break;
            }
        }

        if (prevHoveredObject == null) return;

        if(Input.GetMouseButtonDown(0)) {
            AudioManager.Play("ButtonClickSound");
        }
    }
}
