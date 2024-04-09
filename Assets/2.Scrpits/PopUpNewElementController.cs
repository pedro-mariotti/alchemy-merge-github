using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpNewElementController : MonoBehaviour
{

    [Header("Animações:")]
    [SerializeField] private AnimationCurve ac_Alpha;
    [SerializeField] private AnimationCurve ac_Scale;

    [Header("SpriteRenderer:")]
    [SerializeField] private SpriteRenderer sprSOMBRA;
    [SerializeField] private SpriteRenderer sprBASE;
    [SerializeField] private SpriteRenderer sprELEMENTO;
    [SerializeField] private SpriteRenderer sprCONFETE1;
    [SerializeField] private SpriteRenderer sprCONFETE2;
    [SerializeField] private SpriteRenderer sprCONFETE3;
    [SerializeField] private SpriteRenderer sprCONFETE4;

    [Header("Sprites:")]
    [SerializeField] private Sprite sprBASE_GOLD;
    [SerializeField] private Sprite sprBASE_NORMAL;

    [Header("Offset:")]
    [SerializeField] private Vector3 offsetVector = new();

    //Animcação:
    private float animation_Count = 300f;
    private float animation_End = 100f;
    private bool startAnima = false;

    private int animacaoInfinita = 0;

    private void Start()
    {
        if (sprSOMBRA != null) { sprSOMBRA.color = new Color(1f, 1f, 1f, 0f); }
        if (sprBASE != null) { sprBASE.color = new Color(1f, 1f, 1f, 0f); }
        if (sprELEMENTO != null) { sprELEMENTO.color = new Color(1f, 1f, 1f, 0f); }
        if (sprCONFETE1 != null) { sprCONFETE1.color = new Color(1f, 1f, 1f, 0f); }
        if (sprCONFETE2 != null) { sprCONFETE2.color = new Color(1f, 1f, 1f, 0f); }
        if (sprCONFETE3 != null) { sprCONFETE3.color = new Color(1f, 1f, 1f, 0f); }
        if (sprCONFETE4 != null) { sprCONFETE4.color = new Color(1f, 1f, 1f, 0f); }
    }

    // Update is called once per frame
    void Update()
    {
        //n iniciamos ainda:
        if (!startAnima) { return; }

        if (animation_Count < animation_End)
        {
            //Subtrai (avançar na animação):
            animation_Count++;

            //Final da animação:
            if (animation_Count==animation_End && animacaoInfinita>=1)
            {
                animation_Count=0;
                animacaoInfinita++;
            }

            //Atualiza valor para animação:
            float animation_Index = (animation_Count / animation_End);
            float animation_Alpha;
            float animation_Scale;

            //Alpha:
            if (   (animacaoInfinita==0)  //popup comum
                || (animacaoInfinita==1)) //Aqui temos o popup dentro do overlay de level up, no ciclo 1
            {
                animation_Alpha = ac_Alpha.Evaluate(animation_Index);
                if (sprSOMBRA != null) { sprSOMBRA.color = new Color(1f, 1f, 1f, animation_Alpha); }
                if (sprBASE != null) { sprBASE.color = new Color(1f, 1f, 1f, animation_Alpha); }
                if (sprELEMENTO != null) { sprELEMENTO.color = new Color(1f, 1f, 1f, animation_Alpha); }
                if (sprCONFETE1 != null) { sprCONFETE1.color = new Color(1f, 1f, 1f, animation_Alpha); }
                if (sprCONFETE2 != null) { sprCONFETE2.color = new Color(1f, 1f, 1f, animation_Alpha); }
                if (sprCONFETE3 != null) { sprCONFETE3.color = new Color(1f, 1f, 1f, animation_Alpha); }
                if (sprCONFETE4 != null) { sprCONFETE4.color = new Color(1f, 1f, 1f, animation_Alpha); }
            }
            if ((!PCSettings.lockGame) && (animacaoInfinita!=0))//Saída do popup dentro do overlay de level up
            {
                animation_Alpha = 0f;
                if (sprSOMBRA != null) { sprSOMBRA.color = new Color(1f, 1f, 1f, animation_Alpha); }
                if (sprBASE != null) { sprBASE.color = new Color(1f, 1f, 1f, animation_Alpha); }
                if (sprELEMENTO != null) { sprELEMENTO.color = new Color(1f, 1f, 1f, animation_Alpha); }
                if (sprCONFETE1 != null) { sprCONFETE1.color = new Color(1f, 1f, 1f, animation_Alpha); }
                if (sprCONFETE2 != null) { sprCONFETE2.color = new Color(1f, 1f, 1f, animation_Alpha); }
                if (sprCONFETE3 != null) { sprCONFETE3.color = new Color(1f, 1f, 1f, animation_Alpha); }
                if (sprCONFETE4 != null) { sprCONFETE4.color = new Color(1f, 1f, 1f, animation_Alpha); }
            }

            //Scale
            animation_Scale = (ac_Scale.Evaluate(animation_Index + .3f) / 2f) + .5f;
            sprBASE.transform.localScale = new Vector3(animation_Scale, animation_Scale, 1f);

            animation_Scale = ((ac_Scale.Evaluate(animation_Index + .2f) / 2f) + .5f) / 2f;
            sprELEMENTO.transform.localScale = new Vector3(animation_Scale, animation_Scale, 1f);

            if ((sprCONFETE1 != null) && (sprCONFETE2 != null) && (sprCONFETE3 != null) && (sprCONFETE4 != null))
            {
                animation_Scale = ac_Scale.Evaluate(animation_Index);
                sprCONFETE1.transform.localScale = new Vector3(animation_Scale + .05f, animation_Scale + .05f, 1f);

                animation_Scale = ac_Scale.Evaluate(animation_Index - .1f);
                sprCONFETE2.transform.localScale = new Vector3(animation_Scale + .05f, animation_Scale + .05f, 1f);

                animation_Scale = ac_Scale.Evaluate(animation_Index + .1f);
                sprCONFETE3.transform.localScale = new Vector3(animation_Scale * 1.25f, animation_Scale * 1.25f, 1f);

                animation_Scale = ac_Scale.Evaluate(animation_Index + .3f);
                sprCONFETE4.transform.localScale = new Vector3(animation_Scale * 1.25f, animation_Scale * 1.25f, 1f);
            }

        }
        else
        {
            startAnima = false;
        }
    }

    public void ConfAnimation(Sprite spr, bool isGold, GameObject card)
    {
        //popup comum, cola ele sobre o merge:
        if (card!=null)
        {
            Vector3 offsetVectorWithScale = new Vector3(offsetVector.x * transform.lossyScale.x, offsetVector.y * transform.lossyScale.y, 0f);
            transform.position = card.transform.position + offsetVectorWithScale;

            animacaoInfinita = 0;
        }
        //Aqui temos o popup dentro do overlay de level up:
        else
        {
            animacaoInfinita = 1;
        }

        sprELEMENTO.sprite = spr;

        if (isGold)
        {
            sprBASE.sprite = sprBASE_GOLD;
        }
        else
        {
            sprBASE.sprite = sprBASE_NORMAL;
        }
    }

    public void StartAnimation()
    {
        startAnima = true;
        animation_Count = 0f;
    }

    public bool InAnimation()
    {
        if (animation_Count < (animation_End*.4f) && startAnima)
        {
            return true;
        }

        return false;
    }
}
