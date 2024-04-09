using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class CardWikiController : MonoBehaviour
{

    [SerializeField] private Figure figuraNull;

    public GameObject prefab;
 

    public PCSettings PC;

    public List<Merge> knownMerges = new();
    public List<Figure> allFigures;
    public List<GameObject> cards;
    public List<GameObject> cards2;
    public List<GameObject> cards3;
    public List<Merge> dist;
    

    public List<Figure> startFigures;
    public Figure figura;
    public Figure figNull;
    public SpriteRenderer sprRendererFigura;
    public Sprite sprGoldCard;
    public Sprite sprGreyCard;
    private Sprite Gold;
    public TextController textOld;
    public TextControllerV2 text;
    public Figure AtualFigura;
    public static bool ControlPop = false;
    
    //Usado para criar o distancimento dos cards:
    private float xTamanhoCard;
    private float yTamanhoCard;

    //Sprite figura ?
    public Sprite figuraInterrogacao;

    //4 elementos iniciais:
    public List<Figure> elementosInicais;
    //sprites do popup:
    public List<Sprite> spritesPopupWiki;
    
    
    

    // Start is called before the first frame update
    void Start()
    {
        figura = figuraNull;
        knownMerges = FindObjectOfType<DealController>().knownMerges;
        allFigures = FindObjectOfType<PCSettings>().allFigures;

        PC = GameObject.Find("PC").GetComponent<PCSettings>();

        //Usado para criar o distancimento dos cards:
        xTamanhoCard = 2f;
        yTamanhoCard = 2.5f;
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    public List<GameObject> Instanciador(int tag)
    {
        int count = 0; // contador
        float yPosition = 0f; // posição y inicial
        float xOffset = 0f; // deslocamento x
        bool instancia;

        knownMerges = FindObjectOfType<DealController>().knownMerges;
        
        List<Figure> allKnownFigures = new();
        allKnownFigures.AddRange(FindObjectOfType<DealController>().allKnownFiguresIncludeFinal);

        
        GameObject CardLocal= GameObject.Find("CardsInWiki");
        
        CardLocal.GetComponent<ScrollControl>().restart();
       
/////////////////VERDE\\\\\\\\\\\\\\\\\
        if (tag == 2) 
        {
            instancia=true;
            
            foreach(Figure figure in allKnownFigures)
            {
                foreach(Merge merge in knownMerges)
                {
                    if(merge.resultado == figure)
                    {
                        if(checkIfFigureIsFinal(merge.resultado))
                        {
                            instancia=false;
                            break;
                        }
                        else
                        {
                            instancia=true;
                            break;
                        }
                    }
                }

                //Add os 4 iniciais:
                if(elementosInicais.Contains(figure))
                {
                    instancia=true;
                }

                if(instancia)
                {
                    // Instancia o card
                    GameObject card = Instantiate(prefab, Vector3.zero, Quaternion.identity);

                    SetPositionCard(card, xOffset, yPosition, CardLocal);

                    card.GetComponent<TextControllerV2>().ChangeText(figure.cardName);
                    SpriteRenderer sprRenderer = card.transform.GetChild(1).GetComponent<SpriteRenderer>();
                    sprRenderer.sprite = figure.sprite;

                    // Adiciona o objeto instanciado na lista
                    cards.Add(card);

                    // Incrementa o contador
                    count++;

                    // Atualiza a posição x para a próxima instância
                    xOffset += xTamanhoCard;

                    // Se o contador atingir 3, cria uma nova linha
                    if (count == 3)
                    {
                        count = 0; // reinicia o contador
                        xOffset = 0f; // reinicia o deslocamento x
                        yPosition -= yTamanhoCard; // atualiza a posição y para criar uma nova linha
                    }
                }
            }
            
            return cards;
        }
//////////////////AZUL\\\\\\\\\\\\\\\\\\\\
        else if (tag == 1) 
        {
            PC = GameObject.Find("PC").GetComponent<PCSettings>();

            dist.Clear();
            cards2.Clear();

            //Iniciamos as listas que compoem Azul:
            List<Merge> NearToKnownMerges = new();
            List<Merge> FarToKnownMerges = new();
            
            //Vamos ver quais figuras nos conhecemos
            allKnownFigures.Clear();
            allKnownFigures.AddRange(FindObjectOfType<DealController>().allKnownFiguresIncludeFinal);


            //Percorre todas as merges do jogo:
            foreach (Merge merge in PC.allMerges)
            {   
                //Se for resultado nulo, já pula pra próxima item do foreach;
                if (merge.resultado == PC.figuraNull) {continue;} 
                
                //Se eu já conheço, não serve pro azul:
                if(allKnownFigures.Contains(merge.resultado)) {continue;}

                //Serve! Agora vamos descobrir se é Near ou Far:
                if (allKnownFigures.Contains(merge.a) && allKnownFigures.Contains(merge.b))
                {
                    //Proximo de descobrir:
                    NearToKnownMerges.Add(merge);
                    //Debug.Log("Está proximo de descobrir: "+ merge.resultado);
                }
                else
                {
                    //Longe de descobrir:
                    FarToKnownMerges.Add(merge);
                    //Debug.Log("Está longe de descobrir: "+ merge.resultado);
                }
            }

            //Agora vamos criar quem ta perto:
            foreach(Merge merge in NearToKnownMerges)
            { 
                        GameObject card = Instantiate(prefab, Vector3.zero, Quaternion.identity);

                        SpriteRenderer sprRenderer = card.transform.GetChild(1).GetComponent<SpriteRenderer>();  
                 
                        sprRenderer.sprite = merge.resultado.sprite;

                        if (merge.hint)
                        {
                            sprRenderer.color = new Color(1f,1f,1f,1f);
                            card.GetComponent<TextControllerV2>().ChangeText(merge.resultado.cardName);
                        }
                        else
                        {
                            sprRenderer.color = new Color(0f,0f,0f,.5f);
                            card.GetComponent<TextControllerV2>().ChangeText("??????");
                        }
                        

                        SetPositionCard(card, xOffset, yPosition, CardLocal);

                        // Adiciona o objeto instanciado na lista
                        cards2.Add(card);

                        // Incrementa o contador
                        count++;

                        // Atualiza a posição x para a próxima instância
                        xOffset += xTamanhoCard;

                        // Se o contador atingir 3, cria uma nova linha
                        if (count == 3)
                        {
                            count = 0; // reinicia o contador
                            xOffset = 0f; // reinicia o deslocamento x
                            yPosition -= yTamanhoCard; // atualiza a posição y para criar uma nova linha
                        }
            }

            //Agora vamos criar quem ta longe:
            foreach(Merge merge in FarToKnownMerges)
            { 
                        GameObject card = Instantiate(prefab, Vector3.zero, Quaternion.identity);

                        SpriteRenderer sprRenderer = card.transform.GetChild(1).GetComponent<SpriteRenderer>();  
                        sprRenderer.sprite = figuraInterrogacao;
                        sprRenderer.color = new Color(0f,0f,0f,.5f);

                        card.GetComponent<TextControllerV2>().ChangeText("??????");

                        SetPositionCard(card, xOffset, yPosition, CardLocal);

                        // Adiciona o objeto instanciado na lista
                        cards2.Add(card);

                        // Incrementa o contador
                        count++;

                        // Atualiza a posição x para a próxima instância
                        xOffset += xTamanhoCard;

                        // Se o contador atingir 3, cria uma nova linha
                        if (count == 3)
                        {
                            count = 0; // reinicia o contador
                            xOffset = 0f; // reinicia o deslocamento x
                            yPosition -= yTamanhoCard; // atualiza a posição y para criar uma nova linha
                        } 
            }
 
            return cards2;
        }
//////// AMARELA \\\\\\\\\\
        if (tag == 3) 
        {   

            instancia=false;

            foreach(Figure figure in allKnownFigures)
            {   
               
                foreach(Merge merge in knownMerges)
                {  

                    if(merge.resultado == figure)
                    {
                        if(checkIfFigureIsFinal(merge.resultado))
                        {
                            instancia=true;
                            break;
                        }
                        else
                        {
                            instancia=false;
                            break;
                        }
                    }
                }

                //tira os 4 iniciais:
                if(elementosInicais.Contains(figure))
                {
                    instancia=false;
                }
                
                if(instancia)
                {
                    // Instancia o card
                    GameObject card = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                    
                    SpriteRenderer sprRenderer = card.transform.GetChild(1).GetComponent<SpriteRenderer>();
                    sprRenderer.sprite = figure.sprite;

                    SpriteRenderer sprBG = card.transform.GetChild(2).GetComponent<SpriteRenderer>();
                    sprBG.sprite = sprGoldCard;

                    card.GetComponent<TextControllerV2>().ChangeText(figure.cardName);

                    SetPositionCard(card, xOffset, yPosition, CardLocal);
                            
                    // Adiciona o objeto instanciado na lista
                    cards.Add(card);

                    // Incrementa o contador
                    count++;

                    // Atualiza a posição x para a próxima instância
                    xOffset += xTamanhoCard;

                    // Se o contador atingir 3, cria uma nova linha
                    if (count == 3)
                    {
                        count = 0; // reinicia o contador
                        xOffset = 0f; // reinicia o deslocamento x
                        yPosition -= yTamanhoCard; // atualiza a posição y para criar uma nova linha
                    }
                }
            }
            
            return cards;
        }

            return cards;
    }


    public void InstancePop(GameObject card)
    {

        SpriteRenderer popup_a = GameObject.Find("Wiki_popup_a").GetComponent<SpriteRenderer>();
        SpriteRenderer popup_b = GameObject.Find("Wiki_popup_b").GetComponent<SpriteRenderer>();
        SpriteRenderer popup_r = GameObject.Find("Wiki_popup_r").GetComponent<SpriteRenderer>();

        SpriteRenderer popup_bg = GameObject.Find("Wiki_popup_bg").GetComponent<SpriteRenderer>();


        SpriteRenderer figura = card.transform.GetChild(1).GetComponent<SpriteRenderer>();

        //elemento conquistado (sem receita):
        foreach(Figure elementoInicial in elementosInicais)
        {
            if(elementoInicial.sprite==figura.sprite)
            {
                popup_r.sprite = figura.sprite;
                popup_r.color  = new Color(1f,1f,1f,1f);

                popup_a.sprite = null;
                popup_b.sprite = null;

                popup_bg.sprite = spritesPopupWiki[1];
                return;
            }
        }

        //elemento com receita
        foreach(Merge merge in PC.allMerges)
        {
            if(merge.resultado.sprite==figura.sprite || figura.sprite == figuraInterrogacao) 
            {
                //Achamos o merge!

                //Agora vamos descobrir se é um descoberto, ou não descoberto mas com dica aberta:
                if (merge.descoberto)
                {
                    popup_r.sprite = figura.sprite;
                    popup_r.color  = new Color(1f,1f,1f,1f);

                    popup_a.sprite = merge.a.sprite;

                    popup_b.sprite = merge.b.sprite;

                    popup_bg.sprite = spritesPopupWiki[0];
                }
                else if (merge.hint)
                {
                    popup_r.sprite = figura.sprite;
                    popup_r.color  = new Color(1f,1f,1f,1f);

                    popup_a.sprite = figuraInterrogacao;

                    popup_b.sprite = figuraInterrogacao;

                    popup_bg.sprite = spritesPopupWiki[0];
                }
                else 
                {
                    popup_r.sprite = figura.sprite;
                    popup_r.color  = new Color(0f,0f,0f,.5f); 

                    popup_a.sprite = figuraInterrogacao;

                    popup_b.sprite = figuraInterrogacao;

                    popup_bg.sprite = spritesPopupWiki[0];
                }

                //Debug.Log("Este é o merge: "+ merge);
                return; 
            }
        }

        //elemento sem receita:
        popup_r.sprite = figura.sprite;

        popup_a.sprite = null;
        popup_b.sprite = null;

        popup_bg.sprite = spritesPopupWiki[1];

        return;

    }
    void SetSortingOrderAndLayer(GameObject card, int order, string layer)
    {
        var spriteRenderers = card.GetComponentsInChildren<SpriteRenderer>();
        var canvas = card.GetComponentInChildren<Canvas>();

        foreach (var sr in spriteRenderers)
        {
            sr.sortingOrder = order;
            sr.sortingLayerName = layer;
        }

        if (canvas != null)
        {
            canvas.sortingOrder = order;
            canvas.sortingLayerName = layer;
        }
    }

    void SetPositionCard(GameObject card, float xOffset, float yPosition, GameObject CardLocal)
    {
        card.transform.SetParent(CardLocal.transform);
        card.transform.GetChild(4).localScale = new Vector3(1.4f,1.4f,1);
        card.transform.localScale = new Vector3(1f,1f,1f);
        card.transform.localPosition = new Vector3(3.5f+xOffset,yPosition,0f);
    }
    
    public bool checkIfFigureIsFinal(Figure figure)
    {
        foreach (var mergeToCheck in PC.allMerges)
        {

            if (mergeToCheck.resultado == PC.figuraNull) {continue;} //Se for resultado nulo, já pula pra próxima item do foreach;


            if (    (mergeToCheck.a != figure)
                 && (mergeToCheck.b != figure) ) {continue;} //Se não tiver essa figura, já pula pra próxima item do foreach;

            //Se chegamos aqui sem cair em um continue, é pq tem:
            return false;
            
        }

        //Se chegamos aqui sem cair no "return false;" é pq eu não sou usado em nenhum merge:
        return true;
    }
}
