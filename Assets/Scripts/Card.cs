using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Suit { 
    Hearts,
    Diamonds,
    Clubs,
    Spades,
    All
}

[System.Serializable]
public class CardData {
    [Range(1, 15)]
    public int value;
    public Suit suit;

    public CardData() {
        value = 1;
        suit = Suit.Hearts;
    }

    public CardData(int value, Suit suit) {
        this.value = value;
        this.suit = suit;
    }

    public static CardData ConvertValueStringToCardData(string valueString) {
        CardData data = new CardData();

        if (valueString == "BJ") {
            data.value = 14;
            data.suit = Suit.All;
            return data;
        } else if (valueString == "RJ") {
            data.value = 15;
            data.suit = Suit.All;
            return data;
        }

        if (!char.IsDigit(valueString[0])) {
            switch (valueString[0]) {
                case 'A': data.value = 1; break;
                case 'J': data.value = 11; break;
                case 'Q': data.value = 12; break;
                case 'K': data.value = 13; break;
            }
        } else {
            data.value = int.Parse(valueString.Substring(0, valueString.Length - 1));
        }

        foreach (Suit s in System.Enum.GetValues(typeof(Suit))) {
            if (s.ToString()[0] != valueString[valueString.Length - 1]) continue;
            data.suit = s;
            break;
        }
        return data;
    }
}

[System.Serializable]
public class CardDataArrayWrapper {
    public CardData[] cardDatas;

    public CardDataArrayWrapper(CardData[] cardDatas) {
        this.cardDatas = cardDatas;
    }
}

public class Card : MonoBehaviour {
    public CardData data;
    public Player thrownByPlayer;
    public SpriteRenderer spriteRenderer;
    public float selectSpeed = 10f;
    public bool hovered = false;
    public bool hidden = false;
    public bool thrown = false;
    public bool CanBeThrown { get; private set; } = true;
    public static bool CanThrowCards = true;

    CardArranger cardArranger;
    Vector3 initialPosition;
    Vector3 initialRotation;
    bool soundPlayed = false;

    private void OnValidate() {
        if (!Global.CardFaces.ContainsKey(GetValueString())) return;
        Initialize();
    }

    public void MakeAvailable() {
        CanBeThrown = true;
        spriteRenderer.material.SetFloat("_ColorStrength", 0);
    }

    public void MakeUnavailable() {
        CanBeThrown = false;
        spriteRenderer.material.SetFloat("_ColorStrength", 0.5f);
    }

    public bool CheckAvailability() {
        if (CardStackManager.PoolOfForcedPickup != 0)
            return data.value == 7 || data.value == 14 || data.value == 15;

        return data.value == 14 || data.value == 15 || GameManager.CurrentCard.data.suit == Suit.All || data.suit == GameManager.CurrentCard.data.suit || data.value == GameManager.CurrentCard.data.value || data.value == 11;
    }

    private void Start() {
        cardArranger = GetComponentInParent<CardArranger>();
        Initialize();
    }

    public IEnumerator Throw(Player player) {
        if (GameManager.GameHasFinished) yield break;

        AudioManager.Play("ThrowingCard");

        thrownByPlayer = player;
        transform.parent = GameManager.instance.cardsPool;
        CanBeThrown = false;
        thrown = true;
        hidden = false;
        spriteRenderer.sortingOrder = transform.GetSiblingIndex();

        initialRotation = Vector3.right * 90f;
        initialRotation.z = Random.Range(0, 360f);
        GameManager.CardPoolList.Add(this);

        if (cardArranger != null && cardArranger.cardsInHand.Contains(this)) {
            cardArranger.cardsInHand.Remove(this);
            if (cardArranger.cardsInHand.Count == 1) {
                CanThrowCards = false;
                yield return UIManager.instance.WaitForLastCardButtonPress();
                CanThrowCards = true;
            }

            if (data.value == 11) {
                GameManager.SetPendingCard(this);
                if (player.PV.IsMine) UIManager.instance.selectSuitButtons.SetActive(true);
                yield break;
            } else if(data.value == 8 || data.value == 1) {
                GameManager.ChangeTurn(false);
            }

            GameManager.SetCurrentCard(this);
            GameManager.ChangeTurn();


            if (data.value == 7) {
                SevensAndJokersLogic(2);
            } else if (data.value == 14 || data.value == 15) {
                SevensAndJokersLogic(4);
            }

            if (cardArranger.cardsInHand.Count == 0) player.Finish();
        }

        GameManager.SetCurrentCard(this);
    }

