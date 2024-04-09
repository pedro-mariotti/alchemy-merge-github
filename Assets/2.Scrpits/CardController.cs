using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardController : MonoBehaviour
{
    [Header("Identificador do card:")]
    public string cardID;

    [Header("Conf Geral:")]
    public Figure figura;
    public Figure figuraNull;
    public int statusCard;
    // -1 = pré Spoiler
    // 0 = Spoiler
    // 1 = Bloqueado e sem grana suficiente
    // 2 = Bloqueado e com grana suficiente
    // 3 = Livre para uso
    // 10 = Card especial, apenas para animação, é um novo merge para a wiki.
    // 11 = Card especial, apenas para animação, é um card final na arvore de merges.



    [Header("Desbloquar figura:")]
    public Figure DesbFigura;

    [Header("Ficar disponível para compra:")]
    public int LvlSaidaSpoiler = 1000;

    [Header("Compra:")]
    public int valor = 100;
    public BankController bank;
    public TextController textOld;
    public TextControllerV2 text;
    public GameObject MaoCompra;



    [Header("Sprite Renderers:")]
    public SpriteRenderer sprRendererFigura;
    public SpriteRenderer sprRendererBG;
    public SpriteRenderer sprRendererCadeado;
    public SpriteRenderer sprRendererUnlookFigure;

    [Header("Sprites BG:")]
    public Sprite[] sprArrBG;

    [Header("Cores texto:")]
    public Color c_Spoiler, c_SemGrana, c_ComGrana, c_Figura, c_Wiki, c_CardFinal;

    [Header("Informacoes internas:")]
    public bool inMerge = false;

    private PCSettings PC;
    float animCount = 0;
    float animEnd = 20f;
    float animIndex;
    float cardScale;
    public AnimationCurve bombAnim_ac;
    public bool initBombAnimation = false;

    [Header("Moeda:")]
    public GameObject objMoeda;


    // Start is called before the first frame update
    public void Start()
    {
        PC = GameObject.Find("PC").GetComponent<PCSettings>();
        if (ReferenceEquals(figura, null))
        {
            figura = figuraNull;
        }

        if (statusCard == 1)
        {
            figura = figuraNull;
        }

        //Teste entrada no modo pre-spoiler:
        if (statusCard == 0 && PCSettings.LevelPlayer < LvlSaidaSpoiler-1)
        {
            statusCard = -1;
        }

        //Load:
        FindObjectOfType<PCSettings>().LoadCard(this);

        UpdateSprites();

    }
    private void Update()
    {
        if (initBombAnimation && false)
        {
            animCount++;
            animIndex = animCount / animEnd;
            // Debug.Log("Animacao bomba iniciada");

            cardScale = bombAnim_ac.Evaluate(animIndex);
            // Debug.Log(cardScale);
            transform.localScale = new Vector3(cardScale, cardScale, cardScale);
            Debug.Log(transform.localScale.x);

            if (animCount == animEnd)
            {
                // Debug.Log("Animacao bomba finalizada");
                animCount = 0;
                initBombAnimation = false;
            }
        }

        TextController(valor);

        //Teste saida do modo pre-spoiler:
        if (statusCard == -1 && PCSettings.LevelPlayer >= LvlSaidaSpoiler-1 && (PCSettings.unlockCards || PCSettings.onboardingStage >= 3))
        {
            //O bixo é brabo, subiu de level meismo hein:
            statusCard = 0; //avança pro status 0
            UpdateSprites();
        }

        //Teste saida do modo spoiler:
        if (statusCard == 0 && PCSettings.LevelPlayer >= LvlSaidaSpoiler && (PCSettings.unlockCards || PCSettings.onboardingStage >= 3))
        {
            //O bixo é brabo, subiu de level meismo hein:
            statusCard = 1; //avança pro status 1
            UpdateSprites();
        }
        else if (statusCard == 1 && FindObjectOfType<BankController>().GetBankValue() >= valor)
        {
            //Juntando as nica, dá pra comprar hein:
            statusCard = 2; //avança pro status 2
            UpdateSprites();
        }
        else if (statusCard == 2 && FindObjectOfType<BankController>().GetBankValue() < valor)
        {
            //carão vei:
            statusCard = 1; //volta pro status 1
            UpdateSprites();
            //Libera tutorial:
            AnimacaoChamarAtencao animaCard = gameObject.GetComponent<AnimacaoChamarAtencao>();
            if (animaCard != null)
            { animaCard.StopAnimacao(); }
        }


        //Vamos dar foco ao tutorial:
        if (PCSettings.onboardingStage<3)
        {
            if (statusCard <= 2 && statusCard != -1)
            {
                statusCard = -1;
                UpdateSprites();
            }
        }
    }

    // Update is called once per frame
    public void UpdateSprites(bool isDeal = false)
    {
        //Animação de deal:
        if (isDeal)
        { gameObject.GetComponent<AnimacaoDeal>().Init(); }

        //Por padrão, desabilita a mão do "compre! compre! compre! Botini"
        if (MaoCompra!=null)
        {MaoCompra.SetActive(false);}

        switch (statusCard)
        {
            //pre-spoiler:
            case -1:
            //Spoiler
            case 0:
                //Figura do card:
                sprRendererFigura.sprite = null;

                //Cor texto:
                text.ChangeColorText(c_Spoiler);

                //Desbloqueio de uma nova figura:
                bool isDesbFigura = false;
                if (DesbFigura != null && DesbFigura != figuraNull && statusCard==0)
                {
                    //Figura:
                    sprRendererUnlookFigure.enabled = true;
                    sprRendererUnlookFigure.sprite = DesbFigura.sprite;
                    sprRendererUnlookFigure.transform.localScale = new Vector3(.5f, .5f, .5f);
                    sprRendererUnlookFigure.transform.localPosition = new Vector3(0f, .1f, 0f);
                    sprRendererUnlookFigure.color = new Color(0f, 0f, 0f, ((float)PCSettings.LevelPlayer / (float)LvlSaidaSpoiler) * .5f);

                    isDesbFigura = true;
                }
                else
                {
                    //Figura:
                    sprRendererUnlookFigure.enabled = false;
                }

                //Cadeado:
                sprRendererCadeado.enabled = false;

                //background por status:
                if (statusCard==0)
                {sprRendererBG.sprite = sprArrBG[0];}
                else
                {sprRendererBG.sprite = null;}
                break;

            //Bloqueado e sem grana suficiente:
            case 1:
            //Bloqueado e com grana suficiente:
            case 2:
                //Figura do card:
                sprRendererFigura.sprite = null;


                //Cor texto:
                if (statusCard == 1)
                { text.ChangeColorText(c_SemGrana); }
                else
                { 
                    text.ChangeColorText(c_ComGrana);

                    //Aprofeitando o if pra ativar a mão do "compre! compre! compre! Botini"
                    if (MaoCompra!=null)
                    {MaoCompra.SetActive(true);}
                }


                //Desbloqueio de uma nova figura:
                if (DesbFigura != null && DesbFigura != figuraNull)
                {
                    //Figura:
                    sprRendererUnlookFigure.enabled = true;
                    sprRendererUnlookFigure.sprite = DesbFigura.sprite;
                    sprRendererUnlookFigure.transform.localScale = new Vector3(.4f, .4f, .4f);
                    sprRendererUnlookFigure.transform.localPosition = new Vector3(0f, 0.4f, 0f);
                    sprRendererUnlookFigure.color = new Color(0f, 0f, 0f, .75f);

                    //Cadeado:
                    sprRendererCadeado.enabled = true;
                    sprRendererCadeado.transform.localScale = new Vector3(.6f, .6f, .6f);
                    sprRendererCadeado.transform.localPosition = new Vector3(.35f, .1f, 0f);
                }
                else
                {
                    //Figura:
                    sprRendererUnlookFigure.enabled = false;

                    //Cadeado:
                    sprRendererCadeado.enabled = true;
                    sprRendererCadeado.transform.localScale = new Vector3(.9f, .9f, .9f);
                    sprRendererCadeado.transform.localPosition = new Vector3(0f, .33f, 0f);
                }

                //background por status:
                sprRendererBG.sprite = sprArrBG[statusCard];
                break;


            //Livre para uso:
            case 3:
                //Cor texto:
                text.ChangeColorText(c_Figura);

                //Figura do card:
                if (sprRendererFigura != null)
                    sprRendererFigura.sprite = figura.sprite;

                //Desbloqueio de uma nova figura:
                if (sprRendererUnlookFigure != null)
                    sprRendererUnlookFigure.enabled = false;

                //Cadeado:
                if (sprRendererCadeado != null)
                    sprRendererCadeado.enabled = false;

                //background por status:
                sprRendererBG.sprite = sprArrBG[statusCard];
                break;



            //Card especial, apenas para animação, é um novo merge para a wiki.
            case 10:
                //Cor texto:
                text.ChangeColorText(c_Wiki);

                //Desbloqueio de uma nova figura:
                if (sprRendererUnlookFigure != null)
                    sprRendererUnlookFigure.enabled = false;

                //Cadeado:
                if (sprRendererCadeado != null)
                    sprRendererCadeado.enabled = false;

                //background por status:
                sprRendererBG.sprite = sprArrBG[3];

                //Figura do card:
                sprRendererFigura.sprite = figura.sprite;
                break;

            //Card especial, apenas para animação, é um card final na arvore de merges.
            case 11:
                //Cor texto:
                text.ChangeColorText(c_CardFinal);

                //Figura do card:
                sprRendererFigura.sprite = figura.sprite;
                break;

        }

        //Salva:
        FindObjectOfType<PCSettings>().SaveCard(this);
    }


    public void TextController(int price)
    {
        //Isso aqui é resto do código mal resolvido: 
        textOld.ChangeText(" ");

        //Sem Moeda por padrão:
        if (objMoeda!=null)
        {objMoeda.GetComponent<SpriteRenderer>().enabled = false;}
        //tamanho padrão:
        text.ChangeTextSize(.15f);
        //Posição padrão:
        text.changePos(0f,0f);

        if (statusCard < 1)
        {
            text.ChangeText("");//Spoiler n tem preço nem nome
        }
        else if (figura != figuraNull && figura != null)
        {
            text.ChangeText(figura.cardName); //nome do elemento
        }
        else if (statusCard == 2 || statusCard == 1)
        {
            //preço do slot novo:
            string sprPrice = PC.ShortenNumber(price); 
            text.ChangeText(sprPrice); 

            //aparece moeda:
            objMoeda.GetComponent<SpriteRenderer>().enabled = true;

            //Novo tamanho: 
            text.ChangeTextSize(.2f);

            //Posiciona:
            int quantidadeCaracteres = sprPrice.Length;
            switch (quantidadeCaracteres)
            {
                case 1:
                 text.changePos(-.13f,-.01f);
                 objMoeda.transform.localPosition = new Vector3(.13f, objMoeda.transform.localPosition.y,objMoeda.transform.localPosition.z);
                 break;
                case 2:
                 text.changePos(-.12f,-.01f);
                 objMoeda.transform.localPosition = new Vector3(.21f, objMoeda.transform.localPosition.y,objMoeda.transform.localPosition.z);
                 break;
                case 3:
                 text.changePos(-.11f,-.01f);
                 objMoeda.transform.localPosition = new Vector3(.3f, objMoeda.transform.localPosition.y,objMoeda.transform.localPosition.z);
                 break;
                case 4:
                 text.changePos(-.10f,-.01f);
                 objMoeda.transform.localPosition = new Vector3(.38f, objMoeda.transform.localPosition.y,objMoeda.transform.localPosition.z);
                 break;
                case 5:
                 text.changePos(-.08f,-.01f);
                 objMoeda.transform.localPosition = new Vector3(.45f, objMoeda.transform.localPosition.y,objMoeda.transform.localPosition.z);
                 break;
                default:
                 break;
            }
        }
        else if (statusCard == 3 && figura == figuraNull)
        {
            text.ChangeText("");//slot vazio, sem elemento nenhum no card.
        }

    }

    public void ChangeLayerToUI()
    {
        sprRendererFigura.sortingLayerName = "UI";
        sprRendererBG.sortingLayerName = "UI";
        text.ChangeLayer("UI");
    }
}
