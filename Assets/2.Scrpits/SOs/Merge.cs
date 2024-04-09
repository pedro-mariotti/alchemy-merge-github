using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[CreateAssetMenu(fileName = "Novo Merge", menuName = "Merge")]
public class Merge : ScriptableObject
{
    public Figure a; //Somatorio 1
    public Figure b; //Somatorio 2
    public Figure resultado; //Resultado

    public bool descoberto = false; //Se o player já descobriu
    public int count = 0; //Número de vezes que foi feito esse merge
    public bool hint = false; //Se ele foi revelado por compra de dica.



    public MergeData ToMergeData()
    {
        if (a != null && b != null && resultado != null)
        {
            MergeData mergeData = new()
            {
                a = this.a.figura,
                b = this.b.figura,
                resultado = this.resultado.figura,
                descoberto = this.descoberto,
                hint = this.hint,
                count = this.count
            };

            return mergeData;
        }
        else
        {
            return new MergeData();
        }
    }

    public void FromMergeData(MergeData mergeData, List<Figure> ListAllFigures)
    {

        this.a = FindFigure(mergeData.a, ListAllFigures);
        this.b = FindFigure(mergeData.b, ListAllFigures);
        this.resultado = FindFigure(mergeData.resultado, ListAllFigures);
        this.descoberto = mergeData.descoberto;
        this.hint = mergeData.hint;
        this.count = mergeData.count;

    }

    public Figure FindFigure(tipoDeFigura FiguraPesquisa, List<Figure> ListAllFigures)
    {
        foreach (var figureAtual in ListAllFigures)
        {
            if (figureAtual.figura == FiguraPesquisa)
            { return figureAtual; }
        }
        return null;
    }

}


//Usaremos essa classe auxiliar apenas para pegar as informação do disco (salvar) e vice-versa:
[Serializable]
public class MergeData
{
    [SerializeField] private tipoDeFigura _a;
    [SerializeField] private tipoDeFigura _b;
    [SerializeField] private tipoDeFigura _resultado;
    [SerializeField] private bool _descoberto;
    [SerializeField] private bool _hint;
    [SerializeField] private int _count;

    public tipoDeFigura a { get => _a; set => _a = value; }
    public tipoDeFigura b { get => _b; set => _b = value; }
    public tipoDeFigura resultado { get => _resultado; set => _resultado = value; }
    public bool descoberto { get => _descoberto; set => _descoberto = value; }
    public bool hint { get => _hint; set => _hint = value; }
    public int count { get => _count; set => _count = value; }
}


[Serializable]
public class MergeDataArrayWrapper
{
    public MergeData[] mergeDataArrayWrapper;
}