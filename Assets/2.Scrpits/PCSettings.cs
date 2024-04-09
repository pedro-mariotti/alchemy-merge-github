using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


public class PCSettings : MonoBehaviour
{
    public static bool lockGame; //bloqueia o jogo enquanto estiver no overlay de level up
    public static bool unlockCards = false; //variavel que permite o desbloqueio dos cards novos apos o overlay fechar
    [Header("Atualizar listas:")]
    [SerializeField] private bool update = false;

    [Header("Figura Vazia:")]
    [SerializeField] public Figure figuraNull;

    [Header("Merges:")]
    public List<Merge> allMerges;

    [Header("Figures:")]
    public List<Figure> allFigures;
    public List<Figure> figuresExtrasLevel;
    public List<Figure> figuresExtrasSlot;

    [Header("Level Player:")]
    public List<float> MergesToLevelUpPlayer;
    public List<Figure> unlockFiguresLevelPlayer; //Organizar em ordem
    public static int LevelPlayer = 1;
    public static int DiscoveredMerges = 0;
    public static float LevelPlayer_XP = 0;
    public static float LevelPlayer_XP_MAX = 0;
    public static float LevelPlayer_XP_MIN = 0;


    //To drag e merge:
    public static bool inDragging;
    public static bool inMerge;
    public static GameObject cardToMerge;
    public static GameObject cardInDrag;

    public static int onboardingStage = 0;
    public static bool tutorialTrashFinish = false;
    public static bool tutorialHintFinish = false;

    public static bool inAnimationMerge; //Aqui informamos quando já acabou todo o processo de animação de um merge.

    //To wiki page
    public static bool inWiki;
    public static bool inWikiFinal;
    public static bool inScrolling;
    public static bool inGreen;
    public static bool inYellow;
    public static bool inBlue;
    public static int WikiAba = 1;

    //PowerUps:
    public static float ChanceToSpawnHigherTierElement = 0;

    //Dicas;
    public static int hints = 5;


    [SerializeField] GameObject board;


    public string ShortenNumber(int number) //metodo coletivo pra encurtar os numeros e adicionar o sufixo respectivo
    {
        char amountAlgarism = ' ';
        float displayPrice = number;
        int currentSuffix = 0;
        // Debug.Log("number = " + displayPrice);

        while (displayPrice >= 1000)
        {
            displayPrice /= 1000;
            currentSuffix++;
        }
        switch (currentSuffix)
        {
            case 1:
                amountAlgarism = 'K';
                break;
            case 2:
                amountAlgarism = 'M';
                break;
            case 3:
                amountAlgarism = 'B';
                break;
            case 4:
                amountAlgarism = 'T';
                break;
            case 5:
                amountAlgarism = 'Q';
                break;
            default:
                if (currentSuffix > 5)
                {
                    amountAlgarism = (char)(amountAlgarism + 1);
                }
                break;
        }

        // Ajuste para garantir três algarismos antes do sufixo
        string formattedPrice = displayPrice.ToString("F2");
        if (displayPrice >= 100)
        {
            formattedPrice = displayPrice.ToString("F0");
        }
        else if (displayPrice >= 10)
        {
            formattedPrice = displayPrice.ToString("F1");
        }

        // Remover o ".0" ou ".00" do final, se existir
        if (formattedPrice.EndsWith(".0") || formattedPrice.EndsWith(",0"))
        {
            formattedPrice = formattedPrice.Substring(0, formattedPrice.Length - 2);
        }
        else if (formattedPrice.EndsWith(".00") || formattedPrice.EndsWith(",00"))
        {
            formattedPrice = formattedPrice.Substring(0, formattedPrice.Length - 3);
        }
        else if ((formattedPrice.EndsWith("0") || formattedPrice.EndsWith("0")) && (formattedPrice.Contains(".") || formattedPrice.Contains(",")))
        {
            formattedPrice = formattedPrice.Substring(0, formattedPrice.Length - 1);
        }

        if (amountAlgarism == ' ')
        { return formattedPrice; }
        else
        { return formattedPrice + amountAlgarism; }

    }
    void Awake()
    {
        //FrameRate:
        Application.targetFrameRate = 60;


        //TESTES NÃO CARREGAR PROGRESSO//
        #if !UNITY_EDITOR
            LoadMergeData();
            LoadGame();
        #endif

        #if UNITY_EDITOR
            update = true;
            OnValidate();
        #endif

    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (update)
        {
            PlayerPrefs.DeleteKey("tutorialCompraSlot");

            //Nomear cards com identificadores únicos para save/load:
            for (int i = 0; i < board.transform.childCount; i++)
            {
                Transform currentLine = board.transform.GetChild(i);
                for (int j = 0; j < currentLine.childCount; j++)
                {
                    GameObject currentCard = currentLine.GetChild(j).gameObject;
                    CardController currentCardController = currentCard.GetComponent<CardController>();

                    if (currentCardController!=null)
                    {
                        if (currentCardController.cardID != "l"+(i+1)+"_c"+j)
                        {
                            Debug.LogError("cardID Errado!! "+ "l"+(i+1)+"_c"+j);
                        }
                    }

                }
            }

            allMerges.Clear();
            findAllMerges();

            allFigures.Clear();
            findAllFigures();

            // SaveMergeData();
            string filePath = Application.persistentDataPath + "/merge_data.dat";
            if (File.Exists(filePath))
            {
                File.Delete(filePath); // This line deletes the file if it exists
            }

            Debug.Log("Update merges!");
            update = false;
        }
    }

