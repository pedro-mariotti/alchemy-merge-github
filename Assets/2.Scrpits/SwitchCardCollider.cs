using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCardCollider : MonoBehaviour
{
    [Header("Colliders:")]
    public BoxCollider2D colliderToDrag;
    public CircleCollider2D colliderToMerge;

    [Header("Pai (card):")]
    public CardDrag cardDrag;
    // Start is called before the first frame update
    void Start()
    {
        colliderToDrag.enabled = true;
        colliderToMerge.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(PCSettings.inWiki)
        {
            colliderToDrag.enabled = false;
            colliderToMerge.enabled = false;
        }

        if(PCSettings.lockGame)
        {
            colliderToDrag.enabled = false;
            colliderToMerge.enabled = false;
        }

        else if (PCSettings.inDragging)
        {
            colliderToDrag.enabled = false;
            colliderToMerge.enabled = true;

            if (cardDrag.isDragging)
            {
                colliderToMerge.enabled = false;
            }
        }
        else
        {
            colliderToDrag.enabled = true;
            colliderToMerge.enabled = false;
        }
    }
}
