using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardArranger : MonoBehaviour {
    public GameObject cardPrefab;
    public List<Card> cardsInHand = new List<Card>();
    public float maxWidth = 5f;
    public float spaceBetweenCards = 0.1f;
    public float smoothness = 2f;
    public int numberOfCards = 10;

    Card prevSelected;
    Player player;

    public void GenerateCards() {
        for (int i = 0; i < numberOfCards; i++) {
            SpawnCard();
        }
    }

    public void SpawnCards(string[] valueStrings) {
        foreach(string card in valueStrings) {
            SpawnCard(card);
        }
    }

    public void SpawnCard(string valueString) {
        Transform card = Instantiate(cardPrefab, transform, true).transform;

        card.localPosition = Vector3.zero;
        if (true) card.localEulerAngles = Vector3.right * 90; // (90, 0, 0)
        else card.localEulerAngles = Vector3.right * -90;

        Card cl = card.GetComponent<Card>();
        cl.spriteRenderer.sortingOrder = card.GetSiblingIndex();
        cardsInHand.Add(cl);
        cl.Initialize(valueString);
    }

    public void SpawnCard() {
        Transform card = Instantiate(cardPrefab, transform, true).transform;

        card.localPosition = Vector3.zero;
        if (player.photonView.IsMine) card.localEulerAngles = Vector3.right * 90; // (90, 0, 0)
        else card.localEulerAngles = Vector3.right * -90;

        Card cl = card.GetComponent<Card>();
        cl.spriteRenderer.sortingOrder = card.GetSiblingIndex();
        cardsInHand.Add(cl);
        cl.Randomize();
    }

    //private void OnValidate() {
    //    if (!Application.isPlaying) return;

    //    cardsInHand.Clear();
    //    foreach (Transform child in transform) Destroy(child.gameObject);
    //    player = transform.parent.GetComponent<Player>();
    //}

    private void Start() {
        player = transform.parent.GetComponent<Player>();
    }
    
    private void Update() {
        if (!player.photonView.IsMine) return;
        CheckHoveredCards();
    }

    private void SpaceCards() {
        int count = transform.childCount;
        float spacing = Mathf.Min(spaceBetweenCards, maxWidth / (count - 1));
        float startX = -(spacing * (count - 1)) / 2f;

        for (int i = 0; i < count; i++) {
            Transform card = transform.GetChild(i);
            Vector3 currLocalPosition = card.localPosition;
            Vector3 targetPosition = new Vector3(startX + i * spacing, currLocalPosition.y, currLocalPosition.z);

            if (GameMath.SqrDistance(currLocalPosition, targetPosition) > 0.05f * 0.05f)
                card.localPosition = Vector3.Lerp(currLocalPosition, targetPosition, smoothness * Time.deltaTime);
            else card.localPosition = targetPosition;
        }
    }

    public Vector3 GetTargetPosition(Transform card) {
        int count = transform.childCount;
        float spacing = Mathf.Min(spaceBetweenCards, maxWidth / (count - 1));
        float startX = -(spacing * (count - 1)) / 2f;

        Vector3 currLocalPosition = card.localPosition;
        Vector3 targetPosition = new Vector3(startX + card.GetSiblingIndex() * spacing, currLocalPosition.y, currLocalPosition.z);

        return targetPosition;
    }

    private void CheckHoveredCards() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        if (hits.Length == 0) {
            if (prevSelected != null) {
                prevSelected.hovered = false;
                prevSelected = null;
            }
            return;
        }
        
        if(prevSelected != null) prevSelected.hovered = false;

        prevSelected = GetTopCardOfHits(hits);
        if(prevSelected.transform.parent == transform) { //If it is a child of the card arranger else ignore it
            prevSelected.hovered = true;

            if(Input.GetMouseButtonDown(0)) {
                prevSelected.transform.parent = GameManager.instance.cardsPool;
                prevSelected.transform.localPosition = Vector3.zero;
                //prevSelected = null;
                //prevSelected.hovered = false;
            }
        }
    }

    private Card GetTopCardOfHits(RaycastHit[] hits) {
        Card selectedCard = hits[0].transform.GetComponent<Card>();
        foreach (RaycastHit raycastHit in hits) {
            Card card = raycastHit.transform.GetComponent<Card>();
            if (card == null) continue;
            //if (selectedCard.transform.GetSiblingIndex() < card.transform.GetSiblingIndex()) selectedCard = card;
            if (selectedCard.spriteRenderer.sortingOrder < card.spriteRenderer.sortingOrder) selectedCard = card;
        }
        return selectedCard;
    }
}
