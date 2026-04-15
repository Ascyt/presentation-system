using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Helpers
{
    public static float Mod(float x, float m)
    {
        return (x % m + m) % m;
    }
    public static int ToInt<T>(this T value) where T : Enum
    {
        return Convert.ToInt32(value);
    }
    public static T RunOnEnumAsInt<T>(this T value, Func<int, int> func) where T : Enum
    {
        int numericValue = Convert.ToInt32(value);
        int newNumericValue = func(numericValue);
        return (T)Enum.ToObject(typeof(T), newNumericValue);
    }

    public const float TAU = Mathf.PI * 2f;
}
