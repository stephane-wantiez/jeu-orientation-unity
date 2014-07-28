using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    private static System.Random _random = new System.Random();

    public static void ShuffleFY<T>(IList<T> list)
    {
        var random = _random;
        for (int i = list.Count; i > 1; i--)
        {
            // Pick random element to swap.
            int j = random.Next(i); // 0 <= j <= i-1
            // Swap.
            T tmp = list[j];
            list[j] = list[i - 1];
            list[i - 1] = tmp;
        }
    }

    public delegate void DestroyMethod(GameObject obj);

    public static void DeleteChildrenOfTransform(Transform transformParent, DestroyMethod destroyMethod)
    {
        for (int i = transformParent.childCount - 1; i >= 0; --i)
        {
            destroyMethod(transformParent.GetChild(i).gameObject);
        }
    }
}
