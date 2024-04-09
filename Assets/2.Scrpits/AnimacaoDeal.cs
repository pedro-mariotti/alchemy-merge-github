using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimacaoDeal : MonoBehaviour
{
    [Header("Animações:")]
    [SerializeField] private AnimationCurve ac_GoToCard;

    [Header("SpriteRenderer:")]
    [SerializeField] private SpriteRenderer sprFigura;

    [Header("SpriteGameObject:")]
    [SerializeField] private GameObject sprObj;
    
    [Header("Texto:")]
    [SerializeField] private TextControllerV2 text;

    //Animcação:
    private Vector3 positionStart;
    private Vector3 positionEnd;
    private Vector3 scaleStart;
    private Vector3 scaleEnd;
    private Vector3 localPositionEnd;
    private float animationGoToCard_Count = 0f;
    private float animationGoToCard_End = 30f;
    private float animationGoToCard_Index = 0f;
    private float animationGoToCard_Lerp = 0f;
    private bool inAnimation = false;

    public void Start()
    {
        localPositionEnd = sprObj.transform.localPosition;
    }
    public void Init()
    {
        if (inAnimation == false)
        {
            inAnimation = true;

            scaleStart = new Vector3(.2f,.2f,1f);
            scaleEnd = sprObj.transform.localScale;

            positionEnd = new Vector3(transform.position.x + (localPositionEnd.x*transform.lossyScale.x),transform.position.y+ (localPositionEnd.y*transform.lossyScale.y),transform.position.z);

            sprFigura.color  = new Color(1f,1f,1f,0f);
            text.ChangeAlphaText(0f);

            animationGoToCard_Count = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inAnimation)
        {
            if (animationGoToCard_Count < animationGoToCard_End)
            {
                //Subtrai (avançar na animação):
                animationGoToCard_Count++;

                //Atualiza valor para animação:
                animationGoToCard_Index = (animationGoToCard_Count / animationGoToCard_End);

                //Posiciona:
                positionStart = GameObject.Find("DealButton").transform.position;
                animationGoToCard_Lerp = ac_GoToCard.Evaluate(animationGoToCard_Index);
                sprObj.transform.position = Vector3.Lerp(positionStart, positionEnd, animationGoToCard_Lerp);
                

                //Scala:
                sprObj.transform.localScale = Vector3.Lerp(scaleStart, scaleEnd, animationGoToCard_Index);
                
                //Alpha:
                float alphaAnimation = animationGoToCard_Index*2;
                sprFigura.color  = new Color(1f,1f,1f,alphaAnimation);
                text.ChangeAlphaText(alphaAnimation-.95f);

                //Último estágio da animação:
                if (animationGoToCard_Count == animationGoToCard_End)
                {
                    inAnimation = false;
                }
            }
        }
        else
        {
            sprObj.transform.localPosition = localPositionEnd;
        }
    }
}
