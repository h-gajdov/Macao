using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class MainMenuCardSpawner : MonoBehaviour
{
    public GameObject cardPrefab;
    public float spawnTime = 1f;
    public float positionOffset = 0.1f;

    float prevSpawnTime;
    private void Start()
    {
        Global.Initialize();
        InvokeRepeating(nameof(Spawn), 0f, spawnTime);
        prevSpawnTime = spawnTime;
    }

    private void Update()
    {
        if (prevSpawnTime == spawnTime) return;

        CancelInvoke(nameof(Spawn));
        InvokeRepeating(nameof(Spawn), 0f, spawnTime);
        prevSpawnTime = spawnTime;
    }

    private void Spawn()
    {
        Vector3 target = transform.position;
        target.x += Random.Range(-positionOffset, positionOffset);
        target.z += Random.Range(-positionOffset, positionOffset);
        Transform card = Instantiate(cardPrefab, target, Quaternion.identity).transform;
        card.parent = transform;

        int cardIdx = Random.Range(0, Global.AllCardStrings.Count);
        string cardStr = Global.AllCardStrings[cardIdx];
        card.GetComponent<Renderer>().material.SetTexture("_MainTex", Global.CardFaces[cardStr].texture);

        float x = Random.Range(0, 360f);
        float y = Random.Range(0, 360f);
        float z = Random.Range(0, 360f);
        card.rotation = Quaternion.Euler(x, y, z);
        Destroy(card.gameObject, 10f);
    }
}
