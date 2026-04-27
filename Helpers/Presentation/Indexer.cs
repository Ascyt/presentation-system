using System;
using System.Collections.Generic;
using TMPro;
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

    /// <summary>
    /// Reload all presentation objects and their initial positions from the entry point. 
    /// </summary>
    public void ReloadIndex()
    {
        root = GetNodeOf(entry.transform);
    }
    /// <summary>
    /// Reset all presentation objects to their initial values (active, position, scale, rotation, color) from when <see cref="ReloadIndex"/> was last called.
    /// </summary>
    public void ResetToInitial() 
    {
        ResetNodeToInitial(root);
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
        t.TryGetComponent<ParticleSystem>(out ParticleSystem? particleSystem);
        t.TryGetComponent<TextMeshPro>(out TextMeshPro? textMeshPro);
        return new PresentationObject() 
        {
            name = name, 
            obj = t.gameObject, 
            children = children, 
            spriteRenderer = spriteRenderer,
            particleSystem = particleSystem,
            textMeshPro = textMeshPro,

            initialIsActive = t.gameObject.activeSelf,
            initialPosition = t.localPosition,
            initialScale = t.localScale,
            initialRotation = t.localRotation,
            initialColor = spriteRenderer != null ? spriteRenderer.color : 
                textMeshPro != null ? textMeshPro.color : null,
            initialParticleSystemIsPlaying = particleSystem != null ? particleSystem.isPlaying : null,
            initialTextMeshProText = textMeshPro != null ? textMeshPro.text : null
        };
    }
    private void ResetNodeToInitial(PresentationObject node)
    {
        node.obj.SetActive(node.initialIsActive);
        node.transform.localPosition = node.initialPosition;
        node.transform.localScale = node.initialScale;
        node.transform.localRotation = node.initialRotation;
        if (node.spriteRenderer != null && node.initialColor.HasValue)
        {
            node.spriteRenderer.color = node.initialColor.Value;
        }
        if (node.particleSystem != null)
        {
            node.particleSystem.Clear();
            if (node.initialParticleSystemIsPlaying == true)
                node.particleSystem.Play();
            else
                node.particleSystem.Stop();
        }
        if (node.textMeshPro != null && node.initialTextMeshProText != null)
        {
            node.textMeshPro.text = node.initialTextMeshProText;

            if (node.initialColor.HasValue)
            {
                node.textMeshPro.color = node.initialColor.Value;
            }
        }

        foreach (PresentationObject child in node.children.Values)
        {
            ResetNodeToInitial(child);
        }
    }
}