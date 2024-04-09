using UnityEngine;
using UnityEditor;
public class CreateAMerge : MonoBehaviour
{
    [Header("Info figura")]
    public Sprite figureSprite;
    public string figureFileName, cardName;
    public bool isReward;

    [Header("Ingredientes do merge")]
    public Figure itemA;
    public Figure itemB;
    [Header(" ")]
    public bool create;
#if UNITY_EDITOR
    private void CreateMerge()
    {

        Figure figura = ScriptableObject.CreateInstance<Figure>();


        string fileFigurePath = "Assets/5.SO/Figuras/" + figureFileName + ".asset";
        string fileMergePath = "Assets/5.SO/Merges/" + figureFileName + ".asset";


        figura.sprite = figureSprite;
        figura.cardName = cardName;
        AssetDatabase.CreateAsset(figura, fileFigurePath);


        if (!isReward)
        {
            Merge merge = ScriptableObject.CreateInstance<Merge>();
            merge.a = itemA;
            merge.b = itemB;
            merge.resultado = figura;
            AssetDatabase.CreateAsset(merge, fileMergePath);
        }

        AssetDatabase.SaveAssets();

        Debug.Log("Figure created and saved at: " + fileFigurePath);
    }
    private void OnValidate()
    {
        if (create)
        {
            CreateMerge();
            create = false;
        }
    }
#endif
}