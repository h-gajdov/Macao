using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void OnValidate() {
        if (!Global.CardFaces.ContainsKey(GetValueString())) return;
        spriteRenderer.sharedMaterial.SetTexture("_FrontTexture", Global.CardFaces[GetValueString()]);
    }

    private void Start() {
        spriteRenderer.material.SetTexture("_FrontTexture", Global.CardFaces[GetValueString()]);
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
}
