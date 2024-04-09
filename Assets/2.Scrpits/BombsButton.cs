using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VibrationRDG;

public class BombsButton : MonoBehaviour
{

    [Header("Animações de tutorial:")]
    [SerializeField] private AnimacaoChamarAtencao tutorialBombs;

    private BoxCollider2D boxCollider;

    [Header("Conf. Gerais:")]
    [SerializeField] private Figure figuraNull;
    [SerializeField] GameObject board;
    [SerializeField] GameObject tutorialHand; //coloca o tutorial aqui pra bloquear a bomba caso esteja no tutorial
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Não clica com lockGame:
        if (PCSettings.lockGame) { return; }

        // Checa se o player tocou no botão
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

            List<GameObject> CardsNoTabuleiro = GetCardsNoTabuleiro();

            if (boxCollider == Physics2D.OverlapPoint(touchPos) && !CardsNoTabuleiro[0].GetComponent<CardController>().initBombAnimation && !PCSettings.inAnimationMerge && !tutorialHand.activeSelf)
            {
                //Para tutorial em mim:
                tutorialBombs.StopAnimacao();

                //se tocou, limpa todas as figuras:   

                foreach (var card in CardsNoTabuleiro)
                {
                    card.GetComponent<CardController>().figura = figuraNull;
                    card.GetComponent<CardController>().UpdateSprites();
                    card.GetComponent<CardController>().initBombAnimation = true;
                    Camera.main.GetComponent<ScreenShake>().TriggerShake();
                    Vibration.Vibrate(50);

                    //Salva card:
                    FindObjectOfType<PCSettings>().SaveCard(card.GetComponent<CardController>());
                }

                //Atualiza placar de possiveis merges:
                FindObjectOfType<PossibleToMerge>().updatePossibleToMerge();
            }
        }
    }


    public List<GameObject> GetCardsNoTabuleiro()
    {
        List<GameObject> retorno = new();

        for (int i = 0; i < board.transform.childCount; i++)
        {
            Transform currentLine = board.transform.GetChild(i);
            for (int j = 0; j < currentLine.childCount; j++)
            {
                GameObject currentCard = currentLine.GetChild(j).gameObject;
                if (currentCard.GetComponent<CardController>().statusCard == 3 && currentCard.GetComponent<CardController>().figura != figuraNull)
                {
                    retorno.Add(currentCard);
                }
            }
        }

        return retorno;

    }
}