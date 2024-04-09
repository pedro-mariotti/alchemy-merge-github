using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class DealController : MonoBehaviour
{

    [Header("Animações de tutorial:")]
    [SerializeField] private AnimacaoChamarAtencao tutorialDeal;


    [Header("Objetos de tutorial:")]
    public GameObject objOnboarding1;
    public GameObject objMaoOnboarding1;

    [Header("Compra:")]
    public TextController text;
    public int dealPrice = 2;

    [Header("Figuras iniciais e nula:")]
    public List<Figure> startFigures;
    public List<Figure> FiguresToOnboarding;
    [SerializeField] private Figure figuraNull;

    [Header("Conf gerais:")]
    [SerializeField] private PCSettings PC;
    [SerializeField] GameObject board;
    //LIXO: Apenas debug:
    public TextController textDebug;

    [Header("Conf internas:")]
    public List<GameObject> emptyCards = new();
    public List<Merge> knownMerges = new();
    public List<Figure> knownFigures = new();
    public List<Figure> allKnownFiguresIncludeFinal = new();
    public List<Figure> knownFiguresHigherTier = new();
    public List<tipoDeFigura> tipo;

    private int nLimiteMergeRepetido = 2;

    private int nLimiteTestDealFigures = 0;

    [Header("Moeda:")]
    public GameObject objMoeda;

    void Start()
    {
        //Carrega dealPrice salvo:
        if (PlayerPrefs.HasKey("dealPrice"))
        {
            #if !UNITY_EDITOR
            dealPrice = PlayerPrefs.GetInt("dealPrice");
            #endif
        }

        UpdateEmptyCards();
        UpdateKnownMergesAndFiguresToDeal();
        GameObject canvasObject = GameObject.Find("Canvas");
        Vector3 textInstantiatePos = transform.position + new Vector3(0f, 0.8f, 0f);

        updateTextValor(false);
    }

    void Update()
    {
        updateTextValor(false);
    }

    public void updateTextValor(bool isFree)
    {
        if (!isFree)
        {
            //preço em texto e no texto:
            string sprPrice = PC.ShortenNumber(dealPrice);
            text.ChangeText(sprPrice);

            //aparece moeda:
            objMoeda.GetComponent<SpriteRenderer>().enabled = true;

            //Posiciona:
            text.changePos(.6f, .5f);
            int quantidadeCaracteres = sprPrice.Length;
            switch (quantidadeCaracteres)
            {
                case 1:
                    objMoeda.transform.localPosition = new Vector3(.2f, objMoeda.transform.localPosition.y, objMoeda.transform.localPosition.z);
                    break;
                case 2:
                    objMoeda.transform.localPosition = new Vector3(.3f, objMoeda.transform.localPosition.y, objMoeda.transform.localPosition.z);
                    break;
                case 3:
                    objMoeda.transform.localPosition = new Vector3(.44f, objMoeda.transform.localPosition.y, objMoeda.transform.localPosition.z);
                    break;
                case 4:
                    objMoeda.transform.localPosition = new Vector3(.54f, objMoeda.transform.localPosition.y, objMoeda.transform.localPosition.z);
                    break;
                case 5:
                    objMoeda.transform.localPosition = new Vector3(.64f, objMoeda.transform.localPosition.y, objMoeda.transform.localPosition.z);
                    break;
                default:
                    break;
            }
        }
        else
        {
            //texto:
            text.ChangeText("Free!");

            //Some moeda:
            objMoeda.GetComponent<SpriteRenderer>().enabled = false;

            //Posiciona:
            text.changePos(.5f, .5f);
        }
    }

    public void startDealFigures()
    {
        nLimiteTestDealFigures = 0;
        DealFigures(true);

        if (PCSettings.onboardingStage==1)
        {
            //Avança no tutorial:
            PCSettings.onboardingStage = 2;
            PC.SaveGame();
            objOnboarding1.SetActive(false);
            objMaoOnboarding1.SetActive(false);
        }
    }

    public void DealFigures(bool limitarMergesRepetidos, bool limitarHigherTierFigures = true)
    {
        tutorialDeal.StopAnimacao();

        nLimiteTestDealFigures++;
        bool TentaDeNovo = false;
        int count = 0;
        
        //Isso aqui é para forçarmos o tutorial da lixeira:
        if (!PCSettings.tutorialTrashFinish && PCSettings.LevelPlayer>1)
        {
            //Apenas escolhe figuras aleatórias para as demais:
            foreach (GameObject cardFree in emptyCards)
            {
                //Fogo:
                Figure figuraSorteada = startFigures[2]; 

                //[ideia descartada até então] Forçar elemento de alto nível:
                //int index = Random.Range(0, knownFiguresHigherTier.Count);
                //figuraSorteada = knownFiguresHigherTier[index];


                cardFree.GetComponent<CardController>().figura = figuraSorteada;
                cardFree.GetComponent<CardController>().UpdateSprites(true);
            }

            //Atualiza placar de possiveis merges:
            FindObjectOfType<PossibleToMerge>().updatePossibleToMerge(); 

            return;
        }

        foreach (GameObject card in emptyCards)
        {
            count++;
            int index;
            Figure figuraSorteada;

            //MODO DE ESCOLHE DA FIGURA 1 [PRIMEIRO DEAL, TUTOTIAL]
                Debug.LogError("onboardingStage: "+PCSettings.onboardingStage);
                if (PCSettings.onboardingStage==1)
                {
                    figuraSorteada = FiguresToOnboarding[count];
                }
            //MODO DE ESCOLHE DA FIGURA 2 [CASO SEJA AS ÚLTIMAA 2, FORÇAR UTILIDADE]
                else if (count >= emptyCards.Count-1)
                {
                    List<Figure> FiguresToNewMerge = new();
                    List<Figure> FiguresToMerge = new();
                    List<Figure> FiguresToTest = GetFiguresNoTabuleiro();
                    
                    foreach (var merge in PC.allMerges)
                    {
                        //FILTRO 1:
                        //Se for resultado nulo, já pula pra próxima item do foreach:
                        if (merge.resultado == figuraNull) { continue; }

                        //FILTRO 2:
                        //Vamos descobrir se temos 1 dos elementos necessarios:
                        foreach (var figure in FiguresToTest)
                        {
                            if (merge.a == figure && (knownFiguresHigherTier.Contains(merge.b) || knownFigures.Contains(merge.b) ))
                            { 
                                if (merge.descoberto) { FiguresToMerge.Add(merge.b);}
                                else                  { FiguresToNewMerge.Add(merge.b);}
                            }
                            if (merge.b == figure && (knownFiguresHigherTier.Contains(merge.a) || knownFigures.Contains(merge.a) ))
                            { 
                                if (merge.descoberto) { FiguresToMerge.Add(merge.a);}
                                else                  { FiguresToNewMerge.Add(merge.a);}
                            }
                        }
                    }

                    //Caso tenha a possibilidade de entregar um merge novo pro jogador:
                    if (FiguresToNewMerge.Count > 0)
                    {
                        index = Random.Range(0, FiguresToNewMerge.Count);
                        figuraSorteada = FiguresToNewMerge[index];
                    }
                    //Caso não tenha a possibilidade de entregar um merge novo pro jogador:
                    else
                    {
                        index = Random.Range(0, FiguresToMerge.Count);
                        figuraSorteada = FiguresToMerge[index];
                    }
                    
                }
            //MODO DE ESCOLHE DA FIGURA 3 [ALEATÓRIO]
                else
                {
                    //Vamos decidir se será HigherTier ou não:
                    if (limitarHigherTierFigures)
                    {
                        float Sorteio = Random.value;
                        if (Sorteio < PCSettings.ChanceToSpawnHigherTierElement)
                        {
                            index = Random.Range(0, knownFiguresHigherTier.Count);
                            figuraSorteada = knownFiguresHigherTier[index];
                        }
                        else
                        {
                            index = Random.Range(0, knownFigures.Count);
                            figuraSorteada = knownFigures[index];
                        }
                    }
                    //Sem essa limitação: 
                    else
                    {
                        List<Figure> allKnownFigures = new();
                        allKnownFigures.AddRange(knownFigures);
                        allKnownFigures.AddRange(knownFiguresHigherTier);

                        index = Random.Range(0, allKnownFigures.Count);
                        figuraSorteada = allKnownFigures[index];
                    }
                }

            //Atualiza figura sorteada no card:
            card.GetComponent<CardController>().figura = figuraSorteada;

            //Controle anti repetição de figuras/elementos:
            //Primeiro vemos quantos ficou no tabuleiro:
            List<Figure> figuresNoTabuleiro = GetFiguresNoTabuleiro();
            int countFiguresIguais = 0;
            foreach (var figure in figuresNoTabuleiro)
            {
                if (figure == figuraSorteada)
                {
                    countFiguresIguais++;
                }
            }
            //Depois passamos merge por merge, e vemos qual merge exige mais dessa figura (como mar que são 3 águas):
            int countFiguresIguaisEmMerge = 0;
            foreach (var merge in PC.allMerges)
            {
                if (merge.resultado == figuraNull) { continue; } //Se for resultado nulo, já pula pra próxima item do foreach;
                if (merge.count >= nLimiteMergeRepetido && limitarMergesRepetidos) { continue; } //Limit de vezes que vamos tolerar o mesmo merge;

                int localCount = 0;
                if (merge.a == figuraSorteada) { localCount++; }
                if (merge.b == figuraSorteada) { localCount++; }
                if (localCount > countFiguresIguaisEmMerge)
                { countFiguresIguaisEmMerge = localCount; }
            }
            //Por fim, comparamos se tem mais dessa figura que o necessário:
            if (countFiguresIguais > countFiguresIguaisEmMerge)
            {
                //Tenta de novo (apenas com essa figura):
                card.GetComponent<CardController>().figura = figuraNull;
                TentaDeNovo = true;
            }

            //Controle anti jogo sem saída:
            //E controle anti repetição de merges muito repetidos:
            if (count >= emptyCards.Count) //Caso eu seja a última figura que está sendo escolhida
            {
                if (CheckSemSlotsParaCompra())  //E caso não tenha slots de cards prontos para serem comprados
                {
                    //Achou um merge para essa escolha atual de figura:
                    bool achou = false;
                    //Percorreremos merge por merge:
                    foreach (var merge in PC.allMerges)
                    {
                        //FILTRO 1:
                        if (merge.resultado == figuraNull) { continue; } //Se for resultado nulo, já pula pra próxima item do foreach;

                        //FILTRO 2:
                        if (merge.count >= nLimiteMergeRepetido && limitarMergesRepetidos) { continue; } //Limit de vezes que vamos tolerar o mesmo merge;

                        //FILTRO 3:
                        //Agora um filtro que garante que esse merge seja possível com as figuras que já temos no tabuleiro:
                        bool checkA = false;
                        bool checkB = false;
                        List<Figure> figuresNoTabuleiroESlots = GetFiguresNoTabuleiroENosSlotsCompraveis();
                        foreach (var figure in figuresNoTabuleiroESlots)
                        {
                            if ((merge.a == figure || merge.a == figuraNull || merge.a == null) && !checkA) { checkA = true; }
                            else if ((merge.b == figure || merge.b == figuraNull || merge.b == null) && !checkB) { checkB = true; }
                        }

                        if (checkA && checkB)
                        {
                            Debug.Log("Merge possível: " + merge.resultado);
                            textDebug.ChangeText("DEBUG: Merge possível: " + merge.resultado);
                            achou = true;
                            break; //Já achamos um merge, então pode quebrar o foreach.
                        }
                    }

                    //Não encontramos um merge:
                    if (!achou)
                    {
                        Debug.Log("Nenhum Merge possível...");
                        //Tenta de novo (com todos os cards desse deal):
                        foreach (GameObject cardFree in emptyCards)
                        {
                            cardFree.GetComponent<CardController>().figura = figuraNull;
                            cardFree.GetComponent<CardController>().UpdateSprites(true);
                        }
                        TentaDeNovo = true;
                    }

                }
                else
                {
                    textDebug.ChangeText("É possível comprar um novo card/slot...");
                }
            }


            card.GetComponent<CardController>().UpdateSprites(true);

            textDebug.ChangeText("DEBUG: XP: " + PCSettings.LevelPlayer_XP);

        }

        //Não encontramos um merge, tenta de novo:
        if (TentaDeNovo && PCSettings.onboardingStage!=1)
        {
            //tentamos COM limitação de merges repetidos:
            if (nLimiteTestDealFigures < 2)
            {
                UpdateEmptyCards();
                DealFigures(true);
            }
            //tentamos SEM limitação de merges repetidos:
            else if (nLimiteTestDealFigures < 0)
            {
                UpdateEmptyCards();
                DealFigures(false);
            }
            //tentamos SEM limitação de Higher-Tier figures:
            else if (nLimiteTestDealFigures < 0)
            {
                UpdateEmptyCards();
                DealFigures(true, false);
            }
            //Desistimos...:
            //Aqui vamos pegar a combinação com menos formações (provavelmente uma nova), e entregar ao jogador:
            else
            {
                //Vamos descobrir qual o melhor merge pra gente:
                //Percorreremos merge por merge:
                Merge mergeEscolhido = ScriptableObject.CreateInstance<Merge>();
                mergeEscolhido.count = 9999;
                List<Figure> figuresNoTabuleiroESlots = GetFiguresNoTabuleiroENosSlotsCompraveis();
                foreach (var merge in PC.allMerges)
                {
                    //FILTRO 1:
                    //Se for resultado nulo, já pula pra próxima item do foreach:
                    if (merge.resultado == figuraNull) { continue; }

                    //FILTRO 2:
                    //Agora vamos ver se esse é um merge que foi feito menos vezes que o nosso "mergeEscolhido" atual:
                    if (merge.count > mergeEscolhido.count) { continue; }

                    //FILTRO 3: 
                    //Vemos se temos os elementos necessários para esse merge:
                    if (!((knownFigures.Contains(merge.a) || knownFiguresHigherTier.Contains(merge.a))
                          && (knownFigures.Contains(merge.b) || knownFiguresHigherTier.Contains(merge.b))))
                    { continue; }

                    //FILTRO 4:
                    //Vamos descobrir se é possível induzir a criação desse merge:
                    //Tem 2 espaços vazios:
                    if (emptyCards.Count >= 2)
                    {
                        mergeEscolhido = merge;
                        continue;
                    }
                    //Tem apenas um espaço, porém já temos um dos elementos:
                    bool checkA = false;
                    bool checkB = false;
                    foreach (var figure in figuresNoTabuleiroESlots)
                    {
                        if ((merge.a == figure)) { checkA = true; }
                        else if ((merge.b == figure)) { checkB = true; }
                    }

                    if (checkA || checkB)
                    {
                        mergeEscolhido = merge;
                        continue;
                    }


                }

                //Deu ruim grandão:
                if (mergeEscolhido.count == 9999)
                {
                    Debug.Log("[GAME OVER] Nenhum merge possível.");
                    //Precisamos pensar no que fazer quando isso ocorrer!
                    //Acho que o ideal seria indicar ao jogador que use a bomba.
                }
                else
                {

                    Debug.Log("[FIM DE LINHA] Merge possível: " + mergeEscolhido.resultado);
                    textDebug.ChangeText("DEBUG: Merge possível: " + mergeEscolhido.resultado);

                    //Agora que já temos o merge escolhido em "mergeEscolhido" para ser usado, vamos forçá-lo:
                    if (mergeEscolhido.a != mergeEscolhido.b)
                    {
                        if (!figuresNoTabuleiroESlots.Contains(mergeEscolhido.a))
                        {
                            emptyCards[0].GetComponent<CardController>().figura = mergeEscolhido.a;
                            emptyCards[0].GetComponent<CardController>().UpdateSprites(true);
                            emptyCards.RemoveAt(0);
                        }
                        if (!figuresNoTabuleiroESlots.Contains(mergeEscolhido.b))
                        {
                            emptyCards[0].GetComponent<CardController>().figura = mergeEscolhido.b;
                            emptyCards[0].GetComponent<CardController>().UpdateSprites(true);
                            emptyCards.RemoveAt(0);
                        }
                    }
                    else
                    {
                        Debug.Log("[IGUAIS!]");

                        if (emptyCards.Count >= 2)
                        {
                            emptyCards[0].GetComponent<CardController>().figura = mergeEscolhido.a;
                            emptyCards[0].GetComponent<CardController>().UpdateSprites(true);
                            emptyCards.RemoveAt(0);

                            emptyCards[0].GetComponent<CardController>().figura = mergeEscolhido.a;
                            emptyCards[0].GetComponent<CardController>().UpdateSprites(true);
                            emptyCards.RemoveAt(0);
                        }
                        else
                        {
                            emptyCards[0].GetComponent<CardController>().figura = mergeEscolhido.a;
                            emptyCards[0].GetComponent<CardController>().UpdateSprites(true);
                            emptyCards.RemoveAt(0);
                        }
                    }
                }

                //Apenas escolhe figuras aleatórias para as demais:
                foreach (GameObject cardFree in emptyCards)
                {
                    int index;
                    Figure figuraSorteada;

                    //Vamos decidir se será HigherTier ou não:
                    float Sorteio = Random.value;
                    if (Sorteio < PCSettings.ChanceToSpawnHigherTierElement)
                    {
                        index = Random.Range(0, knownFiguresHigherTier.Count);
                        figuraSorteada = knownFiguresHigherTier[index];
                    }
                    else
                    {
                        index = Random.Range(0, knownFigures.Count);
                        figuraSorteada = knownFigures[index];
                    }

                    cardFree.GetComponent<CardController>().figura = figuraSorteada;
                    cardFree.GetComponent<CardController>().UpdateSprites(true);
                }

            }
        }
        else
        {
            emptyCards.Clear();
        }

        //Atualiza placar de possiveis merges:
        FindObjectOfType<PossibleToMerge>().updatePossibleToMerge();
    }

    public void UpdateKnownMergesAndFiguresToDeal()
    {
        //Primeiro, knownMerges:
        //usando LINQ para fazer uma busca na lista de merges para encontrar aqueles que foram descobertos e os colocando na lista de knownMerges
        knownMerges = PC.allMerges.Where(merge => merge.descoberto == true).ToList();

        //Limpando os resultados nulos:
        for (int i = knownMerges.Count - 1; i >= 0; i--)
        {
            if (knownMerges[i].resultado == figuraNull)
            {
                knownMerges.RemoveAt(i);
            }
        }

        //Segundo, "knownFigures":
        //Limpa "knownFigures":
        knownFigures.Clear();
        //Percorre todos os merges conhecidos...
        foreach (var merge in knownMerges)
        {
            //e adiciona seus resultados a "knownFigures":
            knownFigures.Add(merge.resultado);
        }

        //Em "knownFigures", adiciona os elementos iniciais (água, ar, terra e fogo)
        knownFigures.AddRange(startFigures);

        //Em "knownFigures", adiciona os elementos desbloqueados via LevelUp
        knownFigures.AddRange(PC.figuresExtrasSlot);
        knownFigures.AddRange(PC.figuresExtrasLevel);

        //EXTRA :: Aproveitamos para ter allKnownFiguresIncludeFinal, antes do filtro:
        allKnownFiguresIncludeFinal.Clear();
        allKnownFiguresIncludeFinal.AddRange(knownFigures);

        //Terceiro, hora de filtrar "knownFigures" com apenas figuras úteis para merge:
        //Variável local usada para identidicar se achou um merge útil:
        bool achou;
        //Salvamos temporariamente todas as figuras conhecidas (sem filtro):
        List<Figure> allKnownFigures = new();
        allKnownFigures.AddRange(knownFigures);
        //Percorremos todo knownFigures, aplicando o filtro:
        for (int i = knownFigures.Count - 1; i >= 0; i--)
        {
            //Começamos com não achou por padrão;
            achou = false;

            //Vamos percorrer todos os merges que existem no jogo:
            foreach (var merge in PC.allMerges)
            {
                //Se for resultado nulo no merge, já pula pra próxima item do foreach;
                if (merge.resultado == figuraNull) { continue; }

                //Se não tiver essa figura que estamos testando, já pula pra próxima item do foreach;
                if ((merge.a != knownFigures[i])
                    && (merge.b != knownFigures[i])) { continue; }

                //Agora o filtro que garante que esse merge seja possível com as outras figuras que já temos:
                bool checkA = false;
                bool checkB = false;
                foreach (var figure in allKnownFigures)
                {
                    if (merge.a == figure || merge.a == figuraNull || merge.a == null) { checkA = true; }
                    if (merge.b == figure || merge.b == figuraNull || merge.b == null) { checkB = true; }
                }

                //Se achou, podemos sair do foreach;
                if (checkA && checkB)
                { achou = true; break; }
            }

            //Se chegamos aqui, e não encontramos nenhum merge possível, essa figura é descartada:
            if (!achou)
            {
                knownFigures.RemoveAt(i);
            }

            //Próxima figura, fim do atual loop for;
        }

        //Quarto e por fim, hora de separar "knownFiguresHigherTier" de "knownFigures":
        //Variável local usada para identidicar se achou uma figura HigherTier:
        bool HigherTier;

        //Limpa "knownFiguresHigherTier":
        knownFiguresHigherTier.Clear();

        //Percorremos todo knownFigures, aplicando o filtro:
        for (int i = knownFigures.Count - 1; i >= 0; i--)
        {
            //Começamos com HigherTier, por padrão;
            HigherTier = true;

            //As figuras liberadas por LevelUp, Slot comprados ou iniciais não são HigherTier, mesmo não tendo sido usadas ainda:
            if (PC.figuresExtrasSlot.Contains(knownFigures[i]) || PC.figuresExtrasLevel.Contains(knownFigures[i]) || startFigures.Contains(knownFigures[i]))
            {
                HigherTier = false;
            }
            else
            {
                //Vamos percorrer todos os merges conhecidos (ja vem filtrado de resultados nulos):
                foreach (var merge in knownMerges)
                {
                    //Se não tiver essa figura que estamos testando, já pula pra próxima item do foreach;
                    if ((merge.a != knownFigures[i])
                        && (merge.b != knownFigures[i])) { continue; }

                    //Se passou do continue acima, é pq é uma figura que já foi usada em outro merge conhecido,
                    //significa que ela não é Higher-Tier;
                    HigherTier = false;
                    break;
                }
            }

            //Se for HigherTier, add ela em "knownFiguresHigherTier" e remove de "knownFigures":
            if (HigherTier)
            {
                knownFiguresHigherTier.Add(knownFigures[i]);
                knownFigures.RemoveAt(i);
            }

            //Próxima figura, fim do atual loop for;
        }

    }

    public bool UpdateEmptyCards()
    {
        emptyCards.Clear();

        for (int i = 0; i < board.transform.childCount; i++)
        {
            Transform currentLine = board.transform.GetChild(i);
            for (int j = 0; j < currentLine.childCount; j++)
            {
                GameObject currentCard = currentLine.GetChild(j).gameObject;
                if (currentCard.GetComponent<CardController>() != null)
                {
                    if (currentCard.GetComponent<CardController>().statusCard == 3 && currentCard.GetComponent<CardController>().figura == figuraNull)
                    {
                        emptyCards.Add(currentCard);
                    }
                }
            }
        }
        //Debug.Log("emptyCards.Count: " + emptyCards.Count);
        if (emptyCards.Count == 0)
        {
            return false;
        }
        return true;


    }


    public List<Figure> GetFiguresNoTabuleiro()
    {
        List<Figure> retorno = new();

        for (int i = 0; i < board.transform.childCount; i++)
        {
            Transform currentLine = board.transform.GetChild(i);
            for (int j = 0; j < currentLine.childCount; j++)
            {
                GameObject currentCard = currentLine.GetChild(j).gameObject;
                if (currentCard.GetComponent<CardController>().statusCard == 3 && currentCard.GetComponent<CardController>().figura != figuraNull)
                {
                    retorno.Add(currentCard.GetComponent<CardController>().figura);
                }
            }
        }

        return retorno;

    }
    // public List<GameObject> GetCardsComFigurasTabuleiro(int? repeatAmt)
    // {
    //     List<GameObject> retorno = new();

    //     for (int i = 0; i < board.transform.childCount; i++)
    //     {
    //         Transform currentLine = board.transform.GetChild(i);
    //         for (int j = 0; j < currentLine.childCount; j++)
    //         {
    //             GameObject currentCard = currentLine.GetChild(j).gameObject;
    //             if (currentCard.GetComponent<CardController>().statusCard == 3 && currentCard.GetComponent<CardController>().figura != figuraNull)
    //             {
    //                 retorno.Add(currentCard);
    //                 if (repeatAmt != null && repeatAmt <= retorno.Count)
    //                 {
    //                     break;
    //                 }
    //             }
    //         }
    //         if (repeatAmt != null && repeatAmt <= retorno.Count)
    //         {
    //             break;
    //         }
    //     }

    //     return retorno;
    // }


    public List<Figure> GetFiguresNoTabuleiroENosSlotsCompraveis()
    {
        List<Figure> retorno = GetFiguresNoTabuleiro();

        for (int i = 0; i < board.transform.childCount; i++)
        {
            Transform currentLine = board.transform.GetChild(i);
            for (int j = 0; j < currentLine.childCount; j++)
            {
                GameObject currentCard = currentLine.GetChild(j).gameObject;
                if (currentCard.GetComponent<CardController>().statusCard == 2              //é um slot compravel
                 && currentCard.GetComponent<CardController>().DesbFigura != figuraNull     //possui uma figura para dropar
                 && currentCard.GetComponent<CardController>().DesbFigura != null)          //confirma possui uma figura para dropar
                {
                    //agora filtra valor+o custo
                    int QuantoTenho = FindObjectOfType<BankController>().GetBankValue();
                    int QuantoPreciso = currentCard.GetComponent<CardController>().valor + (1);
                    Debug.Log("QuantoTenho: " + QuantoTenho + " | QuantoPreciso: " + QuantoPreciso);
                    if (QuantoTenho >= QuantoPreciso)
                    {
                        retorno.Add(currentCard.GetComponent<CardController>().DesbFigura);
                    }
                }
            }
        }

        return retorno;

    }


    public int GetPrecoDosSlotsCompraveisComDrop()
    {
        int retorno = 0;

        for (int i = 0; i < board.transform.childCount; i++)
        {
            Transform currentLine = board.transform.GetChild(i);
            for (int j = 0; j < currentLine.childCount; j++)
            {
                GameObject currentCard = currentLine.GetChild(j).gameObject;
                if (currentCard.GetComponent<CardController>().statusCard == 2              //é um slot compravel
                 && currentCard.GetComponent<CardController>().DesbFigura != figuraNull     //possui uma figura para dropar
                 && currentCard.GetComponent<CardController>().DesbFigura != null)          //confirma possui uma figura para dropar
                {
                    int QuantoPreciso = currentCard.GetComponent<CardController>().valor + (1);
                    retorno += QuantoPreciso;
                }
            }
        }
        Debug.Log("Preço drops: " + retorno);
        return retorno;

    }

    private bool CheckSemSlotsParaCompra()
    {
        for (int i = 0; i < board.transform.childCount; i++)
        {
            Transform currentLine = board.transform.GetChild(i);
            for (int j = 0; j < currentLine.childCount; j++)
            {
                GameObject currentCard = currentLine.GetChild(j).gameObject;
                //Card liberado para compra:
                if (currentCard.GetComponent<CardController>().statusCard == 2)
                {
                    //Card que não dropa um novo elemento:
                    if (currentCard.GetComponent<CardController>().DesbFigura == figuraNull || currentCard.GetComponent<CardController>().DesbFigura == null)
                    {
                        //agora filtra valor+o custo
                        int QuantoTenho = FindObjectOfType<BankController>().GetBankValue();
                        int QuantoPreciso = currentCard.GetComponent<CardController>().valor + (dealPrice * 3) + GetPrecoDosSlotsCompraveisComDrop();
                        Debug.Log("QuantoTenho: " + QuantoTenho + " | QuantoPreciso: " + QuantoPreciso);
                        if (QuantoTenho >= QuantoPreciso)
                        {
                            return false;
                        }
                    }
                }

            }
        }

        return true;

    }
}
