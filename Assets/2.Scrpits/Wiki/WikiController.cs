using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WikiController : MonoBehaviour
{
    BoxCollider2D boxCollider;
    public Sprite amarelo;
    public Sprite azul;
    public Sprite verde;
    private List<GameObject> cards = new List<GameObject>();
    private List<GameObject> cardspop = new List<GameObject>();
    private List<GameObject> azulCards = new List<GameObject>();
    private CardWikiController cardWikiController;
    public WikiPopupController popUp;

    SoundController soundController;


    // Start is called before the first frame update
    void Start()
    {
        cardWikiController = FindObjectOfType<CardWikiController>();
        popUp = FindObjectOfType<WikiPopupController>();
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Converte a posição do toque para o mundo
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

            // Verifica se algum Box Collider está sendo tocado
            Collider2D[] hitColliders = Physics2D.OverlapPointAll(touchPos);
            foreach (Collider2D hitCollider in hitColliders)
            {
                if (hitCollider != null && hitCollider.transform.IsChildOf(transform))
                {
                    // Obtém a tag do Box Collider tocado
                    string tag = hitCollider.tag;
                    Debug.Log(tag);

                    if (tag == "azul")
                    {
                        DestroyObjects(cards);
                        cards = null;
                        TrocarAba(1);
                        cards = cardWikiController.Instanciador(1);
                        PCSettings.inBlue = true;
                        PCSettings.inGreen = false;
                        PCSettings.inYellow = false;
                        popUp.MoveToStartPosition();
                        soundController.TriggerButtonSound2();
                    }
                    else if (tag == "verde")
                    {
                        DestroyObjects(cards);
                        cards = null;
                        TrocarAba(2);
                        cards = cardWikiController.Instanciador(2);
                        PCSettings.inBlue = false;
                        popUp.MoveToStartPosition();
                        soundController.TriggerButtonSound2();
                    }
                    else if (tag == "amarelo")
                    {
                        DestroyObjects(cards);
                        cards = null;
                        TrocarAba(3);
                        cards = cardWikiController.Instanciador(3);
                        PCSettings.inBlue = false;
                        popUp.MoveToStartPosition();
                        soundController.TriggerButtonSound2();
                    }
                    else if (tag == "fechar")
                    {
                        DestroyObjects(cardspop);
                        PCSettings.inWiki = false;
                        DestroyObjects(cards);
                        cards = null;
                        TrocarAba(1);
                        popUp.MoveToStartPosition();
                        soundController.TriggerButtonSound2();

                    }

                    // Saí do loop se encontrar um Box Collider tocado
                    break;
                }
            }
        }
    }

    public void iniciaAzul()
    {
        PCSettings.inBlue = true;
        DestroyObjects(cards);
        cards = null;
        TrocarAba(1);
        cards = cardWikiController.Instanciador(1);
    }
    public void TrocarAba(int cor)
    {
        PCSettings.WikiAba = cor;

        // Obter o componente SpriteRenderer do objeto filho
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Atribuir a nova imagem da sprite ao componente SpriteRenderer
        if (cor == 1) { spriteRenderer.sprite = azul; }
        if (cor == 2) { spriteRenderer.sprite = verde; }
        if (cor == 3) { spriteRenderer.sprite = amarelo; }
    }
    public void DestroyObjects(List<GameObject> cards)//metodo para apagar as informações do wiki quando troca de  aba ou quandop fecha
    {
        if (cards == null)
        {
            // Debug.LogWarning("Lista de objetos é nula");
            return;
        }

        if (cards.Count == 0)
        {
            // Debug.LogWarning("Lista de objetos está vazia");
            return;
        }

        foreach (GameObject card in cards)
        {
            // Debug.Log("Objeto destruído: " + card.name);
            Destroy(card);
        }
        cards.Clear();

    }
    public void PopInstance(GameObject card)
    {
        if (PCSettings.inBlue == false || true)
        {
            soundController.TriggerButtonSound();
            DestroyObjects(cardspop);
            cardWikiController.InstancePop(card);
        }
    }
}
