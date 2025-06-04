using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private void OnValidate() {
        Global.Initialize();
    }

    private void Start() {
        Global.Initialize();
    }
}
