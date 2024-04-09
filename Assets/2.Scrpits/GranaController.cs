using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranaController : MonoBehaviour
{
    public TextController text;
    public GameObject grana;
    public BankController bank;

    private PCSettings PC;


    private void Start()
    {
        PC = GameObject.Find("PC").GetComponent<PCSettings>();
    }
    // Start is called before the first frame update
    void Update()
    {
        int dinheiro = FindObjectOfType<BankController>().GetBankValue();
        AtualizarValor(dinheiro);
    }

    // Update is called once per frame
    public void AtualizarValor(int valor)
    {
        text.ChangeText(PC.ShortenNumber(valor));
    }
}
