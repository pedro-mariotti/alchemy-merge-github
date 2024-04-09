using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimacaoLixeira : MonoBehaviour
{

    [Header("Animações:")]
    [SerializeField] private AnimationCurve ac_Tampa;
    [SerializeField] private AnimationCurve ac_Scale;

    [Header("Tampa:")]
    [SerializeField] private GameObject objTampa;

    [Header("Status:")]
    public bool inAnimation = false;

    //Animcação:
    private float animation_Count = 0f;
    private float animation_End = 15f;

    // Update is called once per frame
    void Update()
    {
        
        if (inAnimation)
        {
            if (animation_Count < animation_End)
            {
                //Soma (avançar na animação):
                animation_Count++;

                //Atualiza valor para animação:
                float animation_Index = (animation_Count / animation_End);

                //Tampa:
                float novaRotacao = ac_Tampa.Evaluate(animation_Index);
                objTampa.transform.eulerAngles = new Vector3(0f,0f,-novaRotacao);

                //Scale:
                float newScale = ac_Scale.Evaluate(animation_Index);
                transform.localScale = new Vector3(newScale, newScale, 1f);
            }
        }
        else
        {
            if (animation_Count > 0f)
            {
                //Subtrai (Volta na animação):
                animation_Count--;

                //Atualiza valor para animação:
                float animation_Index = (animation_Count / animation_End);

                //Tampa:
                float novaRotacao = ac_Tampa.Evaluate(animation_Index);
                objTampa.transform.eulerAngles = new Vector3(0f,0f,-novaRotacao);

                //Scale:
                float newScale = ac_Scale.Evaluate(animation_Index);
                transform.localScale = new Vector3(newScale, newScale, 1f);
            }
        }
    }
}
