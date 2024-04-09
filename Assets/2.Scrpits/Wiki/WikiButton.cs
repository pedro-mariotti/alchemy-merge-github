using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WikiButton : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private CardWikiController cardWikiController;
    SoundController soundController;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        cardWikiController = FindObjectOfType<CardWikiController>();
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Não clica com lockGame:
        if (PCSettings.lockGame || PCSettings.inAnimationMerge) { return; }

        // Checa se o player tocou no botão
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);


            if (boxCollider == Physics2D.OverlapPoint(touchPos))
            {   //se tocou entra no metodo para aparecer a janela wiki     
                FindObjectOfType<WikiController>().iniciaAzul();
                soundController.TriggerOpenWikiSound();
                PCSettings.inWiki = true;

            }
        }
    }
}
