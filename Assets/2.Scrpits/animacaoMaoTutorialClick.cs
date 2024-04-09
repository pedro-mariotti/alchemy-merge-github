using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animacaoMaoTutorialClick : MonoBehaviour
{
    [Header("Animações:")]
    [SerializeField] private AnimationCurve ac_Angle;

    [Header("Sou para compra de slot:")]
    [SerializeField] private bool isBuySlot;

    private float animation_Count = 0f;

    // Update is called once per frame
    void Update()
    {
        //Soma (avançar na animação):
        animation_Count+=.015f;

        if (animation_Count>1)
        {
            animation_Count-=1;

            if (!PlayerPrefs.HasKey("tutorialCompraSlot") && isBuySlot && PCSettings.onboardingStage==4 && GameObject.Find("objTextTutorialSlot").GetComponent<TextTutorialSlot>().ativo == false)
            {
                GameObject.Find("objTextTutorialSlot").GetComponent<TextTutorialSlot>().ativo = true;
                GameObject.Find("objTextTutorialSlot").transform.localScale = new Vector3(0f,0f,1f);
            }
        }
        
        //Angle:
        float animation_Angle = ac_Angle.Evaluate(animation_Count);
        transform.localRotation = Quaternion.Euler(0f, 0f, animation_Angle+10f);
    }
}
