using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpLevelUp : MonoBehaviour
{

    [Header("Animações:")]
    [SerializeField] private AnimationCurve ac_Alpha;
    // [SerializeField] private AnimationCurve ac_AlphaEnd;
    [SerializeField] private AnimationCurve ac_Scale;

    [Header("SpriteRenderer:")]
    [SerializeField] private SpriteRenderer sprSOMBRA;
    [SerializeField] private SpriteRenderer sprBASE;
    [SerializeField] private SpriteRenderer sprELEMENTO;
    [SerializeField] private SpriteRenderer sprCONFETE1;
    [SerializeField] private SpriteRenderer sprCONFETE2;
    [SerializeField] private SpriteRenderer sprBg;
    [SerializeField] private SpriteRenderer sprButton;

    [Header("Popup secundário:")]
    [SerializeField] private GameObject PopUpNewElementLevelUp;

    [Header("Barra de LevelUp:")]
    [SerializeField] private BarraLevelUp barraLevelUp;
    SoundController soundController;



    //Animcação:
    private float animation_Count = -300f;
    private float animation_End = 150f;

    //na etapa de pre level up:
    private bool inPreLevelUp = false;

    //Desbloqueia novo elemento:
    private Sprite sprForNewElement;
    // bool animationStarted = false, animationEnded = false;

    private void Start()
    {
        sprSOMBRA.color = new Color(1f, 1f, 1f, 0f);
        sprBASE.color = new Color(1f, 1f, 1f, 0f);
        sprELEMENTO.color = new Color(1f, 1f, 1f, 0f);
        sprCONFETE1.color = new Color(1f, 1f, 1f, 0f);
        sprCONFETE2.color = new Color(1f, 1f, 1f, 0f);
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();

    }

    // Update is called once per frame
    void Update()
    {

        if (animation_Count >= 0f && PCSettings.lockGame && !inPreLevelUp)
        {

            animation_Count++;

            float animation_Index;

            //Alpha:
            animation_End = 30f;
            animation_Index = (animation_Count / animation_End);
            float animation_Alpha = ac_Alpha.Evaluate(animation_Index);

            sprSOMBRA.color = new Color(1f, 1f, 1f, animation_Alpha);
            sprBASE.color = new Color(1f, 1f, 1f, animation_Alpha);
            sprELEMENTO.color = new Color(1f, 1f, 1f, animation_Alpha);
            sprCONFETE1.color = new Color(1f, 1f, 1f, animation_Alpha);
            sprCONFETE2.color = new Color(1f, 1f, 1f, animation_Alpha);
            sprBg.color = new Color(0f, 0f, 0f, animation_Alpha - 0.5f);
            sprButton.color = new Color(1f, 1f, 1f, animation_Alpha);

            //Scale:
            animation_End = 150f;
            animation_Index = (animation_Count / animation_End);
            float animation_Scale;

            animation_Scale = (ac_Scale.Evaluate(animation_Index + .3f) / 2f) + .5f;
            sprBASE.transform.localScale = new Vector3(animation_Scale, animation_Scale, 1f);

            animation_Scale = ((ac_Scale.Evaluate(animation_Index + .2f) / 2f) + .5f);
            sprELEMENTO.transform.localScale = new Vector3(animation_Scale, animation_Scale, 1f);

            animation_Scale = ac_Scale.Evaluate(animation_Index + .05f);
            sprCONFETE1.transform.localScale = new Vector3(animation_Scale + .05f, animation_Scale + .05f, 1f);

            animation_Scale = ac_Scale.Evaluate(animation_Index - .1f);
            sprCONFETE2.transform.localScale = new Vector3(animation_Scale + .05f, animation_Scale + .05f, 1f);

        }
        else
        {
            sprSOMBRA.color = new Color(1f, 1f, 1f, 0f);
            sprBASE.color = new Color(1f, 1f, 1f, 0f);
            sprELEMENTO.color = new Color(1f, 1f, 1f, 0f);
            sprCONFETE1.color = new Color(1f, 1f, 1f, 0f);
            sprCONFETE2.color = new Color(1f, 1f, 1f, 0f);
            sprBg.gameObject.SetActive(false);
            sprButton.gameObject.transform.parent.gameObject.SetActive(false);
        }
    }

    public void ConfAnimation(Sprite spr)
    {
        //trava a gameplay:
        PCSettings.lockGame = true;

        inPreLevelUp = true;

        sprForNewElement = spr;

        StartAnimation();
    }
    public void StartAnimation()
    {

        bool LiberadoAposPopUpNewElement = !(GameObject.Find("PopUpNewElement").GetComponent<PopUpNewElementController>().InAnimation());

        if (LiberadoAposPopUpNewElement && PCSettings.inAnimationMerge == false)
        {
            //Libera o level up na barra:
            barraLevelUp.UpdateBarraEmLevelUp();

            if (transform.childCount > 4)
            {
                inPreLevelUp = false;
                PCSettings.lockGame = true;
                // animationStarted = true;
                sprBg.gameObject.SetActive(true);
                sprButton.gameObject.transform.parent.gameObject.SetActive(true);
                soundController.TriggerLevelUpSound();
            }

            animation_Count = 0f;

            //Animação de popup de novo elemento:
            PopUpNewElementLevelUp.GetComponent<PopUpNewElementController>().ConfAnimation(sprForNewElement, false, null);
            PopUpNewElementLevelUp.GetComponent<PopUpNewElementController>().StartAnimation();
        }
        else
        {
            Invoke("StartAnimation", .1f);
        }
    }


    public bool InAnimation()
    {
        if (animation_Count < animation_End || (FindObjectOfType<BarraLevelUp>().LocalLevelPlayer < PCSettings.LevelPlayer))
        {
            return true;
        }

        return false;
    }
}
