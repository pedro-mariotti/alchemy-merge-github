using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CardTap : MonoBehaviour
{

    [Header("Pai (Card):")]
    [SerializeField] private CardController card;
    SoundController soundController;


    private void Start()
    {
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
    }

    void Update()
    {
        //CheckTap:
        if ((Input.GetMouseButtonDown(0)) && (!PCSettings.inDragging) && !PCSettings.lockGame)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null && hit.collider == this.GetComponent<Collider2D>())
            {
                if (card.statusCard == 2 && FindObjectOfType<BankController>().GetBankValue() >= card.valor)
                {
                    //gasta a grana:
                    FindObjectOfType<BankController>().RemoveMoney(card.valor);
                    soundController.TriggerBuySound();

                    //Sou um querido que desbloqueia uma nova figura:
                    if (card.DesbFigura != null && card.DesbFigura != card.figuraNull)
                    {
                        //Inicia com essa figura aqui:
                        card.figura = card.DesbFigura;

                        //Liberar nova figura:
                        PCSettings PC = GameObject.Find("PC").GetComponent<PCSettings>();
                        if (!PC.figuresExtrasSlot.Contains(card.DesbFigura))
                        {
                            PC.figuresExtrasSlot.Add(card.DesbFigura);

                            //Atualiza listas de figuras no deal:
                            DealController dealController = FindObjectOfType<DealController>();
                            dealController.UpdateKnownMergesAndFiguresToDeal();
                        }
                        //Animação de popup de novo elemento:
                        GameObject.Find("PopUpNewElement").GetComponent<PopUpNewElementController>().ConfAnimation(card.DesbFigura.sprite, false, gameObject);
                        GameObject.Find("PopUpNewElement").GetComponent<PopUpNewElementController>().StartAnimation();
                    }

                    //Libera e atualiza arte:
                    card.statusCard = 3;
                    card.UpdateSprites();

                     //Salva tutorial compra de slot:
                    PlayerPrefs.SetInt("tutorialCompraSlot", 1);

                    //Atualiza placar de possiveis merges:
                    FindObjectOfType<PossibleToMerge>().updatePossibleToMerge();
                }
            }
        }
    }

}
