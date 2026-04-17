using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

public struct PresentationObject
{
    public string name;
    public GameObject obj;
    public readonly Transform transform => obj.transform;
    public Dictionary<string, PresentationObject> children;
    public SpriteRenderer? spriteRenderer;

    public bool initialIsActive;
    public Vector3 initialPosition;
    public Vector3 initialScale;
    public Quaternion initialRotation;
    public Color? initialColor;

    public readonly PresentationObject this[string name]
    {
        get
        {
            name = name.Trim().ToLower();

            string[] parts = name.Split('/');
            PresentationObject current = this;
            foreach (string part in parts)
            {
                if (current.children.TryGetValue(part, out PresentationObject child))
                {
                    current = child;
                }
                else
                {
                    throw new Exception($"Node `{current.name}` has no child with name `{part}`");
                }
            }
            return current;
        }
    }
}