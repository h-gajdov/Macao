using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face {
    public Texture texture;
    public Sprite sprite;

    public Face(Texture texture, Sprite sprite) {
        this.texture = texture;
        this.sprite = sprite;
    }
}

public static class Global {
    public static Dictionary<string, Face> CardFaces = new Dictionary<string, Face>();
    public static List<string> AllCardStrings = new List<string>();
    public static Texture BackFace;

    public static void Initialize() {
        CardFaces.Clear();
        AllCardStrings.Clear();
        for (int cardValue = 1; cardValue <= 13; cardValue++) {
            string first = cardValue.ToString();
            if (cardValue == 1) first = "A";
            else if (cardValue > 10) {
                switch (cardValue) {
                    case 11: first = "J"; break;
                    case 12: first = "Q"; break;
                    case 13: first = "K"; break;
                }
            }
            foreach (Suit s in System.Enum.GetValues(typeof(Suit))) {
                string key = first + s.ToString()[0];
                Texture2D texture = Resources.Load<Texture2D>("Sprites/Cards/" + key);
                Sprite sprite = Resources.Load<Sprite>("Sprites/Cards/" + key);
                Face value = new Face(texture, sprite);
                CardFaces.Add(key, value);
                AllCardStrings.Add(key);
            }
        }
        BackFace = Resources.Load<Texture>("Sprites/Cards/Back");
    }
}
