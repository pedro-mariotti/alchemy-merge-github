using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDrag : MonoBehaviour
{
    [Header("Pai (Card):")]
    [SerializeField] private GameObject gameObjectCard;
    [SerializeField] private CardController card;

    [Header("Figura Vazia:")]
    [SerializeField] private Figure figuraNull;

    [Header("Sprites Renderers:")]
    public SpriteRenderer sprRen_Figura;
    public SpriteRenderer sprRen_BG;
    public SpriteRenderer sprRen_Select;
    public TextControllerV2 text;

    [Header("Animações:")]
    [SerializeField] private AnimationCurve ac_BackToStart;
    [SerializeField] private AnimationCurve ac_Merge;
    public int inAnimationMerge = 0;
    public Vector3 positionAnimationMerge = new Vector3(0f, 0f, 0f);
    public CardFinalAnimation cardFinalAnimation = null;
    public MergeController mergeControllerToEnd = null;

    //Animcação:
    private bool InRest = true;
    private Vector3 positionStart;
    private Vector3 positionEndDrag;
    private float animationBackToStart_Count = 0f;
    private float animationBackToStart_End = 20f;
    private float animationBackToStart_Index = 0f;
    private float animationBackToStart_Lerp = 0f;

    private float animationMerge_Count = 0f;
    private float animationMerge_End = 30f;

    private bool inRetornandoDoMerge = false;
    private float animationMergeRetorno_Count = -1f;
    private float animationMergeRetorno_End = 10f;
    bool hoveringTrash = false;
    public Vector3 lastScaleTrash = new Vector3(0f, 0f, 0f);


    private Vector3 scaleStart;
    private Vector3 scaleToMerge = new Vector3(.9f, .9f, 1f);

    //Conf para drag:
    private Vector3 screenPoint;
    private Vector3 offset;
    public bool isDragging = false;

    //Merge:
    private MergeController mergeController;

    //usamos essas informações para gerar o popup de novo elemento:
    private Sprite popUpSprite;
    private bool popUpCardFinal;
    private GameObject popUpCardGameObject;
    private bool startPopUp = false;
    SoundController soundController;

    void Start()
    {
        //Encontra o PC e set mergeController:
        mergeController = gameObjectCard.GetComponent<MergeController>();

        positionStart = gameObjectCard.transform.localPosition;
        positionEndDrag = gameObjectCard.transform.localPosition;
        InRest = true;

        scaleStart = gameObjectCard.transform.localScale;
        lastScaleTrash = scaleStart;

        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
    }

    void Update()
    {
        // Verifica se o usuário tocou na tela
        if (Input.touchCount > 0)
        {
            //Não faz nenhum merge na tela de wiki:
            if (PCSettings.inWikiFinal) { return; }

            //Não faz nenhum merge com lockGame:
            if (PCSettings.lockGame) { return; }

            Touch touch = Input.GetTouch(0);

            // Verifica se o toque começou dentro do Collider do objeto
            if (touch.phase == TouchPhase.Began && GetComponent<Collider2D>() == Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(touch.position)))
            {
                startDrag(touch);
            }
            // Atualiza a posição do objeto enquanto o toque estiver sendo arrastado
            else if (touch.phase == TouchPhase.Moved)
            {
                inDrag(touch);
                checkMerge();
            }
            // Finaliza o drag quando o toque for finalizado
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                endDrag();
                merge();
            }
        }


        //Animação do merge:
        if (inAnimationMerge != 0)
        {
            //Sumir com a sombra mais escura:
            if (animationBackToStart_Count > 0)
            {
                //Soma (avançar na animação):
                animationBackToStart_Count--;

                //Atualiza valores para animação:
                animationBackToStart_Index = (animationBackToStart_Count / animationBackToStart_End);

                //Fade da sprite adicional de destaque por estar selecionado:
                sprRen_Select.color = new Color(1f, 1f, 1f, animationBackToStart_Index);
            }

            //Animacao de juntar:
            if (animationMerge_Count < animationMerge_End)
            {
                //Ordem de desenho sobreposta (para ficar acima dos outros cards):
                text.SetSortingOrder(23);
                sprRen_Figura.sortingOrder = 23;
                sprRen_BG.sortingOrder = 21;
                sprRen_Select.sortingOrder = 20;

                //Soma (avançar na animação):
                animationMerge_Count++;

                //Atualiza valores para animação:
                float animationMerge_Index = (animationMerge_Count / animationMerge_End);
                float animationMerge_Lerp = ac_Merge.Evaluate(animationMerge_Index);

                //scale: 
                gameObjectCard.transform.localScale = scaleStart;
                lastScaleTrash = gameObjectCard.transform.localScale;

                //Definir posição inicial:
                float somaAdd = (inAnimationMerge == 1) ? 1f : -1f;
                Vector3 positionAnimationMergeInicio = positionAnimationMerge + new Vector3(somaAdd * transform.lossyScale.x, 0f, 0f);

                //Posiciona:
                gameObjectCard.transform.position = Vector3.Lerp(positionAnimationMergeInicio, positionAnimationMerge, animationMerge_Lerp);
            }
            //Fim da animação de merge
            else
            {
                //Sumir com a sprite atual
                sprRen_Figura.sprite = null;
                text.ChangeText(" ");
                if (inAnimationMerge == 2)
                {
                    animationMergeRetorno_Count = -1f;
                    inRetornandoDoMerge = true;
                    gameObjectCard.transform.localScale = new Vector3(0f, 0f, 1f);
                }
                //Último estágio da animação:
                //libera para arrastar de novo:
                InRest = true;
                //Ordem de desenho sobreposta (para ficar acima dos outros cards):
                text.SetSortingOrder(3);
                sprRen_Figura.sortingOrder = 3;
                sprRen_BG.sortingOrder = 1;
                sprRen_Select.sortingOrder = 0;




                //Animação card para a wiki:
                if (cardFinalAnimation != null)
                {
                    cardFinalAnimation.Init();
                }

                //Resultado do merge:
                if (mergeControllerToEnd != null)
                {
                    mergeControllerToEnd.ResultadoMerge();
                }


                //Animação de popup de novo elemento:
                if (startPopUp)
                {
                    GameObject.Find("PopUpNewElement").GetComponent<PopUpNewElementController>().ConfAnimation(popUpSprite, popUpCardFinal, popUpCardGameObject);
                    GameObject.Find("PopUpNewElement").GetComponent<PopUpNewElementController>().StartAnimation();
                }

                inAnimationMerge = 0;
                animationMerge_Count = 0f;
            }

            //Não tem mais atualizações de animações caso estejamos no merge.
            return;
        }


        //Retorno à scala inicial (pos merge)
        if (inRetornandoDoMerge)
        {
            if (animationMergeRetorno_Count < animationMergeRetorno_End)
            {
                //Avança na animação:
                animationMergeRetorno_Count++;
                float animationMergeRetorno_index = (animationMergeRetorno_Count / animationMergeRetorno_End);

                if (animationMergeRetorno_Count > 1f)
                {
                    gameObjectCard.transform.localPosition = positionStart;
                }
                else
                {
                    gameObjectCard.transform.localScale = new Vector3(0f, 0f, 1f);
                    lastScaleTrash = gameObjectCard.transform.localScale;
                }
            }
            else
            {
                animationMergeRetorno_Count = -1f;
                inRetornandoDoMerge = false;
                PCSettings.inAnimationMerge = false;
            }

        }

        //Retorno à posição inicial:
        if (!isDragging)
        {
            if (animationBackToStart_Count > 0)
            {
                //Subtrai (avançar na animação):
                animationBackToStart_Count--;

                //Atualiza valores para animação:
                animationBackToStart_Index = (animationBackToStart_Count / animationBackToStart_End);
                animationBackToStart_Lerp = ac_BackToStart.Evaluate(animationBackToStart_Index);

                //Posiciona:
                gameObjectCard.transform.localPosition = Vector3.Lerp(positionStart, positionEndDrag, animationBackToStart_Lerp);

                //Fade da sprite adicional de destaque por estar selecionado:
                sprRen_Select.color = new Color(1f, 1f, 1f, animationBackToStart_Index);

                //Último estágio da animação:
                if (animationBackToStart_Count == 0)
                {
                    //libera para arrastar de novo:
                    InRest = true;
                    //Ordem de desenho sobreposta (para ficar acima dos outros cards):
                    text.SetSortingOrder(3);
                    sprRen_Figura.sortingOrder = 3;
                    sprRen_BG.sortingOrder = 1;
                    sprRen_Select.sortingOrder = 0;
                }
            }
        }
        else
        {
            if (animationBackToStart_Count < animationBackToStart_End)
            {
                //Soma (avançar na animação):
                animationBackToStart_Count++;

                //Atualiza valores para animação:
                animationBackToStart_Index = (animationBackToStart_Count / animationBackToStart_End);

                //Fade da sprite adicional de destaque por estar selecionado:
                sprRen_Select.color = new Color(1f, 1f, 1f, animationBackToStart_Index);
            }
        }

        //Possível merge:
        bool liberadoOnboarding = true;
        if (gameObjectCard.GetComponent<HideOnOnboarding>()!=null)
        {
            if (gameObjectCard.GetComponent<HideOnOnboarding>().enabled)
            {
                liberadoOnboarding = false;
            }
        }

        if ((gameObjectCard.GetComponent<AnimacaoChamarAtencao>().ativo == false) && liberadoOnboarding)
        {
            if (PCSettings.cardToMerge == gameObjectCard)
            {
                gameObjectCard.transform.localScale = scaleToMerge;
            }
            else
            {
                gameObjectCard.transform.localScale = scaleStart;
            }
        }

        //definindo escala hoveringTrash:
        if (liberadoOnboarding)
        {
            if (hoveringTrash && isDragging)
            {
                Vector3 scaleFinalTrash = new Vector3(.3f, .3f, 1f);
                gameObjectCard.transform.localScale = Vector3.Lerp(lastScaleTrash, scaleFinalTrash, .15f);

                lastScaleTrash = gameObjectCard.transform.localScale;
            }
            else
            {
                hoveringTrash = false;

                Vector3 scaleFinalTrash = gameObjectCard.transform.localScale;
                gameObjectCard.transform.localScale = Vector3.Lerp(lastScaleTrash, scaleFinalTrash, .3f);

                lastScaleTrash = gameObjectCard.transform.localScale;
            }
        }
    } //fim do update

    void startDrag(Touch touch)
    {
        //Estou com status de "Livre para uso"
        //não estou vazio (null)
        //e em repouso
        if (card.statusCard == 3
            && card.figura != figuraNull && card.figura != null
            && InRest)
        {
            //Variável:
            isDragging = true;
            PCSettings.inDragging = true;
            PCSettings.cardInDrag = gameObjectCard;
            InRest = false;
            //Informações de posição:
            screenPoint = Camera.main.WorldToScreenPoint(gameObjectCard.transform.position);
            offset = gameObjectCard.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, screenPoint.z));
            //Ordem de desenho sobreposta (para ficar acima dos outros cards):
            text.SetSortingOrder(23);
            sprRen_Figura.sortingOrder = 23;
            sprRen_BG.sortingOrder = 21;
            sprRen_Select.sortingOrder = 20;
        }
    }

    void inDrag(Touch touch)
    {
        if (isDragging)
        {
            Vector3 cursorPoint = new Vector3(touch.position.x, touch.position.y, screenPoint.z);
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;
            gameObjectCard.transform.position = cursorPosition;
        }
    }

    void endDrag()
    {
        if (isDragging)
        {
            isDragging = false;
            PCSettings.inDragging = false;
            PCSettings.cardToMerge = null;
            PCSettings.cardInDrag = null;
            //Para animação rolar:
            animationBackToStart_Count = animationBackToStart_End;
            positionEndDrag = gameObjectCard.transform.localPosition;


        }
    }

    void checkMerge()
    {
        //Estou em dragging?
        if (isDragging)
        {
            //Procura um card para fazer merge por collider:
            var cardCollider = CheckColliding();

            //Checa se encontramos um card:
            if (cardCollider != null)
            {
                if (cardCollider.tag == "Card")
                {
                    PCSettings.cardToMerge = cardCollider;
                }
            }
            else
            {
                PCSettings.cardToMerge = null;
            }
        }
    }
    void TrashCard()
    {
        if (PCSettings.onboardingStage<10)
        {
            //informa que o player já descartou algo: 
            PCSettings.tutorialTrashFinish = true;
 
            //Limpa:
            card.GetComponent<CardController>().figura = figuraNull;
            card.GetComponent<CardController>().UpdateSprites();

            //Animação de volta:
            animationMergeRetorno_Count = -1f;
            inRetornandoDoMerge = true;
            gameObjectCard.transform.localScale = new Vector3(0f, 0f, 1f);
            lastScaleTrash = gameObjectCard.transform.localScale;
            inAnimationMerge = 0;
            animationMerge_Count = 0f;
            animationBackToStart_Count = 0f;
            //libera para arrastar de novo:
            InRest = true;
            //Ordem de desenho sobreposta (para ficar acima dos outros cards):
            text.SetSortingOrder(3);
            sprRen_Figura.sortingOrder = 3;
            sprRen_BG.sortingOrder = 1;
            sprRen_Select.sortingOrder = 0;
            //Sumir com a sombra mais escura:
            sprRen_Select.color = new Color(1f, 1f, 1f, 0f);

            //afasta:
            gameObjectCard.transform.localPosition = new Vector3(1000f, 1000f, 1f);

            soundController.TriggerTrashSound();

            //Atualiza placar de possiveis merges:
            FindObjectOfType<PossibleToMerge>().updatePossibleToMerge();

            //Salva card:
            FindObjectOfType<PCSettings>().SaveCard(card.GetComponent<CardController>());
        }
    }

    void merge()
    {
        //Procura um card para fazer merge por collider:
        var cardCollider = CheckColliding();

        //Checa se encontramos um card:
        if (cardCollider != null)
        {
            if (cardCollider.tag == "Card")
            {
                //Checa se ele não esta vazio:
                if (!CheckCardIsNull(cardCollider))
                {
                    if (!PCSettings.inMerge)
                    {
                        card.inMerge = true;
                        cardCollider.GetComponent<CardController>().inMerge = true;
                        PCSettings.inMerge = true;
                        //Tentamos fazer um merge!
                        mergeController.Merge(cardCollider.GetComponent<CardController>(), card); //Caso seja útil no futuro, essa função retorna Figure resultado, sendo uma figuraNull caso não dê merge.
                    }
                }
                //Se for um vazio, apenas troca de posição:
                else
                {
                    Figure figureCardA = cardCollider.GetComponent<CardController>().figura;
                    Figure figureCardB = card.figura;

                    card.figura = figureCardA;
                    cardCollider.GetComponent<CardController>().figura = figureCardB;

                    //Atualiza artes:
                    card.UpdateSprites();
                    cardCollider.GetComponent<CardController>().UpdateSprites();
                }
            }
            //joga a carta no lixo caso a colisao seja detectada acima do lixo
            if (cardCollider.tag == "Trash")
            {
                TrashCard();
            }
        }
    }

    GameObject CheckColliding()
    {
        GameObject.Find("objTrash").GetComponent<AnimacaoLixeira>().inAnimation = false;
        hoveringTrash = false;

        //Encontra o collider que estou tocando na posição atual card (em dragging):
        Collider2D collider = Physics2D.OverlapPoint(transform.position);
        if (collider != null)
        {
            //Pega o card desse collider:
            GameObject card = collider.gameObject.transform.parent.gameObject;
            GameObject trash = collider.gameObject;

            //Checa se é um card mesmo:
            if (card.tag == "Card")
            {

                //Check se esse card n sou eu mesmo
                if (card != gameObjectCard)
                {
                    CardController cardController = card.GetComponent<CardController>();
                    if (cardController.statusCard == 3)
                    {
                        //retorna esse card:
                        return card;
                    }
                }
            }
            else if (trash.tag == "Trash")
            {
                GameObject.Find("objTrash").GetComponent<AnimacaoLixeira>().inAnimation = true;
                hoveringTrash = true;
                return trash;
            }
        }

        //Não achamos ninguém:
        return null;
    }

    bool CheckCardIsNull(GameObject card)
    {
        //Check se esse card está pronto para merge (com uma figura e liberado para uso)
        CardController cardController = card.GetComponent<CardController>();
        if (cardController.statusCard == 3
                && cardController.figura != figuraNull && cardController.figura != null)
        {
            return false;
        }

        return true;
    }

    public void ConfResultadoMerge(int _Card1ou2, MergeController _myMergeController, Vector3 _position)
    {
        cardFinalAnimation = null;

        inAnimationMerge = _Card1ou2;
        mergeControllerToEnd = _myMergeController;
        positionAnimationMerge = _position;

        startPopUp = false;

        PCSettings.inAnimationMerge = true;
    }

    public void ConfPopupNewElement(Sprite _sprite, bool _cardFinal, GameObject _cardGameObject)
    {
        popUpSprite = _sprite;
        popUpCardFinal = _cardFinal;
        popUpCardGameObject = _cardGameObject;

        startPopUp = true;
    }

}
