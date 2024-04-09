using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimacaoChamarAtencao : MonoBehaviour
{
    [Header("Animações:")]
    [SerializeField] private AnimationCurve ac_Scale;
    [SerializeField] private float scaleBase = 1f;

    //Animcação:
    private float animationScale_Count = 0f;
    private float animationScale_End = 45f;
    public bool ativo = false;
    
    public void StartAnimacao()
    {
        ativo = true;
        float delay = (PCSettings.LevelPlayer<=1) ? -5f : -75f*PCSettings.LevelPlayer;
        if (delay<-300f) {delay=-300f;}
        animationScale_Count = delay;
    }
    
    public void StopAnimacao()
    {
        ativo = false;
        animationScale_Count = animationScale_End-1f;
    }

    // Update is called once per frame
    void Update()
    {
        //Não anima com lockGame:
        if (PCSettings.lockGame) {return;}

        if (animationScale_Count<animationScale_End)
        {
            //Avançar na animação:
            animationScale_Count++;

            //Atualiza valor para animação:
            float animationScale_Index = (animationScale_Count / animationScale_End);
            float newScale = ac_Scale.Evaluate(animationScale_Index)*scaleBase;

            //Scale:
            transform.localScale = new Vector3(newScale, newScale, 1f);

            //Fim?
            if (animationScale_Count==animationScale_End)
            {
                if (ativo)
                {
                    animationScale_Count = 0f;
                }
            }
        }
    }
}
