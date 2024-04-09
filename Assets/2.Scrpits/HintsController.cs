using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintsController : MonoBehaviour
{

    [Header("Texto:")]
    [SerializeField] private TextControllerV2 text;

    [Header("Custo Inicial:")]
    [SerializeField] private int custo;

    [Header("Para animação de resultado final:")]
    [SerializeField] private GameObject prefabGoldenCard;

    [Header("Sprite Renderers:")]
    public SpriteRenderer sprRenMoeda;

    private BoxCollider2D boxCollider;

    public Merge mergeToHint = null;


    // Start is called before the first frame update
    void Start()
    {
        //coleta boxCollider:
        boxCollider = GetComponent<BoxCollider2D>();
        //Sobe o texto (draw layers):
        text.SetSortingOrderExt(101,"UI");

        //Carrega preço salvo:
        if (PlayerPrefs.HasKey("HintCusto"))
        {
            custo = PlayerPrefs.GetInt("HintCusto");
        }
        //Carrega quantidade de dicas que possui:
        if (PlayerPrefs.HasKey("Hints"))
        {
            PCSettings.hints = PlayerPrefs.GetInt("Hints");
        }
    }

    public void UpdateMergeToHint(Merge atualMerge)
    {
        if (mergeToHint != atualMerge || atualMerge == null)
        {
            mergeToHint = atualMerge;

            if (PCSettings.hints>0)
            {
                text.ChangeText("HINTS: "+PCSettings.hints);
                sprRenMoeda.enabled = false;
            }
            else
            {
                text.ChangeText("HINT: "+FindObjectOfType<PCSettings>().ShortenNumber(custo));
                sprRenMoeda.enabled = true;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        //Informa visualmente se tenho grana suficiente:
        if (PCSettings.hints>0 || FindObjectOfType<BankController>().GetBankValue() >= custo)
        {
            text.ChangeColorText(new Color(.24f,.24f,.24f,1f));
        }
        else
        {
            text.ChangeColorText(new Color(.55f,.1f,.1f,1f));
        }

        //Invisivel caso n tenha nada pra abrir de dica: 
        if (mergeToHint==null)
        {
            //Some com esse troço da tela:
            transform.position = new Vector3(10000f,10000f,1f);
            return;
        }
        else
        {
            //Volta pra tela:
            transform.localPosition = new Vector3(0f,-.66f,0f);
        }


        //Não clica com lockGame:
        if (PCSettings.lockGame || PCSettings.inAnimationMerge) { return; }

        // Checa se o player tocou no botão
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);


            if (boxCollider == Physics2D.OverlapPoint(touchPos))
            {   
                if (PCSettings.hints>0 || FindObjectOfType<BankController>().GetBankValue() >= custo)//tenho contições de comprar
                {
                    //Abrir dica!!
                    mergeToHint.hint = true;

                    //informa que o player já usou uma dica: 
                    PCSettings.tutorialHintFinish = true;

                    //Mandar o card pra wiki:
                    createCardToWiki(false, mergeToHint);

                    //Gasta:
                    if (PCSettings.hints>0)
                    {
                        PCSettings.hints--;

                        //Salva:
                        PlayerPrefs.SetInt("Hints", PCSettings.hints);
                    }
                    else
                    {
                        FindObjectOfType<BankController>().RemoveMoney(custo);
                        custo+=100;

                        //Salva:
                        PlayerPrefs.SetInt("HintCusto", custo);
                    }

                    //coleta o texto:
                    string dicaTexto = mergeToHint.resultado.cardName;

                    //Atualiza placar de possiveis merges:
                    FindObjectOfType<PossibleToMerge>().updatePossibleToMerge();

                    //Salvamos o progresso:
                    FindObjectOfType<PCSettings>().SaveMergeData();
                }

            }
        }
    }


    void createCardToWiki(bool isFinal, Merge merge)
    {
        Debug.Log("Criando card");

        //Animação:
        GameObject objGoldenCard = Instantiate(prefabGoldenCard, transform.position, Quaternion.identity);
        objGoldenCard.transform.SetParent(gameObject.transform.parent);
        objGoldenCard.transform.localScale = transform.localScale;

        objGoldenCard.GetComponent<CardController>().figura = merge.resultado;

        if (isFinal)
        {
            objGoldenCard.GetComponent<CardController>().sprRendererFigura.sprite = merge.resultado.sprite;
        }
        else
        {
            objGoldenCard.GetComponent<CardController>().statusCard = 10;
            objGoldenCard.GetComponent<CardController>().UpdateSprites();
        }

        objGoldenCard.GetComponent<CardFinalAnimation>().SetDelay(-75f);
        objGoldenCard.GetComponent<CardFinalAnimation>().Conf(false);
        objGoldenCard.GetComponent<CardFinalAnimation>().Init();

        objGoldenCard.GetComponent<CardController>().ChangeLayerToUI();

        objGoldenCard.GetComponent<CardController>().text.ChangeText(merge.resultado.cardName);

        //gameObjectCardSelect1.GetComponentInChildren<CardDrag>().cardFinalAnimation = objGoldenCard.GetComponent<CardFinalAnimation>();
    }
}