    //Usamos essa função para iniciar um jogo novo:
    private void findAllMerges()
    {
        // Encontra todos os assets do tipo ScriptableObject com o nome especificado
        string[] guids = AssetDatabase.FindAssets("t:Merge");

        foreach (string guid in guids)
        {
            // Carrega o asset do guid atual e o converte para um objeto ScriptableObject
            Object asset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Object));
            Merge merge = asset as Merge;

            // Adiciona a figura à lista de figuras
            if (merge != null)
            {
                merge.descoberto = false;
                merge.hint = false;
                merge.count = 0;
                allMerges.Add(merge);
            }
        }
    }

    private void findAllFigures()
    {
        // Encontra todos os assets do tipo ScriptableObject com o nome especificado
        string[] guids = AssetDatabase.FindAssets("t:Figure");

        foreach (string guid in guids)
        {
            // Carrega o asset do guid atual e o converte para um objeto ScriptableObject
            Object asset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Object));
            Figure figure = asset as Figure;

            // Adiciona a figura à lista de figuras
            if (figure != null)
            {
                allFigures.Add(figure);
            }
        }
    }
#endif



    //Usamos essa função para iniciar um jogo novo:
    public void SaveMergeData()
    {

        var mergeDataArray = new MergeData[allMerges.Count];
        for (int i = 0; i < allMerges.Count; i++)
        {
            if (allMerges[i] != null)
            {
                mergeDataArray[i] = allMerges[i].ToMergeData();
            }

        }

        MergeDataArrayWrapper wrapper = new MergeDataArrayWrapper { mergeDataArrayWrapper = mergeDataArray };

        string jsonData = JsonUtility.ToJson(wrapper);

        string filePath = Application.persistentDataPath + "/merge_data.dat";
        File.WriteAllText(filePath, jsonData);



        //Debug.Log("jsonData save: " + jsonData);
    }

    public void LoadMergeData()
    {
        //Se já existe, um arquivo de save, carrega:
        string filePath = Application.persistentDataPath + "/merge_data.dat";
        if (File.Exists(filePath))
        {
            Debug.Log("Load Merge Data File Exists!");

            bool debug = false;

            if (debug)
            {
                Debug.Log("Load Merge Data File Limpo!");
                File.Delete(filePath);
                return;
            }

            

            string jsonData = File.ReadAllText(filePath);

            MergeDataArrayWrapper deserializedWrapper = JsonUtility.FromJson<MergeDataArrayWrapper>(jsonData);
            MergeData[] mergeDataArray = deserializedWrapper.mergeDataArrayWrapper;

            Debug.Log("mergeDataArray.Length: " + mergeDataArray.Length);

            allMerges.Clear();
            for (int i = 0; i < mergeDataArray.Length; i++)
            {
                var merge = ScriptableObject.CreateInstance<Merge>();
                merge.FromMergeData(mergeDataArray[i], allFigures);
                allMerges.Add(merge);
            }

        }
        else
        {
            //Debug.Log("No saved merge data found");
        }
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("BarraLevelUp_LocalLevelPlayer", FindObjectOfType<BarraLevelUp>().LocalLevelPlayer);

        PlayerPrefs.SetInt("LevelPlayer", LevelPlayer);
        PlayerPrefs.SetInt("DiscoveredMerges", DiscoveredMerges);
        PlayerPrefs.SetFloat("LevelPlayer_XP", LevelPlayer_XP);
        PlayerPrefs.SetFloat("LevelPlayer_XP_MIN", LevelPlayer_XP_MIN);
        PlayerPrefs.SetFloat("LevelPlayer_XP_MAX", LevelPlayer_XP_MAX);
        
        PlayerPrefs.SetInt("onboardingStage", onboardingStage);
        PlayerPrefs.SetInt("tutorialTrashFinish", tutorialTrashFinish ? 1 : 0);
        PlayerPrefs.SetInt("tutorialHintFinish", tutorialHintFinish ? 1 : 0);

    }
    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("BarraLevelUp_LocalLevelPlayer")) {FindObjectOfType<BarraLevelUp>().LocalLevelPlayer = PlayerPrefs.GetInt("BarraLevelUp_LocalLevelPlayer");}

        if (PlayerPrefs.HasKey("LevelPlayer"))         {LevelPlayer = PlayerPrefs.GetInt("LevelPlayer");}
        if (PlayerPrefs.HasKey("DiscoveredMerges"))    {DiscoveredMerges = PlayerPrefs.GetInt("DiscoveredMerges");}
        if (PlayerPrefs.HasKey("LevelPlayer_XP"))      {LevelPlayer_XP = PlayerPrefs.GetFloat("LevelPlayer_XP");}
        if (PlayerPrefs.HasKey("LevelPlayer_XP_MIN"))  {LevelPlayer_XP_MIN = PlayerPrefs.GetFloat("LevelPlayer_XP_MIN");}
        if (PlayerPrefs.HasKey("LevelPlayer_XP_MAX"))  {LevelPlayer_XP_MAX = PlayerPrefs.GetFloat("LevelPlayer_XP_MAX");}

        if (PlayerPrefs.HasKey("onboardingStage"))     {onboardingStage = PlayerPrefs.GetInt("onboardingStage");}
        if (PlayerPrefs.HasKey("tutorialTrashFinish")) {tutorialTrashFinish = PlayerPrefs.GetInt("tutorialTrashFinish") == 1;}
        if (PlayerPrefs.HasKey("tutorialHintFinish"))  {tutorialHintFinish = PlayerPrefs.GetInt("tutorialHintFinish") == 1;}

        /* //Debug load:
            Debug.LogWarning("BarraLevelUp_LocalLevelPlayer: " + FindObjectOfType<BarraLevelUp>().LocalLevelPlayer);
            Debug.LogWarning("LevelPlayer: " + LevelPlayer);
            Debug.LogWarning("DiscoveredMerges: " + DiscoveredMerges);
            Debug.LogWarning("LevelPlayer_XP: " + LevelPlayer_XP);
            Debug.LogWarning("LevelPlayer_XP_MIN: " + LevelPlayer_XP_MIN);
            Debug.LogWarning("LevelPlayer_XP_MAX: " + LevelPlayer_XP_MAX);
            Debug.LogWarning("onboardingStage: " + onboardingStage);
            Debug.LogWarning("tutorialTrashFinish: " + tutorialTrashFinish);
        */
    }

    public void SaveCard(CardController card)
    {
        PlayerPrefs.SetInt(card.cardID+"__figura", card.figura.FigureInteiro());
        PlayerPrefs.SetInt(card.cardID+"__statusCard", card.statusCard);
    }

    public void LoadCard(CardController card)
    {
        #if !UNITY_EDITOR
            if (card.cardID == "")
            {
                Debug.LogError("SEM cardID!!");
                return;
            }

            if (PlayerPrefs.HasKey(card.cardID+"__figura"))
            {
                card.figura = FindFigureByInt( PlayerPrefs.GetInt(card.cardID+"__figura") , allFigures);
            }

            if (PlayerPrefs.HasKey(card.cardID+"__statusCard"))
            {
                card.statusCard = PlayerPrefs.GetInt(card.cardID+"__statusCard");

                Debug.LogWarning(card.cardID+" || Figura: "+card.figura+"  | Status: "+card.statusCard);
            }
        #endif

    }

    public void addXP(float valor)
    {
        LevelPlayer_XP += valor;

        //Atualiza level atual:
        LevelPlayer = 0;
        for (int i = 0; i < MergesToLevelUpPlayer.Count; i++)
        {
            if ((LevelPlayer_XP >= MergesToLevelUpPlayer[i]) && (LevelPlayer_XP < MergesToLevelUpPlayer[i + 1]))
            {
                LevelPlayer = i + 1;

                LevelPlayer_XP_MIN = MergesToLevelUpPlayer[i];
                LevelPlayer_XP_MAX = MergesToLevelUpPlayer[i + 1];
            }
        }

        SaveGame();
    }

    public Figure FindFigureByInt(int FiguraPesquisa, List<Figure> ListAllFigures)
    {
        foreach (var figureAtual in ListAllFigures)
        {
            if (figureAtual.FigureInteiro() == FiguraPesquisa)
            { return figureAtual; }
        }
        return null;
    }

    public void UpdateDiscoveredMerges()
    {
        /*
        DiscoveredMerges = 0;

        foreach (var Merge in allMerges)
        {
            if ((Merge.descoberto) && (Merge.resultado != figuraNull))
            {
                DiscoveredMerges++;
            }
        }

        //Debug.Log("DiscoveredMerges: "+DiscoveredMerges);

        LevelPlayer = 0;
        LevelPlayer_XP = DiscoveredMerges;

        for (int i = 0; i < MergesToLevelUpPlayer.Count; i++)
        {
            if ((LevelPlayer_XP >= MergesToLevelUpPlayer[i]) && (LevelPlayer_XP < MergesToLevelUpPlayer[i + 1]))
            {
                LevelPlayer = i + 1;

                LevelPlayer_XP_MIN = MergesToLevelUpPlayer[i];
                LevelPlayer_XP_MAX = MergesToLevelUpPlayer[i + 1];
            }
        }
        */
    }

}




