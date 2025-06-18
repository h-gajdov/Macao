using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameMath
{
    public static float SqrDistance(Vector3 a, Vector3 b) {
        return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);
    }

    public static List<T> ShuffleList<T>(List<T> list) {
        return list.OrderBy(i => System.Guid.NewGuid()).ToList();
    }

    public static List<T> ShuffleList<T>(List<T> list, int seed) {
        System.Random prng = new System.Random(seed);
        return list.OrderBy(i => prng.Next()).ToList();
    }
}