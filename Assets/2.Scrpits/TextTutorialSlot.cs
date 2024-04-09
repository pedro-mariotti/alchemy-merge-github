using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTutorialSlot : MonoBehaviour
{
    public bool ativo = false;

    // Update is called once per frame
    void Update()
    {

        if (ativo)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
            gameObject.GetComponent<HideOnOnboarding>().enabled = true;

            if (PlayerPrefs.HasKey("tutorialCompraSlot"))
            {
                if (PlayerPrefs.GetInt("tutorialCompraSlot")==1 && PCSettings.onboardingStage==4)
                {
                    ativo = false;
                    PCSettings.onboardingStage = 5;
                    GameObject.Find("PC").GetComponent<PCSettings>().SaveGame();
                    
                    gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    gameObject.GetComponent<HideOnOnboarding>().enabled = false;
                }
            }
        }
    }
}
