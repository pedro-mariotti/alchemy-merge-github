using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WikiSwitchPosition : MonoBehaviour
{
    public bool IAmWiki;
    public bool IAmTabuleiro;
    private float xAtual = 0f;
    private float xInicial = 0f;

    void Start() {
        if (IAmWiki)
        {
            xAtual = 20f;
        }
        else
        {
            xAtual = 0f;
        }

        xInicial = transform.localPosition.x;
    }

    void Update()
    {
        float xFinal;

        if(PCSettings.inWiki != IAmWiki)
        {
            if (IAmWiki)
            {
                xFinal = 20f;
            }
            else
            {
                xFinal = -15f;
            }
        }
        else
        {
            xFinal = 0f;
        }

        xAtual+=(xFinal-xAtual)/10f;

        transform.localPosition = new Vector3(xInicial+xAtual,transform.localPosition.y,transform.localPosition.z);

        if (IAmTabuleiro)
        {
            if (PCSettings.inWiki)
            {
                if(!PCSettings.inWikiFinal)
                {
                    PCSettings.inWikiFinal = true;
                    Debug.Log("Começo da telaWiki");
                }
            }
            else
            {
                if(PCSettings.inWikiFinal)
                {
                    float diferenca = Mathf.Abs(xAtual - xFinal);
                    if(diferenca<.1f)
                    {
                        PCSettings.inWikiFinal = false;
                        Debug.Log("Fim da telaWiki");
                    }
                }
            }
        }
        
        
    }
}
