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

    Vector3 initialPosition;
    
    private void OnValidate() {
        if (!Global.CardFaces.ContainsKey(GetValueString())) return;
        spriteRenderer.sharedMaterial.SetTexture("_FrontTexture", Global.CardFaces[GetValueString()]);
    }

    private void Start() {
        spriteRenderer.material.SetTexture("_FrontTexture", Global.CardFaces[GetValueString()]);
    }

    private void Update() {
        initialPosition = GetComponentInParent<CardArranger>().GetTargetPosition(transform);
        if(hovered) {
            Hover(1, transform.localPosition.z < 0.9f, 999);
        } else {
            Hover(0, transform.localPosition.z > 0.1f, transform.GetSiblingIndex());
        }
    }

    public void UpdateImage() {
        spriteRenderer.material.SetTexture("_FrontTexture", Global.CardFaces[GetValueString()]);
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
        //spriteRenderer.sortingOrder = sortLevel;
        //if(!condition) {
        //    transform.localPosition = targetPosition;
        //    return;
        //}
        transform.localPosition = Vector3.Lerp(currPosition, targetPosition, selectSpeed * Time.deltaTime);
    }
}
