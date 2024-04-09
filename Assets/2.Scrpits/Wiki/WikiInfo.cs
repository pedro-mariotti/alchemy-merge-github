using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WikiInfo : MonoBehaviour
{
    [SerializeField] private List<Sprite> listSprites;
    [SerializeField] private SpriteRenderer spriteRenderer;

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.sprite = listSprites[PCSettings.WikiAba-1];
    }
}
