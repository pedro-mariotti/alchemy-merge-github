using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossibleToMerge : MonoBehaviour
{

    [Header("SpriteRenderer:")]
    [SerializeField] private List<SpriteRenderer> sprsFigurasPar;
    [SerializeField] private List<SpriteRenderer> sprsFigurasImpar;
    [SerializeField] private SpriteRenderer sprRendererFundo;

    [Header("Sprites:")]
    [SerializeField] private List<Sprite> sprsFundos;

    [Header("Conf gerais:")]
    [SerializeField] private PCSettings PC;
    [SerializeField] private GameObject board;
    [SerializeField] private DealController dealController;
    public Figure figuraNull;

    [Header("Animações de tutorial:")]
    [SerializeField] private AnimacaoChamarAtencao tutorialDeal;
    [SerializeField] private AnimacaoChamarAtencao tutorialBombs;

    public List<Merge> MergesPossibleToMerge = new();

    bool updateInicial = true;

    // Start is called before the first frame update
    void Start()
    {
        updatePossibleToMerge();
    }

    // Update is called once per frame
    void Update()
    {
        //Atualiza placar de possiveis merges, no inicio do jogo (depois dos loads):
        if (updateInicial)
        {
            updatePossibleToMerge();
            updateInicial=false;
        }
    }

    public void updatePossibleToMerge()
    {
        //Limpa:
        MergesPossibleToMerge.Clear();

        //Obtem infos do tabuleiro:
        List<Figure> figuresNoTabuleiro = dealController.GetFiguresNoTabuleiro();

        //Vamos percorrer merge por merge e descobrir quais são possiveis:
        foreach (var merge in PC.allMerges)
        {
            //FILTRO 1:
            //Se for resultado nulo, já pula pra próxima item do foreach:
            if (merge.resultado == figuraNull) { continue; }

            //FILTRO 2:
            //Vamos descobrir se temos os elementos necessarios:
            bool checkA = false;
            bool checkB = false;
            foreach (var figure in figuresNoTabuleiro)
            {
                if ((merge.a == figure) && checkA == false) { checkA = true; }
                else if ((merge.b == figure)) { checkB = true; }
            }

            //Temos os elementos necessarios:
            if (checkA && checkB)
            {
                MergesPossibleToMerge.Add(merge);

                if (MergesPossibleToMerge.Count == 10) //Limite de spoilers
                {
                    break;
                }
            }
        }

        //Sem nenhum merge possível? Tutorial.
        //Por padrão setamos sem:
        tutorialDeal.StopAnimacao();
        tutorialBombs.StopAnimacao();
        StopAnimacaoAllCards();
        //Agora sim checar:
        if (MergesPossibleToMerge.Count == 0)
        {
            //Vamos verificar os slots:
            bool slotCompravel = CheckSlotsParaCompra();
            if (slotCompravel == false)
            {
                //Vamos verificar se tem algum slot vazio:
                dealController.UpdateEmptyCards();
                if (dealController.emptyCards.Count == 0)
                {
                    //Sem slots, sugerir bomba:
                    tutorialBombs.StartAnimacao();
                }
                else
                {
                    //Com slots, sugerir Deal:
                    tutorialDeal.StartAnimacao();
                }
            }
        }

        //Arte:
        clearAllFiguras();

        sprRendererFundo.sprite = sprsFundos[MergesPossibleToMerge.Count];

        if (MergesPossibleToMerge.Count >= 9)
        {
            transform.localScale = new Vector3(.6f, .6f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(.85f, .85f, 1f);
        }

        if (MergesPossibleToMerge.Count % 2 == 0)
        {
            //Par:
            for (int i = 0; i < MergesPossibleToMerge.Count; i++)
            {
                sprsFigurasPar[i].sprite = MergesPossibleToMerge[i].resultado.sprite;
                if (MergesPossibleToMerge[i].descoberto == true || MergesPossibleToMerge[i].hint == true)
                { sprsFigurasPar[i].color = new Color(1f, 1f, 1f, 1f); }
                else
                { sprsFigurasPar[i].color = new Color(0f, 0f, 0f, .8f); }
            }
        }
        else
        {
            //Ímpar:
            for (int i = 0; i < MergesPossibleToMerge.Count; i++)
            {
                sprsFigurasImpar[i].sprite = MergesPossibleToMerge[i].resultado.sprite;
                if (MergesPossibleToMerge[i].descoberto == true || MergesPossibleToMerge[i].hint == true)
                { sprsFigurasImpar[i].color = new Color(1f, 1f, 1f, 1f); }
                else
                { sprsFigurasImpar[i].color = new Color(0f, 0f, 0f, .8f); }
            }
        }

        //Atualiza Dicas:
        FindObjectOfType<HintsController>().UpdateMergeToHint(GetNewPossibleMerge());
    }





    private void clearAllFiguras()
    {
        foreach (var sprite in sprsFigurasPar)
        {
            sprite.color = new Color(0f, 0f, 0f, 0f);
        }
        foreach (var sprite in sprsFigurasImpar)
        {
            sprite.color = new Color(0f, 0f, 0f, 0f);
        }
    }


    private bool CheckSlotsParaCompra()
    {
        bool retorno = false;
        for (int i = 0; i < board.transform.childCount; i++)
        {
            Transform currentLine = board.transform.GetChild(i);
            for (int j = 0; j < currentLine.childCount; j++)
            {
                GameObject currentCard = currentLine.GetChild(j).gameObject;
                //Card liberado para compra:
                if ((currentCard.GetComponent<CardController>().statusCard == 1) || (currentCard.GetComponent<CardController>().statusCard == 2))
                {
                    //agora filtra valor
                    int QuantoTenho = FindObjectOfType<BankController>().GetBankValue();
                    int QuantoPreciso = currentCard.GetComponent<CardController>().valor;
                    if (QuantoTenho >= QuantoPreciso)
                    {
                        currentCard.GetComponent<AnimacaoChamarAtencao>().StartAnimacao();
                        retorno = true;
                    }
                }

            }
        }


        return retorno;
    }

    private void StopAnimacaoAllCards()
    {
        for (int i = 0; i < board.transform.childCount; i++)
        {
            Transform currentLine = board.transform.GetChild(i);
            for (int j = 0; j < currentLine.childCount; j++)
            {
                GameObject currentCard = currentLine.GetChild(j).gameObject;
                AnimacaoChamarAtencao animaCard = currentCard.GetComponent<AnimacaoChamarAtencao>();
                if (animaCard != null)
                { animaCard.StopAnimacao(); }
            }
        }
    }

    public Merge GetNewPossibleMerge()
    {
        //Vamos percorrer merge por merge e descobrir quais são possiveis:
        foreach (var merge in MergesPossibleToMerge)
        {
            if ((merge.descoberto == false) && (merge.hint == false))
            {
                return merge;
            }
        }

        return null;
    }
}
