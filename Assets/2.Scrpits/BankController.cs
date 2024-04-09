using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BankController : MonoBehaviour
{
    //private static int totalBank;
    
    [SerializeField]
    public int totalBank;
    public GranaController granaController;
   
    void Awake()
    {
        //Carrega grana salva:
        if (PlayerPrefs.HasKey("totalBank"))
        {
            #if !UNITY_EDITOR
            totalBank = PlayerPrefs.GetInt("totalBank");
            #endif
        }

    }

    public void StoreMoney(int moneyAmt)
    {
        totalBank += moneyAmt;

        //Atualizando valor na tela:
        FindObjectOfType<GranaController>().AtualizarValor(totalBank); 

        //Salva:
        PlayerPrefs.SetInt("totalBank", totalBank);
    }
    public bool RemoveMoney(int moneyAmt)
    {
        if (moneyAmt >= totalBank)
        {
            return false;
        }
        totalBank -= moneyAmt;

        //Atualizando valor na tela:
        FindObjectOfType<GranaController>().AtualizarValor(totalBank);

        //Salva:
        PlayerPrefs.SetInt("totalBank", totalBank);

        return true;
    }
    public int GetBankValue()
    {
        return totalBank;
    }

}