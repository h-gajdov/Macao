using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardArranger : MonoBehaviour {
    public GameObject cardPrefab;
    public List<Card> cardsInHand = new List<Card>();
    public float maxWidth = 5f;
    public float spaceBetweenCards = 0.1f;
    public int numberOfCards = 10;

    public void GenerateCards() {
        float tmp = 0;
        for (int i = 0; i < numberOfCards; i++) {
            Transform card = Instantiate(cardPrefab, transform, true).transform;

            card.localPosition = Vector3.zero;
            card.localPosition = new Vector3(0, 0, tmp);
            card.eulerAngles = Vector3.right * 90; // (90, 0, 0)

            //Vector3 currScale = card.localScale;
            //card.localScale = new Vector3(currScale.x / 2, currScale.y / 2, currScale.z);

            Card cl = card.GetComponent<Card>();
            cl.spriteRenderer.sortingOrder = card.GetSiblingIndex();
            cardsInHand.Add(cl);
            cl.Randomize();
        }
    }

    private void OnValidate() {
        if (!Application.isPlaying) return;

        cardsInHand.Clear();
        foreach (Transform child in transform) Destroy(child.gameObject);

        GenerateCards();
    }

    private void Start() {
        GenerateCards();
    }
    
    private void Update() {
        int count = transform.childCount;
        float spacing = Mathf.Min(spaceBetweenCards, maxWidth / (count - 1));
        float startX = -(spacing * (count - 1)) / 2f;

        for (int i = 0; i < count; i++) {
            Transform card = transform.GetChild(i);
            Vector3 currLocalPosition = card.localPosition;
            card.localPosition = new Vector3(startX + i * spacing, currLocalPosition.y, currLocalPosition.z);
        }
    }
}