    private IEnumerator RandomSpinWhenThrowing() {
        float throwDuration = 0.25f;
        float elapsed = 0f;

        Vector3 startPos = transform.localPosition;
        Vector3 targetPos = Vector3.zero;

        float spinSpeed = 720f;

        float startX = transform.localEulerAngles.x;
        float targetX = 90f;
        float currentZRotation = transform.localEulerAngles.z;

        while (elapsed < throwDuration) {
            elapsed += Time.deltaTime;
            float t = elapsed / throwDuration;

            transform.localPosition = Vector3.Lerp(startPos, targetPos, t);

            float currentX = Mathf.LerpAngle(startX, targetX, t * selectSpeed / 4);
            currentZRotation += spinSpeed * Time.deltaTime;

            transform.localRotation = Quaternion.Euler(currentX, 0f, currentZRotation);

            yield return null;
        }
    }

    private void SevensAndJokersLogic(int amount) {
        CardStackManager.PoolOfForcedPickup += amount;
        if (!GameManager.PlayerOnTurn.cardArranger.Contains(7) &&
            !GameManager.PlayerOnTurn.cardArranger.Contains(14) &&
            !GameManager.PlayerOnTurn.cardArranger.Contains(15))
            CardStackManager.PickUpCardsFromPoolOfForcedPickup();
        else
            UIManager.instance.takeCards.gameObject.SetActive(GameManager.PlayerOnTurn.PV.IsMine);
    }

    private void Update() {
        if (!thrown) {
            spriteRenderer.sortingOrder = 100 + transform.GetSiblingIndex();
            if (cardArranger == null || !cardArranger.player.PV.IsMine) return;
        }

        initialPosition = (!thrown) ? cardArranger.GetTargetPosition(transform) : Vector3.zero;

        if (hovered) {
            if (!soundPlayed) AudioManager.Play("CardHover");
            Hover(1.5f);
            soundPlayed = true;
        } else {
            Hover(0);
            soundPlayed = false;
        }

        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(initialRotation), selectSpeed * Time.deltaTime);
        if(thrown) spriteRenderer.sortingOrder = transform.GetSiblingIndex();
    }

    public void Initialize() {
        Sprite sprite = Global.CardFaces[GetValueString()].sprite;
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = transform.GetSiblingIndex();
        initialRotation = (hidden) ? Vector3.right * -90 : Vector3.right * 90;
    }

    public void Initialize(CardData card) {
        data = card;
        Initialize();
    }

    public void Initialize(string valueString) {
        data = CardData.ConvertValueStringToCardData(valueString);
        Initialize();
    }
    
    public void Randomize() {
        data.value = Random.Range(1, 14);
        Suit[] allSuits = { Suit.Hearts, Suit.Diamonds, Suit.Clubs, Suit.Spades };
        data.suit = allSuits[Random.Range(0, allSuits.Length)];
    }

    public string GetValueString() {
        string first = data.value.ToString();
        if (data.value == 1) first = "A";
        else if(data.value > 10) {
            switch(data.value) {
                case 11: first = "J"; break;
                case 12: first = "Q"; break;
                case 13: first = "K"; break;
                case 14: return "BJ";
                case 15: return "RJ";
            }
        }

        return first + data.suit.ToString()[0];
    }

    private void Hover(float target) {
        Vector3 currPosition = transform.localPosition;
        Vector3 targetPosition = new Vector3(initialPosition.x, initialPosition.y, target);
        transform.localPosition = Vector3.Lerp(currPosition, targetPosition, selectSpeed * Time.deltaTime);
        spriteRenderer.sortingOrder = 100 + transform.GetSiblingIndex();
    }

    public static Suit StringToSuit(string suit) {
        switch(suit) {
            case "Hearts": return Suit.Hearts;
            case "Spades": return Suit.Spades;
            case "Clubs": return Suit.Clubs;
            case "Diamonds": return Suit.Diamonds;
        }
        return Suit.All;
    }
}
