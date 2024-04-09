using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAvisoMergeFalho : MonoBehaviour
{
    private GameObject LastCardInDrag = null;
    private PCSettings PC;
    private bool ativo = false;
    private bool isFail = false;


    private bool ativoPorNoMatches = false;

    //Animcação:
    private float animation_Count = 0f;
    private float animation_End = 15f;
    
    [Header("SpriteRenderer:")]
    [SerializeField] private SpriteRenderer sprRender;
    [SerializeField] private SpriteRenderer sprRenderIcon;

    [Header("meu card:")]
    [SerializeField] private CardController myCard;

    // Start is called before the first frame update
    void Start()
    {
        PC = GameObject.Find("PC").GetComponent<PCSettings>();
    }

    // Update is called once per frame
    void Update()
    {
        //Sombra:
        if (ativo || ativoPorNoMatches)
        {
            if (animation_Count<animation_End)
            {
                animation_Count++;
            }

            if (isFail)
            {sprRender.color = new Color(1f,0f,0f,animation_Count/animation_End*.35f);}
            else
            {sprRender.color = new Color(0f,1f,0f,animation_Count/animation_End*.35f);}

            if (ativo && !ativoPorNoMatches && isFail)
            {sprRenderIcon.color = new Color(1f,1f,1f,animation_Count/animation_End);}
            else
            {sprRenderIcon.color = new Color(1f,1f,1f,0f);}
        }
        else
        {
            if (animation_Count>0f)
            {
                animation_Count--;

                if (sprRender.color.r == 1f)
                {sprRender.color = new Color(1f,0f,0f,animation_Count/animation_End*.35f);}
                else
                {sprRender.color = new Color(0f,1f,0f,animation_Count/animation_End*.35f);}

                if (sprRenderIcon.color.a>0f)
                {sprRenderIcon.color = new Color(1f,1f,1f,animation_Count/animation_End);}
            }
        }

        //Atualizar informação de ativo ou n:
        //para tutorial de no matches:
        if (FindObjectOfType<PossibleToMerge>().MergesPossibleToMerge.Count == 0 && myCard.statusCard==3) // && PCSettings.onboardingStage >= 2
        {
            //ativoPorNoMatches = true; //DESATIVADO//
            //isFail = true;
        }
        else
        {
            ativoPorNoMatches = false;
        }

        //para aviso de in drag já conhecido:
        if (!PCSettings.inDragging)
        {
            ativo = false;
        }
        else //if (PCSettings.cardInDrag!=LastCardInDrag)
        {
            LastCardInDrag = PCSettings.cardInDrag;

            if (myCard.figura == PC.figuraNull) //card sem nada
            {
                ativo = false;
                return;
            }

            if (LastCardInDrag==null)
            {
                ativo = false;
            }
            else
            {
                foreach (Merge merge in PC.allMerges)
                {
                    if (merge.resultado == PC.figuraNull) //não dá merge conhecido
                    {
                        if ((merge.a == myCard.figura) && (merge.b == LastCardInDrag.GetComponent<CardController>().figura))
                        {
                            ativo = true;
                            isFail = true;
                            break;
                        }

                        if ((merge.b == myCard.figura) && (merge.a == LastCardInDrag.GetComponent<CardController>().figura))
                        {
                            ativo = true;
                            isFail = true;
                            break;
                        }
                    }
                    else if (merge.descoberto)//dá merge conhecido
                    {
                        if ((merge.a == myCard.figura) && (merge.b == LastCardInDrag.GetComponent<CardController>().figura))
                        {
                            ativo = true;
                            isFail = false;
                            break;
                        }

                        if ((merge.b == myCard.figura) && (merge.a == LastCardInDrag.GetComponent<CardController>().figura))
                        {
                            ativo = true;
                            isFail = false;
                            break;
                        }
                    }
                }
            }
        }
    }
}
