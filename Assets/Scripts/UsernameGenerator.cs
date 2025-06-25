using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UsernameGenerator {
    private static string[] Adjectives = {
        "Adventurous", "Agile", "Ancient", "Angry", "Artistic", "Awkward", "Balanced", "Beautiful", "Bitter", "Glorious",
        "Blue", "Bold", "Brave", "Bright", "Brilliant", "Bubbly", "Calm", "Cautious", "Charming", "Cheerful",
        "Clever", "Clumsy", "Cold", "Cool", "Crazy", "Creepy", "Curious", "Cute", "Daring", "Dark",
        "Deadly", "Deep", "Delightful", "Determined", "Dizzy", "Dreamy", "Drowsy", "Eager", "Elegant", "Energetic",
        "Enraged", "Excited", "Fancy", "Fast", "Fearless", "Fiery", "Fierce", "Fluffy", "Fragile", "Friendly",
        "Funny", "Fuzzy", "Gentle", "Gigantic", "Glowing", "Golden", "Graceful", "Grumpy", "Happy", "Hasty",
        "Helpful", "Honest", "Hyper", "Icy", "Innocent", "Invisible", "Jolly", "Joyful", "Kind", "Lazy",
        "Light", "Little", "Lonely", "Loud", "Lucky", "Magic", "Massive", "Mighty", "Mysterious", "Nervous",
        "Nice", "Noisy", "Odd", "Orange", "Peaceful", "Playful", "Powerful", "Purple", "Quick", "Quiet",
        "Rapid", "Red", "Sad", "Shiny", "Short", "Shy", "Silly", "Silent", "Slow", "Small",
        "Sneaky", "Soft", "Spooky", "Strange", "Strong"
    };

    private static string[] Nouns = {
        "Alien", "Angel", "Ant", "Archer", "Artist", "Atom", "Avenger", "Bandit", "Bat", "Bear",
        "Beast", "Bee", "Bird", "Blade", "Blob", "Bot", "Brain", "Bug", "Bunny", "Cactus",
        "Cat", "Chameleon", "Champ", "Chicken", "Clown", "Cloud", "Cobra", "Cookie", "Cow", "Crab",
        "Creature", "Crocodile", "Cube", "Cyclone", "Dancer", "Deer", "Devil", "Dino", "Dog", "Dolphin",
        "Dragon", "Duck", "Eagle", "Elf", "Eye", "Falcon", "Fighter", "Firefly", "Fish", "Flame",
        "Fox", "Frog", "Galaxy", "Gamer", "Ghost", "Giant", "Giraffe", "Goblin", "Griffin", "Hamster",
        "Hawk", "Hero", "Horse", "Hydra", "Iceberg", "Imp", "Jaguar", "Jellyfish", "Kangaroo", "Knight",
        "Koala", "Kraken", "Lizard", "Llama", "Magician", "Mammoth", "Mech", "Monkey", "Monster", "Mouse",
        "Ninja", "Octopus", "Ogre", "Owl", "Panda", "Panther", "Parrot", "Penguin", "Phoenix", "Pig",
        "Pirate", "Pixie", "Pug", "Rabbit", "Raccoon", "Reaper", "Robot", "Shark", "Skeleton", "Sloth",
        "Snake", "Squirrel", "Tiger", "Toad", "Troll", "Turtle", "Unicorn", "Vampire", "Viking", "Villain",
        "Walrus", "Whale", "Wizard", "Wolf", "Wraith", "Yeti", "Zombie"
    };

    public static string GenerateUsername() {
        string adjective = Adjectives[Random.Range(0, Adjectives.Length)];
        string noun = Nouns[Random.Range(0, Nouns.Length)];
        int number = Random.Range(100, 999);
        return adjective + noun + number;
    }
}
