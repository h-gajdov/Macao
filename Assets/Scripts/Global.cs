using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global {
    public static Dictionary<string, Texture> CardFaces = new Dictionary<string, Texture>();
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
                Texture value = Resources.Load<Texture>("Sprites/Cards/" + key);
                CardFaces.Add(key, value);
                AllCardStrings.Add(key);
            }
        }
        BackFace = Resources.Load<Texture>("Sprites/Cards/Back");
    }
}
