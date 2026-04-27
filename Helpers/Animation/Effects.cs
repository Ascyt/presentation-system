using System;
using UnityEngine;

#nullable enable

public static class Effects
{
    public static void FadeFromTo<T>(AnimatedStateMachine<T> machine, Fading fading, PresentationObject obj, 
        (Vector3, Vector3)? position = null, (Vector3, Vector3)? scale = null, (Quaternion, Quaternion)? rotation = null, (Color, Color)? color = null) where T : Enum
    {
        machine.Fade(fading, (fadingValue, isExit) =>
        {
            if (position.HasValue)
            {
                obj.transform.localPosition = Vector3.Lerp(position.Value.Item1, position.Value.Item2, fadingValue);
            }
            if (scale.HasValue)
            {
                obj.transform.localScale = Vector3.Lerp(scale.Value.Item1, scale.Value.Item2, fadingValue);
            }
            if (rotation.HasValue)
            {
                obj.transform.localRotation = Quaternion.Lerp(rotation.Value.Item1, rotation.Value.Item2, fadingValue);
            }
            if (color.HasValue)
            {
                Color c = Color.Lerp(color.Value.Item1, color.Value.Item2, fadingValue);
                if (obj.spriteRenderer != null)
                {
                    obj.spriteRenderer.color = c;
                }
                else if (obj.textMeshPro != null)
                {
                    obj.textMeshPro.color = c;
                }
            }
        });
    }

    public static void FadeTo<T>(AnimatedStateMachine<T> machine, Fading fading, PresentationObject obj, 
        Vector3? position = null, Vector3? scale = null, Quaternion? rotation = null, Color? color = null) where T : Enum
    {
        Vector3 startPosition = obj.transform.localPosition;
        Vector3 startScale = obj.transform.localScale;
        Quaternion startRotation = obj.transform.localRotation;
        Color? startColor = obj.spriteRenderer != null ? obj.spriteRenderer.color : 
            obj.textMeshPro != null ? obj.textMeshPro.color : null;

        FadeFromTo(machine, fading, obj, 
            position.HasValue ? (startPosition, position.Value) : null, 
            scale.HasValue ? (startScale, scale.Value) : null, 
            rotation.HasValue ? (startRotation, rotation.Value) : null, 
            color.HasValue ? (startColor ?? Color.white, color.Value) : null);
    }
}