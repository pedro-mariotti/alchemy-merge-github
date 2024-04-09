using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//x = 0.69
//y = -1.36

//final x = 0.42
//final y = -1.2
public class TutorialController : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] private GameObject initialCard;
    [SerializeField] private GameObject finalCard;
    [SerializeField] private GameObject dealButton;
    [SerializeField] private GameObject objOnboarding0;
    [SerializeField] private MergeController mergeController;
    [SerializeField] private Figure figuraNull;
    [SerializeField] private PossibleToMerge possibleToMergeScr;
    // private SpriteRenderer handSpr;
    [Header("Curvas de animação")]
    // [SerializeField] private AnimationCurve opacityCurve;
    [SerializeField] private AnimationCurve movementCurve;
    // [SerializeField] private AnimationCurve rotationCurve;

    private Vector3 finalPosition = new();
    private Vector3 initialPosition = new();
    private float animationCount = 1f;
    private float animationEnd = 100f;
    private float animationIndex;
    [SerializeField] private GameObject trashGO;
    bool hasSearched = false;
    List<GameObject> figurasNoTabuleiro = new();
    public int mergesToStartTrashTutorial = 0;
    
    void Start()
    {
        initialPosition = initialCard.transform.position;
        finalPosition = finalCard.transform.position;

        // handSpr = GetComponentInChildren<SpriteRenderer>();
        transform.position = initialPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (PCSettings.onboardingStage==0)
        {
            //Atualiza posição:
            initialPosition = initialCard.transform.position;
            finalPosition = finalCard.transform.position;

            //Soma (avançar na animação):
            animationCount++;

            //Atualiza valor para animação:
            animationIndex = animationCount / animationEnd;
            
            float sprPositionIndex = movementCurve.Evaluate(animationIndex);
                    
            transform.position = Vector3.Lerp(initialPosition, finalPosition, sprPositionIndex);

            //Último estágio da animação:
            if (animationCount == animationEnd)
            {
                //Reinicia:
                animationCount=0f;
            }
        }
        else
        {
            gameObject.SetActive(false);
            objOnboarding0.SetActive(false);
        }
        
        if (PCSettings.inMerge || initialCard.GetComponent<CardController>().figura == figuraNull || finalCard.GetComponent<CardController>().figura == figuraNull || PCSettings.onboardingStage>0)
        {
            if (PCSettings.onboardingStage==0)
            {
                PCSettings.onboardingStage = 1;
                FindObjectOfType<PCSettings>().SaveGame();
            }
                gameObject.SetActive(false);
                objOnboarding0.SetActive(false);
        }


    }

}
