using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHint : MonoBehaviour
{
    private bool jaUsouDica = false;
    private Vector3 startPositon;
    private float startScale;

    [Header("Animações:")]
    [SerializeField] private AnimationCurve ac_Scale;
    [SerializeField] float animation_Count = 0f;
    private float animation_End = 20f;

    public HintsController hintsController;

    // Start is called before the first frame update
    void Awake()
    {
        startPositon = transform.localPosition;
        startScale = transform.localScale.x;

        //Some:
        transform.localPosition = new Vector3 (transform.localPosition.x,-10000f,0f);
        transform.localScale = new Vector3(0f, 0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        //Exibir:
        if (hintsController.mergeToHint!=null && animation_Count<animation_End && PCSettings.LevelPlayer>1 && PCSettings.tutorialTrashFinish && !PCSettings.tutorialHintFinish)
        {
            animation_Count++;

            //Atualiza valor para animação:
            float animation_Index = (animation_Count / animation_End);

            //Scale:
            float newScale = ac_Scale.Evaluate(animation_Index)*startScale;
            transform.localScale = new Vector3(newScale, newScale, 1f);

            //Posição:
            transform.localPosition = startPositon;
        }
        
        //Sumir:
        if ((hintsController.mergeToHint==null || PCSettings.tutorialHintFinish) && animation_Count>0f)
        {
            animation_Count--;

            //Atualiza valor para animação:
            float animation_Index = (animation_Count / animation_End);

            //Scale:
            float newScale = ac_Scale.Evaluate(animation_Index)*startScale;
            transform.localScale = new Vector3(newScale, newScale, 1f);

            //Posição:
            if (animation_Count==0f)
            {
                transform.localPosition = new Vector3 (transform.localPosition.x,-10000f,0f);
            }
        }

        //Destroy:
        if (PCSettings.tutorialHintFinish && animation_Count==0f)
        {
            Destroy(gameObject);
        }
    }
}