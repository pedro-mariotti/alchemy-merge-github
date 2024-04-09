using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinAnimation : MonoBehaviour
{
    [Header("Animações:")]
    [SerializeField] private AnimationCurve ac_GoToPlacar;

    [Header("SpriteRenderer:")]
    [SerializeField] private SpriteRenderer sprCoin;

    //Animcação:
    private Vector3 positionStart;
    private Vector3 positionEnd;
    private Vector3 scaleStart;
    private Vector3 scaleEnd;
    private float animationGoToPlacar_Count = -30f;
    private float animationGoToPlacar_End = 65f;
    private float animationGoToPlacar_Index = 0f;
    private float animationGoToPlacar_Lerp = 0f;

    public void Init()
    {
        positionStart = transform.position;

        scaleStart = transform.localScale;
        scaleEnd = new Vector3(.2f,.2f,1f);

        sprCoin.color  = new Color(1f,1f,1f,0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (animationGoToPlacar_Count <= 0)
        {
            //Alpha:
            float alphaAnimation = 1f-((animationGoToPlacar_Count*-1f)/30f);
            sprCoin.color  = new Color(1f,1f,1f,alphaAnimation);
        }

        if (animationGoToPlacar_Count < animationGoToPlacar_End)
        {
            //Subtrai (avançar na animação):
            animationGoToPlacar_Count++;

            //Atualiza valor para animação:
            animationGoToPlacar_Index = (animationGoToPlacar_Count / animationGoToPlacar_End);

            //Posiciona:
            positionEnd = GameObject.Find("PlacarGrana").transform.position;
            animationGoToPlacar_Lerp = ac_GoToPlacar.Evaluate(animationGoToPlacar_Index);
            transform.position = Vector3.Lerp(positionStart, positionEnd, animationGoToPlacar_Lerp);

            //Scala:
            transform.localScale = Vector3.Lerp(scaleStart, scaleEnd, animationGoToPlacar_Index);
            

            //Último estágio da animação:
            if (animationGoToPlacar_Count == animationGoToPlacar_End)
            {
                Destroy(gameObject);
            }
        }
    }
}
