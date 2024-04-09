using UnityEngine;

public class LevelUpButtonTap : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    public int increment = 2;//multiplicador do preço do dealCards
    private PCSettings PC;

    private void Start()
    {
        PC = GameObject.Find("PC").GetComponent<PCSettings>();
        boxCollider = GetComponent<BoxCollider2D>();

    }
    private void Update()
    {
        // Checa se o player tocou no botão
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);


            if (boxCollider == Physics2D.OverlapPoint(touchPos))
            {
                PCSettings.lockGame = false;
                PCSettings.unlockCards = true;
                Debug.Log("Level up button clicado");
                transform.parent.GetChild(4).gameObject.SetActive(false);
                gameObject.SetActive(false);
                Invoke("ResetUnlockCards", 2f); //atrasa a invocação desse método para resetar o unlockCards para dar tempo de todas as cartas desbloquearem

                //Libera esse card: 
                GameObject emptyCard = GetOneEmptyCard();
                int numberLast = PC.figuresExtrasLevel.Count;
                emptyCard.GetComponent<CardController>().figura = PC.figuresExtrasLevel[numberLast-1];
                emptyCard.GetComponent<CardController>().UpdateSprites(true);

                //Atualiza placar de possiveis merges:
                FindObjectOfType<PossibleToMerge>().updatePossibleToMerge();
            }
        }
    }
    private void ResetUnlockCards()
    {
        PCSettings.unlockCards = false;
    }


    private GameObject GetOneEmptyCard()
    {
        GameObject board = GameObject.Find("Tabuleiro");

        for (int i = 0; i < board.transform.childCount; i++)
        {
            Transform currentLine = board.transform.GetChild(i);
            for (int j = 0; j < currentLine.childCount; j++)
            {
                GameObject currentCard = currentLine.GetChild(j).gameObject;
                if (currentCard.GetComponent<CardController>()!=null)
                {
                    if (currentCard.GetComponent<CardController>().statusCard == 3 && currentCard.GetComponent<CardController>().figura == PC.figuraNull)
                    {
                        return currentCard;
                    }
                }
            }
        }

        return null;


    }
}
