using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFinalAnimation : MonoBehaviour
{
    [Header("Texto:")]
    public TextControllerV2 text;

    [Header("Animações:")]
    [SerializeField] private AnimationCurve ac_GoToWiki;

    //Animcação:
    private Vector3 positionStart;
    private Vector3 positionEnd;
    private Vector3 scaleStart;
    private Vector3 scaleEnd;
    private float animationGoToWiki_Count = -25f;
    private float animationGoToWiki_End = 70f;
    private float animationGoToWiki_Index = 0f;
    private float animationGoToWiki_Lerp = 0f;
    private bool startAnima = false;

    public void Init()
    {
        startAnima = true;
    }

    public void SetDelay(float delay)
    {
        animationGoToWiki_Count = delay;
    }

    public void Conf(bool newElement)
    {
        positionStart = transform.position;

        scaleStart = transform.localScale;
        scaleEnd = new Vector3(.2f,.2f,1f);

        text.SetSortingOrder(103);

        if (newElement)
        {
            animationGoToWiki_Count = -100f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!startAnima)
        {
            //Some pra fora da tela:
            transform.position = new Vector3(-2000f,-200f,-200f);

            //n iniciamos ainda:
            return;
        }

        if (animationGoToWiki_Count < animationGoToWiki_End)
        {
            //Subtrai (avançar na animação):
            animationGoToWiki_Count++;

            //Atualiza valor para animação:
            animationGoToWiki_Index = (animationGoToWiki_Count / animationGoToWiki_End);

            //Posiciona:
            positionEnd = GameObject.Find("WikiButton").transform.position;
            animationGoToWiki_Lerp = ac_GoToWiki.Evaluate(animationGoToWiki_Index);
            transform.position = Vector3.Lerp(positionStart, positionEnd, animationGoToWiki_Lerp);

            //Scala:
            transform.localScale = Vector3.Lerp(scaleStart, scaleEnd, animationGoToWiki_Index);
            

            //Último estágio da animação:
            if (animationGoToWiki_Count == animationGoToWiki_End)
            {
                Destroy(gameObject);
            }
        }
    }
}
