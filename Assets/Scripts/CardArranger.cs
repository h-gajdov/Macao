using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardArranger : MonoBehaviour {
    public GameObject cardPrefab;
    public List<Card> cardsInHand = new List<Card>();
    public float smoothness = 2f;
    public int numberOfCards = 10;

    [Space]
    [Header("Cards Placement Parameters")]
    public float spaceBetweenCards = 0.1f;
    public float baseRadius = 5f;
    public float maxWidth = 5f;

    Card prevSelected;
    [HideInInspector] public Player player;
    bool allUnavailable = false; //TODO: Delete this when moving CheckAvailability out of Update

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

    public void SpawnCards(CardData[] cardData) {
        foreach(CardData data in cardData) {
            SpawnCard(data);
        }
    }

    private Card SetInitialTransformOfCard(Transform card) {
        card.localPosition = Vector3.zero;

        Card cl = card.GetComponent<Card>();
        cl.spriteRenderer.sortingOrder = 10 + card.GetSiblingIndex();
        if(card.IsChildOf(transform)) cardsInHand.Add(cl);

        cl.hidden = !player.PV.IsMine;

        return cl;
    }

    public Card SpawnCard(string valueString, Transform parent) {
        Transform card = Instantiate(cardPrefab, parent, true).transform;
        Card cl = SetInitialTransformOfCard(card);
        cl.Initialize(valueString);
        return cl;
    }

    public Card SpawnCard(string valueString) {
        Transform card = Instantiate(cardPrefab, transform, true).transform;
        Card cl = SetInitialTransformOfCard(card);
        cl.Initialize(valueString);
        return cl;
    }

    public Card SpawnCard(CardData data, Transform parent) {
        Transform card = Instantiate(cardPrefab, parent, true).transform;
        Card cl = SetInitialTransformOfCard(card);
        cl.Initialize(data);
        return cl;
    }

    public Card SpawnCard(CardData data) {
        Transform card = Instantiate(cardPrefab, transform, true).transform;
        Card cl = SetInitialTransformOfCard(card);
        cl.Initialize(data);
        return cl;
    }

    public Card SpawnCard() {
        Transform card = Instantiate(cardPrefab, transform, true).transform;
        Card cl = SetInitialTransformOfCard(card);
        cl.Randomize();
        return cl;
    }

    public CardDataArrayWrapper GetCardDatas() {
        CardData[] result = new CardData[cardsInHand.Count];
        int i = 0;
        foreach (Card card in cardsInHand) {
            result[i++] = card.data;
        }
        return new CardDataArrayWrapper(result);
    }

    private void Start() {
        player = transform.parent.GetComponent<Player>();
    }
    
    private void Update() {
        if (!player.PV.IsMine) {
            ArrangeOpponentCards(cardsInHand, transform, baseRadius);
            return;
        }

        CheckHoveredCards();
        if(!allUnavailable && GameManager.CurrentCard != null) SetAvailabilityOfCards(); //TODO: Remove this and make it check when your turn comes
    }

    public void EnableCards() {
        allUnavailable = false;
    }

    public void DisableAllCards() {
        foreach (Card cl in cardsInHand) {
            cl.MakeUnavailable();
        }
        allUnavailable = true;
    }

    private void SetAvailabilityOfCards() {
        foreach (Card cl in cardsInHand) {
            if (cl.CheckAvailability()) cl.MakeAvailable();
            else cl.MakeUnavailable();
        }
    }

    private void SpaceCards() {
        int count = transform.childCount;
        float spacing = Mathf.Min(spaceBetweenCards, maxWidth / (count - 1));
        float startX = -(spacing * (count - 1)) / 2f;

        int i = 0;
        foreach(Card cl in cardsInHand) {
            Transform card = cl.transform;
            Vector3 currLocalPosition = card.localPosition;
            Vector3 targetPosition = new Vector3(startX + i++ * spacing, currLocalPosition.y, currLocalPosition.z);

            if (GameMath.SqrDistance(currLocalPosition, targetPosition) > 0.05f * 0.05f)
                card.localPosition = Vector3.Lerp(currLocalPosition, targetPosition, smoothness * Time.deltaTime);
            else card.localPosition = targetPosition;
        }
    }

    public void ArrangeOpponentCards(List<Card> cards, Transform centerPoint, float radius) {
        int count = cards.Count;
        if (count == 0) return;
        if(count == 1) {
            Vector3 currPosition = cards[0].transform.localPosition;
            Vector3 targetPosition = Vector3.forward * baseRadius;
            cards[0].transform.localPosition = Vector3.Lerp(currPosition, targetPosition, 10 * Time.deltaTime);
            cards[0].transform.localRotation = Quaternion.Lerp(cards[0].transform.localRotation, Quaternion.identity, 10 * Time.deltaTime);
            return;
        }

        float spacing = Mathf.Min(spaceBetweenCards, maxWidth / (count - 1));
        float targetWidth = Mathf.Min(spacing * (count - 1), maxWidth);
        float halfChord = targetWidth / 2f;
        float angleRad = 2f * Mathf.Asin(halfChord / radius);
        float angleRange = angleRad * Mathf.Rad2Deg;
        float angleStep = angleRange / (count - 1);
        float startAngle = -angleRange / 2f;

        Vector3 arcAxis = Vector3.Cross(centerPoint.forward, centerPoint.right);
        if (arcAxis == Vector3.zero)
            arcAxis = Vector3.Cross(centerPoint.forward, Vector3.up);

        for (int i = 0; i < count; i++) {
            float angle = startAngle + angleStep * i;
            float rad = Mathf.Deg2Rad * angle;

            Quaternion rotation = Quaternion.AngleAxis(angle, arcAxis.normalized);
            Vector3 offsetDirection = rotation * centerPoint.forward;

            //Vector3 currPosition = cards[i].transform.position;
            //Vector3 targetPosition = centerPoint.position + offsetDirection.normalized * radius;
            //cards[i].transform.position = Vector3.Lerp(currPosition, targetPosition, 10 * Time.deltaTime);
            //cards[i].transform.LookAt(centerPoint.position);
            //cards[i].transform.Rotate(0f, 180f, 0f);
            // World-space calculations for uniformity

            Vector3 targetPos = centerPoint.position + offsetDirection * radius;
            Quaternion targetRot = Quaternion.LookRotation(centerPoint.position - targetPos) * Quaternion.Euler(0, 180, 0);

            // Smooth motion
            cards[i].transform.position = Vector3.Lerp(cards[i].transform.position, targetPos, 10f * Time.deltaTime);
            cards[i].transform.rotation = Quaternion.Lerp(cards[i].transform.rotation, targetRot, 10f * Time.deltaTime);
        }
    }

    public Vector3 GetTargetPosition(Transform card) {
        int count = transform.childCount;
        float spacing = Mathf.Min(spaceBetweenCards, maxWidth / (count - 1));
        float startX = -(spacing * (count - 1)) / 2f;

        Vector3 currLocalPosition = card.localPosition;
        Vector3 targetPosition = Vector3.right * (startX + card.GetSiblingIndex() * spacing);

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
        if(prevSelected != null && prevSelected.transform.parent == transform) { //If it is a child of the card arranger else ignore it
            prevSelected.hovered = true;

            if(Input.GetMouseButtonDown(0)) {
                if (GameManager.PlayerOnTurn != player) {
                    Debug.LogError("It's not your turn!");
                    return;
                }
                if (!prevSelected.CanBeThrown || !Card.CanThrowCards) return;
                player.PV.RPC("RPC_Throw", RpcTarget.AllBuffered, cardsInHand.IndexOf(prevSelected));
            }
        }
    }

    private Card GetTopCardOfHits(RaycastHit[] hits) {
        Card selectedCard = hits[0].transform.GetComponent<Card>();
        if (selectedCard == null) return null;

        foreach (RaycastHit raycastHit in hits) {
            Card card = raycastHit.transform.GetComponent<Card>();
            if (card == null) continue;
            //if (selectedCard.transform.GetSiblingIndex() < card.transform.GetSiblingIndex()) selectedCard = card;
            if (selectedCard.spriteRenderer.sortingOrder < card.spriteRenderer.sortingOrder) selectedCard = card;
        }
        return selectedCard;
    }

    public void ReturnCards() {
        List<string> tmp = new List<string>();

        while (CardStackManager.UndealtCards.Count > 0) {
            tmp.Add(CardStackManager.UndealtCards.Pop());
        }

        for (int i = cardsInHand.Count - 1; i >= 0; i--) {
            CardStackManager.UndealtCards.Push(cardsInHand[i].GetValueString());
        }

        for (int i = tmp.Count - 1; i >= 0; i--) {
            CardStackManager.UndealtCards.Push(tmp[i]);
        }
    }

    public bool Contains(int value) {
        foreach(Card card in cardsInHand) {
            if (card.data.value == value) return true;
        }
        return false;
    }

    public bool Contains(Suit suit) {
        foreach (Card card in cardsInHand) {
            if (card.data.suit == suit) return true;
        }
        return false;
    }
}
