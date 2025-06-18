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

    public static List<string> ShuffleListDebug(List<string> list, int seed) {
        List<string> debug = new List<string>() { "7C", "7H", "7S", "1S", "2S", "3S", "4S", "6S", "9H", "8S", "9S", "10S", "QH", "KH" };
        list.RemoveAll(p => debug.Contains(p));
        list = ShuffleList(list, seed);
        debug.AddRange(list);

        int idx7D = debug.IndexOf("7D");
        int idxBJ = debug.IndexOf("BJ");
        int idxRJ = debug.IndexOf("RJ");
        debug[idx7D] = debug[28];
        debug[28] = "7D";
        debug[idxBJ] = debug[52];
        debug[52] = "BJ";
        debug[idxRJ] = debug[51];
        debug[51] = "RJ";

        int idx5D = debug.IndexOf("5D");
        debug[idx5D] = debug.Last();
        debug[debug.Count - 1] = "5D";
        return debug;
    }
}