using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class VerifyFiles : MonoBehaviour
{
    public bool verify, verifyFiguraQuality;
    public Merge mergeGuia;
    public PCSettings PC;


#if UNITY_EDITOR
    private void Start()
    {
        Verify();
    }

    public void Verify()
    {
        string mergePath = AssetDatabase.GetAssetPath(mergeGuia);
        string folderPath = Path.GetDirectoryName(mergePath);
        // Debug.Log(folderPath[0]);

        string[] guidsArr = AssetDatabase.FindAssets("t:ScriptableObject", new[] { folderPath });


        foreach (string guid in guidsArr)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Merge merge = (Merge)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Merge));
            // Debug.Log(merge);

            if (merge.count != 0)
            {
                merge.count = 0;
                Debug.Log("Merge " + merge.name + " teve 'count' resetado");
            }
            if (merge.descoberto != false)
            {
                merge.descoberto = false;
                Debug.Log("Merge " + merge.name + " teve 'descoberto' resetado");
            }
            if (merge.hint != false)
            {
                merge.hint = false;
                Debug.Log("Merge " + merge.name + " teve 'hint' resetado");
            }
            if (merge.a == null || merge.b == null || merge.resultado == null)
            {
                Debug.Log("Merge " + merge.name + " esta faltando ingredientes");
            }
        }
    }

    void VerifyFiguraQuality()
    {
        int recipesAmt = 0;
        int timesRepeated = 0;
        foreach (Figure figura in PC.allFigures)
        {
            foreach (Merge merge in PC.allMerges)
            {
                if (merge.a == figura || merge.b == figura)
                {
                    recipesAmt++;
                }
                if (merge.resultado == figura)
                {
                    timesRepeated++;
                }
            }
            if (recipesAmt != 0)
            {
                if (recipesAmt == 1)
                {
                    Debug.LogWarning("Figura " + figura.name + " tem " + recipesAmt + " merge possivel");
                }
                else
                {
                    Debug.Log("Figura " + figura.name + " tem " + recipesAmt + " merges possiveis");
                }
                if (timesRepeated > 1)
                {
                    Debug.LogError("Figura " + figura.name + " tem " + timesRepeated + " repeticoes.");
                }
            }
            timesRepeated = 0;
            recipesAmt = 0;
        }
    }
    private void OnValidate()
    {
        if (verify)
        {
            Verify();
            verify = false;
        }
        else if (verifyFiguraQuality)
        {
            VerifyFiguraQuality();
            verifyFiguraQuality = false;
        }
    }
#endif
}
