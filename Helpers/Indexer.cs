using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

public class Indexer : MonoBehaviour
{
    [SerializeField]
    private GameObject entry;

    private PresentationObject root;

    public PresentationObject this[string name] => root[name];

    void Awake()
    {
        ReloadIndex();
    }

    public void ReloadIndex()
    {
        root = GetNodeOf(entry.transform);
    }
    private PresentationObject GetNodeOf(Transform t)
    {
        string name = t.name.Trim().ToLower();

        Dictionary<string, PresentationObject> children = new();

        foreach (Transform child in t)
        {
            PresentationObject childNode = GetNodeOf(child);
            children.Add(childNode.name, childNode);
        }

        t.TryGetComponent<SpriteRenderer>(out SpriteRenderer? spriteRenderer);
        return new PresentationObject() 
        {
            name = name, 
            obj = t.gameObject, 
            children = children, 
            spriteRenderer = spriteRenderer 
        };
    }
}