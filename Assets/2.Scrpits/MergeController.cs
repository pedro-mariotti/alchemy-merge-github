using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeController : MonoBehaviour
{
    [Header("Figura Vazia:")]
    [SerializeField] private Figure figuraNull;

    [Header("Para animação de resultado final:")]
    public GameObject prefabGoldenCard;

    [Header("Para animação de ganhar grana:")]
    public GameObject prefabCoinAnimation;

    [Header("Para animação fail merge:")]
    public GameObject prefabXFailMerge;

    //Grana:
    private int bufunfa;

    private CardController referenciaCard1;
    private CardController referenciaCard2;

    //Proxima figura ficar no tabuleiro:
    private Figure FiguraMergeResultado;


    //Auto-completaveis:
    private PCSettings PC;
    private DealController dealController;
    private CardWikiController cardWikiController;

    public bool tutorialMergeFlag = false; //flag somente pra mostrar pro tutorial que um merge ocorreu
    SoundController soundController;

    public void Start()
    {
        dealController = FindObjectOfType<DealController>();
        PC = GameObject.Find("PC").GetComponent<PCSettings>();
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
    }

    // Merge (retorna Figure, sendo uma figuraNull caso não dê merge):
    public Figure Merge(CardController card1, CardController card2)
    {
        //Não faz nenhum merge na tela de wiki:
        if (PCSettings.inWikiFinal) { return figuraNull; }

        //Liberar para próximo merge:
        Invoke("inMergeFinish", .3f);

        //Essa var serve para chegarmos a conclusão de se deu merge ou não no final da função:
        bool mergeSuccess = false;
        bool cardFinal = false;

        foreach (var Merge in PC.allMerges)
        {
            CardController checkA = null;
            CardController checkB = null;

            //Check elemento "a" do merge:
            if (Merge.a == card1.figura) { checkA = card1; }
            else if (Merge.a == card2.figura) { checkA = card2; }
            else { continue; } // não achamos nenhuma figura correspondende nos seletores, continue no foreach para poupar processamento.

            //Check elemento "b" do merge:
            if (Merge.b == card1.figura && checkA != card1) { checkB = card1; }
            else if (Merge.b == card2.figura && checkA != card2) { checkB = card2; }
            else { continue; } // não achamos nenhuma figura correspondende nos seletores, continue no foreach para poupar processamento.

            //Se chegamos aqui sem cair em nenhum "continue", é pq deu merge:
            mergeSuccess = true;



            //Informamos que esse merge foi descoberto:
            bool newMerge = !Merge.descoberto;
            Merge.descoberto = true;

            //Informamos que esse merge foi feito mais uma vez:
            Merge.count++;

            //Esse merge pode ser apenas um merge sem resultado que já foi testado, então vamos confirmar:
            if (Merge.resultado == figuraNull)
            {
                //Debug:
                //-Debug.LogWarning("Não deu merge");

                //Animação no card para indicar fail:
                card1.gameObject.GetComponent<AnimacaoFailMerge>().StartAnimation();
                card2.gameObject.GetComponent<AnimacaoFailMerge>().StartAnimation();
                // soundController.TriggerFailMergeSound();
                Instantiate(prefabXFailMerge, card2.transform.position, Quaternion.identity);

            }
            else
            {
                //MERGE!!!
                soundController.TriggerMergeSound();
                soundController.TriggerMoneySound();
                tutorialMergeFlag = true;


                //Add pontos por ser um merge:
                PC.addXP(.1f);

                //verifica se a carta está selecionada e ativa a animação
                GameObject gameObjectCardSelect1 = card1.gameObject;
                GameObject gameObjectCardSelect2 = card2.gameObject;
                if (gameObjectCardSelect1 != null) { gameObjectCardSelect1.GetComponentInChildren<CardDrag>().ConfResultadoMerge(1, this, card1.transform.position); }
                if (gameObjectCardSelect2 != null) { gameObjectCardSelect2.GetComponentInChildren<CardDrag>().ConfResultadoMerge(2, null, card1.transform.position); }



                //limpa o card depois que é feito o merge   

                referenciaCard1 = card1;
                referenciaCard2 = card2;

                if (checkIfFigureIsFinal(Merge.resultado))
                {
                    //Add pontos extras por ser um merge final:
                    PC.addXP(.2f);

                    //Essa figura que geramos não é útil em nenhum outro merge!
                    //Desconsidera sua criação no tabuleiro:
                    FiguraMergeResultado = figuraNull;
                    //E recompensa o jogado:
                    FindObjectOfType<BankController>().StoreMoney(bufunfa);
                    CreateCoins(card1);
                    CreateCoins(card1);
                    CreateCoins(card1);

                    //Animação:
                    GameObject objGoldenCard = Instantiate(prefabGoldenCard, card1.transform.position, Quaternion.identity);
                    objGoldenCard.transform.SetParent(card1.gameObject.transform.parent);
                    objGoldenCard.transform.localScale = card1.transform.localScale;

                    objGoldenCard.GetComponent<CardController>().figura = Merge.resultado;
                    objGoldenCard.GetComponent<CardController>().sprRendererFigura.sprite = Merge.resultado.sprite;

                    objGoldenCard.GetComponent<CardFinalAnimation>().Conf(newMerge);

                    gameObjectCardSelect1.GetComponentInChildren<CardDrag>().cardFinalAnimation = objGoldenCard.GetComponent<CardFinalAnimation>();

                    cardFinal = true;
                }
                else
                {
                    FiguraMergeResultado = Merge.resultado;
                }

                //Invoke("ResultadoMerge", 5.1f);




                //Debug:
                //-Debug.LogWarning("Merge: " + Merge.resultado +" | Feito: "+Merge.count+" vezes");

                dealController.UpdateKnownMergesAndFiguresToDeal();


                bufunfa = dealController.dealPrice * 3;
                if (newMerge)
                {
                    //Add pontos extras por ser um merge novo:
                    PC.addXP(.5f);
                    soundController.TriggerNewElementSound();

                    FindObjectOfType<BankController>().StoreMoney(bufunfa);
                    CreateCoins(card1);
                    CreateCoins(card1);
                    CreateCoins(card1);

                    //Animação de popup de novo elemento:
                    gameObjectCardSelect1.GetComponentInChildren<CardDrag>().ConfPopupNewElement(Merge.resultado.sprite, cardFinal, card1.gameObject);

                    if (!cardFinal)
                    {

                        //Animação:
                        GameObject objGoldenCard = Instantiate(prefabGoldenCard, card1.transform.position, Quaternion.identity);
                        objGoldenCard.transform.SetParent(card1.gameObject.transform.parent);
                        objGoldenCard.transform.localScale = card1.transform.localScale;

                        objGoldenCard.GetComponent<CardController>().figura = Merge.resultado;
                        objGoldenCard.GetComponent<CardController>().statusCard = 10;
                        objGoldenCard.GetComponent<CardController>().UpdateSprites();

                        objGoldenCard.GetComponent<CardFinalAnimation>().Conf(newMerge);

                        gameObjectCardSelect1.GetComponentInChildren<CardDrag>().cardFinalAnimation = objGoldenCard.GetComponent<CardFinalAnimation>();

                    }
                }
                else
                {
                    FindObjectOfType<BankController>().StoreMoney(bufunfa / 2);
                    CreateCoins(card1);
                    CreateCoins(card1);
                }

            }

        }

        if (!mergeSuccess)
        {
            //Debug:
            //-Debug.LogWarning("Não deu merge");

            //Animação no card para indicar fail:
            card1.gameObject.GetComponent<AnimacaoFailMerge>().StartAnimation();
            card2.gameObject.GetComponent<AnimacaoFailMerge>().StartAnimation();
            // soundController.TriggerFailMergeSound();
            Instantiate(prefabXFailMerge, card2.transform.position, Quaternion.identity);

            //Adicionamos à lista, para sabermos que já tentamos essa:
            Merge novoMerge = ScriptableObject.CreateInstance<Merge>();
            novoMerge.a = card1.figura;
            novoMerge.b = card2.figura;
            novoMerge.resultado = figuraNull; //Aqui deixamos claro que é um merge sem resultado.
            novoMerge.descoberto = true;
            PC.allMerges.Add(novoMerge);

        }


        //Salvamos o progresso:
        PC.SaveMergeData();

        return figuraNull;
    }

    public void inMergeFinish()
    {
        //Finaliza Merge:
        PCSettings.inMerge = false;

        if (referenciaCard1 != null)
            referenciaCard1.inMerge = false;
        if (referenciaCard2 != null)
            referenciaCard2.inMerge = false;

        //Atualiza placar de possiveis merges:
        FindObjectOfType<PossibleToMerge>().updatePossibleToMerge();
    }

    public void ResultadoMerge()// metedod criado para auxiliar a dar um delay de s quando precisar limpar as cartas depois do merge
    {

        if (referenciaCard1 != null)
        {
            referenciaCard1.figura = FiguraMergeResultado;
            referenciaCard1.sprRendererFigura.sprite = FiguraMergeResultado.sprite;

            //Salva card:
            FindObjectOfType<PCSettings>().SaveCard(referenciaCard1);
        }
        if (referenciaCard2 != null)
        {
            referenciaCard2.figura = figuraNull;
            referenciaCard2.sprRendererFigura.sprite = figuraNull.sprite;

            //Salva card:
            FindObjectOfType<PCSettings>().SaveCard(referenciaCard2);

        }


        //Atualiza placar de possiveis merges:
        FindObjectOfType<PossibleToMerge>().updatePossibleToMerge();
    }

    public bool checkIfFigureIsFinal(Figure figure)
    {
        foreach (var merge in PC.allMerges)
        {

            if (merge.resultado == figuraNull) { continue; } //Se for resultado nulo, já pula pra próxima item do foreach;


            if ((merge.a != figure)
                 && (merge.b != figure)) { continue; } //Se não tiver essa figura, já pula pra próxima item do foreach;

            //Se chegamos aqui sem cair em um continue, é pq tem:
            return false;

        }

        //Se chegamos aqui sem cair no "return false;" é pq eu não sou usado em nenhum merge:
        return true;
    }

    public void CreateCoins(CardController card1)
    {
        //Animação:
        Vector3 createPosition = new Vector3(card1.transform.position.x + Random.Range(-.5f, .5f), card1.transform.position.y + Random.Range(-.5f, .5f), card1.transform.position.z);
        GameObject objCoin = Instantiate(prefabCoinAnimation, createPosition, Quaternion.identity);
        //objCoin.transform.SetParent(card1.gameObject.transform.parent);
        //objCoin.transform.localScale = card1.transform.localScale;

        objCoin.GetComponent<CoinAnimation>().Init();
    }
    // Verificar se é um merge conhecido (retorna Figure, sendo uma figuraNull caso não dê merge e null se ainda não foi testado):
    /*
    public Figure CheckMerge()
    {
        //Caso tenha apenas 1 ou 0 elementos nos seletores, descarta:
        int countElements = 0;
        if (Selector1.figura != null && Selector1.figura != figuraNull) { countElements++; }
        if (Selector2.figura != null && Selector2.figura != figuraNull) { countElements++; }
        if (Selector3.figura != null && Selector3.figura != figuraNull) { countElements++; }
        if (countElements < 2)
        {
            sprRen_Icone.sprite = spr_Botao;
            return null;
        }

        foreach (var Merge in PC.allMerges)
        {
            SelectorController checkA = null;
            SelectorController checkB = null;
            SelectorController checkC = null;

            //Check elemento "a" do merge:
            if (Merge.a == Selector1.figura) { checkA = Selector1; }
            else if (Merge.a == Selector2.figura) { checkA = Selector2; }
            else if (Merge.a == Selector3.figura) { checkA = Selector3; }
            else { continue; } // não achamos nenhuma figura correspondende nos seletores, continue no foreach para poupar processamento.

            //Check elemento "b" do merge:
            if (Merge.b == Selector1.figura && checkA != Selector1) { checkB = Selector1; }
            else if (Merge.b == Selector2.figura && checkA != Selector2) { checkB = Selector2; }
            else if (Merge.b == Selector3.figura && checkA != Selector3) { checkB = Selector3; }
            else { continue; } // não achamos nenhuma figura correspondende nos seletores, continue no foreach para poupar processamento.


            //Se chegamos aqui sem cair em nenhum "continue", é pq é um merge conhecido:
            //Então verificamos se é um merge sem resuldado:
            if (Merge.resultado == figuraNull)
            {
                sprRen_Icone.sprite = spr_BotaoNegado;
            }
            else
            {
                //Vemos se é um merge que já descobrimos ou que o jogador ainda vai testar:
                if (Merge.descoberto)
                {
                    sprRen_Icone.sprite = Merge.resultado.sprite;
                }
                else
                {
                    sprRen_Icone.sprite = spr_Botao;
                }
            }
            return Merge.resultado;
        }

        //Se chegamos até aqui, no final do foreach sem cair em um "return", é pq é um merge que ainda não foi testado:
        sprRen_Icone.sprite = spr_Botao;
        return null;
    }
    */
}
