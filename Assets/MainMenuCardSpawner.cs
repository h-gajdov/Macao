using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class MainMenuCardSpawner : MonoBehaviour
{
    public GameObject cardPrefab;
    public float spawnTime = 1f;

    float prevSpawnTime;
    private void Start() {
        InvokeRepeating(nameof(Spawn), 0f, spawnTime);
        prevSpawnTime = spawnTime;
    }

    private void Update() {
        if (prevSpawnTime == spawnTime) return;

        CancelInvoke(nameof(Spawn));
        InvokeRepeating(nameof(Spawn), 0f, spawnTime);
        prevSpawnTime = spawnTime;
    }

    private void Spawn() {
        Transform card = Instantiate(cardPrefab, transform.position, Quaternion.identity).transform;
        float x = Random.Range(0, 360f);
        float y = Random.Range(0, 360f);
        float z = Random.Range(0, 360f);
        card.rotation = Quaternion.Euler(x, y, z);
        Destroy(card.gameObject, 10f);
    }
}
