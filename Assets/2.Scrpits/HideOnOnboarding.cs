using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnOnboarding : MonoBehaviour
{
    [SerializeField] private int StageShow;

    [SerializeField] private bool AutoNextStep = false;
    private int CountNextStep = 0;

    private Vector3 startPositon;


    [Header("Animações:")]
    [SerializeField] private AnimationCurve ac_Scale;
    [SerializeField] float animation_Count = 0f;
    private float animation_End = 20f;

    private float startScale;
    
    // Start is called before the first frame update
    void Awake()
    {
        startPositon = transform.localPosition;
        startScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (PCSettings.onboardingStage>StageShow && animation_Count<0f)
        {
            animation_Count=0f;
        }

        if (PCSettings.onboardingStage>=StageShow && !AutoNextStep && animation_Count == animation_End)
        {
            transform.localPosition = startPositon;
            this.enabled = false;
            return;
        }

        //Soma (avançar na animação):
        if (animation_Count < animation_End && PCSettings.onboardingStage>=StageShow)
        {
            animation_Count++;
        }

        //Atualiza valor para animação:
        float animation_Index = (animation_Count / animation_End);

        //Scale:
        float newScale = ac_Scale.Evaluate(animation_Index)*startScale;
        transform.localScale = new Vector3(newScale, newScale, 1f);


        if (PCSettings.onboardingStage!=StageShow)
        {
            transform.localPosition = new Vector3 (transform.localPosition.x,-10000f,0f);
        }
        else
        {

            transform.localPosition = startPositon;

            if (AutoNextStep)
            {
                CountNextStep++;
                if (CountNextStep>500 || (PCSettings.inMerge && CountNextStep>75))
                {
                    if (PCSettings.onboardingStage==StageShow)
                    {
                        PCSettings.onboardingStage++;
                        FindObjectOfType<PCSettings>().SaveGame();
                        gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
