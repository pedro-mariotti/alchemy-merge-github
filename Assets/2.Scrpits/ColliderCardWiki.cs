using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCardWiki : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    public WikiPopupController popUp;
    private CardWikiController cardWikiController;
    public GameObject prefab;
    public List<GameObject> cardsPop;
    public WikiController wikiC;
    public Sprite agua;
    public Sprite terra;
    public Sprite fogo;
    public Sprite ar;
    public bool ver;
    SoundController soundController;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        popUp = FindObjectOfType<WikiPopupController>();
        cardWikiController = GetComponentInParent<CardWikiController>();
        wikiC = FindObjectOfType<WikiController>();
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
    }

    // Update is called once per frame
    void Update()
    {

        //verificando se o jogador clicou no card
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && !PCSettings.inScrolling)
        {
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            Collider2D colliderImput = Physics2D.OverlapPoint(touchPos);
            if (boxCollider == colliderImput)
            {   //se tocou entra no metodo para aparecer o popup do wiki        



                GameObject cardTocado = colliderImput.transform.parent.gameObject;

                popUp.MoveToPosition();
                wikiC.GetComponent<WikiController>().PopInstance(cardTocado);


            }
        }

    }



}
