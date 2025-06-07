using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum Suit { 
    Hearts,
    Diamonds,
    Clubs,
    Spades
}

public class Card : MonoBehaviour {
    [Range(1, 13)]
    public int value;
    public Suit suit;
    public SpriteRenderer spriteRenderer;
    public float selectSpeed = 10f;
    public bool hovered = false;
    public bool hidden = false;
    public bool thrown = false;
    public bool CanBeThrown { get; private set; } = true;

    CardArranger cardArranger;
    Vector3 initialPosition;
    //private void OnValidate() {
    //    if (!Global.CardFaces.ContainsKey(GetValueString())) return;
    //    Initialize();
    //}

    public void MakeAvailable() {
        CanBeThrown = true;
        spriteRenderer.material.SetFloat("_ColorStrength", 0);
    }

    public void MakeUnavailable() {
        CanBeThrown = false;
        spriteRenderer.material.SetFloat("_ColorStrength", 0.5f);
    }

    public bool CheckAvailability() {
        return suit == GameManager.CurrentCard.suit || value == GameManager.CurrentCard.value || value == 11;
    }

    private void Start() {
        cardArranger = GetComponentInParent<CardArranger>();
        Initialize();
    }

    public void Throw() {
        if (!CanBeThrown) return;

        transform.parent = GameManager.instance.cardsPool;
        thrown = true;
        CanBeThrown = false;
        spriteRenderer.sortingOrder = 10 + transform.GetSiblingIndex();

        GameManager.SetCurrentCard(this);

        if(value == 11) {
            UIManager.instance.selectSuitButtons.SetActive(true);
        }

        if(cardArranger != null && cardArranger.cardsInHand.Contains(this)) cardArranger.cardsInHand.Remove(this);
    }

    private void Update() {
        initialPosition = (!thrown) ? cardArranger.GetTargetPosition(transform) : Vector3.zero;

        if(hovered) {
            Hover(1.5f, transform.localPosition.z < 0.9f, 999);
        } else {
            Hover(0, transform.localPosition.z > 0.1f, transform.GetSiblingIndex());
        }
    }

    public void Initialize() {
        Sprite sprite = Global.CardFaces[GetValueString()].sprite;
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = transform.GetSiblingIndex();
    }

    public void Initialize(string valueString) {
        if (!char.IsDigit(valueString[0])) {
            switch (valueString[0]) {
                case 'A': value = 1; break;
                case 'J': value = 11; break;
                case 'Q': value = 12; break;
                case 'K': value = 13; break;
            }
        } else {
            value = int.Parse(valueString.Substring(0, valueString.Length - 1));
        }

        foreach (Suit s in System.Enum.GetValues(typeof(Suit))) {
            if (s.ToString()[0] != valueString[1]) continue;
            suit = s;
            break;
        }

        Initialize();
    }
    
    public void Randomize() {
        value = Random.Range(1, 14);
        Suit[] allSuits = { Suit.Hearts, Suit.Diamonds, Suit.Clubs, Suit.Spades };
        suit = allSuits[Random.Range(0, allSuits.Length)];
    }

    public string GetValueString() {
        string first = value.ToString();
        if (value == 1) first = "A";
        else if(value > 10) {
            switch(value) {
                case 11: first = "J"; break;
                case 12: first = "Q"; break;
                case 13: first = "K"; break;
            }
        }

        return first + suit.ToString()[0];
    }

    private void Hover(float target, bool condition, int sortLevel) {
        Vector3 currPosition = transform.localPosition;
        Vector3 targetPosition = new Vector3(initialPosition.x, initialPosition.y, target);
        transform.localPosition = Vector3.Lerp(currPosition, targetPosition, selectSpeed * Time.deltaTime);
    }

    public static Suit StringToSuit(string suit) {
        switch(suit) {
            case "Hearts": return Suit.Hearts;
            case "Spades": return Suit.Spades;
            case "Clubs": return Suit.Clubs;
        }
        return Suit.Diamonds;
    }
}
