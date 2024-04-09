using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimacaoFailMerge : MonoBehaviour
{

    [Header("Animações:")]
    [SerializeField] private AnimationCurve ac_Angle;

    [Header("Auto-play:")]
    [SerializeField] private bool autoplay = false;

    //Animcação:
    private float animation_Count = 300f;
    private float animation_End = 100f;
    private float animation_Index;

    // Update is called once per frame
    void Update()
    {
        if (animation_Count < animation_End)
        {
            //Subtrai (avançar na animação):
            animation_Count++;

            //Atualiza valor para animação:
            float animation_Index = (animation_Count / animation_End); if (autoplay) { animation_Index += .1f; }
            float animation_Angle = ac_Angle.Evaluate(animation_Index);

            //Angle:
            transform.localRotation = Quaternion.Euler(0f, 0f, animation_Angle);
        }
    }

    public void StartAnimation()
    {
        animation_Count = 0f;
    }

    void Start()
    {
        if (autoplay)
        {
            StartAnimation();
        }
    }
}
