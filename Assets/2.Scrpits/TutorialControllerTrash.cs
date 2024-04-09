using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialControllerTrash : MonoBehaviour
{

    [Header("GameObjects")]
    [SerializeField] private GameObject objTrash;
    [SerializeField] private GameObject board;
    [SerializeField] private GameObject cardDefault;
    [SerializeField] private GameObject objOnboarding4;

    [Header("Curvas de animação")]
    [SerializeField] private AnimationCurve movementCurve;

    [Header("Animacao Chamar Atencao Trash")]
    [SerializeField] private AnimacaoChamarAtencao atencaoTrash;

    [Header("Figura de fogo")]
    [SerializeField] private Figure figFogo;


    //Animcação:
    private Vector3 positionStart;
    private Vector3 positionEnd;
    private float animation_Count = 0f;
    private float animation_End = 100f;

    // Start is called before the first frame update
    void Start()
    {
        //Some com esse troço da tela:
        transform.position = new Vector3(10000f,10000f,1f);
    }

    // Update is called once per frame
    void Update()
    {
        bool lixeiraAtiva = atencaoTrash.ativo;
        if (!PCSettings.tutorialTrashFinish && lixeiraAtiva)
        {
            //Texto auxiliar no tutorial: 
            objOnboarding4.SetActive(true);

            //Soma (avançar na animação):
            animation_Count++;

            //Atualiza valor para animação:
            float animation_Index = (animation_Count / animation_End);
            float animation_Lerp = movementCurve.Evaluate(animation_Index);

            //Dados de posicao inicial e final:
            Vector3 positionStart = CardDeDescarte().transform.position;
            Vector3 positionEnd = objTrash.transform.position;

            //Posiciona:
            transform.position = Vector3.Lerp(positionStart, positionEnd, animation_Lerp);

            //Último estágio da animação:
            if (animation_Count == animation_End)
            {
                //Reinicia:
                animation_Count=0f;
            }
        }
        else
        {
            //Some com esse troço da tela:
            transform.position = new Vector3(10000f,10000f,1f);
            //Texto auxiliar no tutorial: 
            objOnboarding4.SetActive(false);
        }
    }

    private GameObject CardDeDescarte()
    {
        for (int i = 0; i < board.transform.childCount; i++)
        {
            Transform currentLine = board.transform.GetChild(i);
            for (int j = 0; j < currentLine.childCount; j++)
            {
                GameObject currentCard = currentLine.GetChild(j).gameObject;
                if (currentCard.GetComponent<CardController>() != null)
                {
                    if (currentCard.GetComponent<CardController>().statusCard == 3 && currentCard.GetComponent<CardController>().figura == figFogo)
                    {
                        return currentCard;
                    }
                }
            }
        }

        return cardDefault;
    }
}
