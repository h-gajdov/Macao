using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;

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
    public static Dictionary<string, Sprite> CountryFlags = new Dictionary<string, Sprite>();
    public static Dictionary<Suit, Sprite> SuitSprites = new Dictionary<Suit, Sprite>();
    public static List<string> AllCardStrings = new List<string>();
    public static Material[] CharacterMaterials = new Material[4];
    public static Texture BackFace;

    public static string[] AvailableLanguages = {
        "Macedonian", "English"
    };

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
                if (s == Suit.All) break;

                string key = first + s.ToString()[0];
                Texture2D texture = Resources.Load<Texture2D>("Sprites/Cards/" + key);
                Sprite sprite = Resources.Load<Sprite>("Sprites/Cards/" + key);
                Face value = new Face(texture, sprite);
                CardFaces.Add(key, value);
                AllCardStrings.Add(key);
            }
        }

        //Black Joekr
        AllCardStrings.Add("BJ");
        Texture2D t = Resources.Load<Texture2D>("Sprites/Cards/BJ");
        Sprite sp = Resources.Load<Sprite>("Sprites/Cards/BJ");
        Face v = new Face(t, sp);
        CardFaces.Add("BJ", v);

        //Red Joker
        AllCardStrings.Add("RJ");
        t = Resources.Load<Texture2D>("Sprites/Cards/RJ");
        sp = Resources.Load<Sprite>("Sprites/Cards/RJ");
        v = new Face(t, sp);
        CardFaces.Add("RJ", v);


        SuitSprites.Clear();
        foreach (Suit key in System.Enum.GetValues(typeof(Suit))) {
            Sprite value = Resources.Load<Sprite>("Sprites/Suits/" + key);
            SuitSprites.Add(key, value);
        }

        BackFace = Resources.Load<Texture>("Sprites/Cards/Back");

        for(int i = 0; i < 4; i++) {
            CharacterMaterials[i] = Resources.Load<Material>($"Prefabs/Character/Materials/Character_{i + 1}");
        }

        CountryFlags.Clear();
        foreach(string language in AvailableLanguages) {
            Sprite sprite = Resources.Load<Sprite>($"Sprites/Flags/{language}");
            CountryFlags.Add(language, sprite);
        }
    }
}
