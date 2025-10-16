using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteOrder : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock propBlock;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        propBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        spriteRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_SpriteOrder", spriteRenderer.sortingOrder);
        spriteRenderer.SetPropertyBlock(propBlock);
    }
}
