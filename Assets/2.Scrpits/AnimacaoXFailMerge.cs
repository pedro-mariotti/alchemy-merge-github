using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimacaoXFailMerge : MonoBehaviour
{
    [Header("Animações:")]
    [SerializeField] private AnimationCurve ac_Alpha;

    [Header("SpriteRenderer:")]
    [SerializeField] private SpriteRenderer sprX;

    //Animcação:
    private Vector3 positionStart;
    private Vector3 positionEnd;
    private float animation_Count = 0f;
    private float animation_End = 100f;

    void Start()
    {
        positionStart = transform.position;
        positionEnd = positionStart + new Vector3(0f,.5f,0f);

        transform.localScale = GameObject.Find("GAME").transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (animation_Count < animation_End)
        {
            //Subtrai (avançar na animação):
            animation_Count++;

            //Atualiza valor para animação:
            float animation_Index = (animation_Count / animation_End);
            float animation_Alpha = ac_Alpha.Evaluate(animation_Index);

            //Posiciona:
            transform.position = Vector3.Lerp(positionStart, positionEnd, animation_Index);

            //Alpha:
            sprX.color  = new Color(1f,1f,1f,animation_Alpha);
            
            

            //Último estágio da animação:
            if (animation_Count == animation_End)
            {
                Destroy(gameObject);
            }
        }
    }
}
