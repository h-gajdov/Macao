using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVManager : MonoBehaviour {
    private void Start() {
        float baseWidth = 1920f;
        float baseHeight = 1080f;

        float currentAspect = (float)Screen.width / Screen.height;
        float baseAspect = baseWidth / baseHeight;

        Camera cam = Camera.main;
        cam.fieldOfView *= baseAspect / currentAspect;
    }
}
