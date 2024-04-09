using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarraLevelUp : MonoBehaviour
{
    [Header("Figura Vazia:")]
    [SerializeField] private Figure figuraNull;

    [Header("PC:")]
    public PCSettings PC;

    [Header("Barra:")]
    public Transform barraP1;
    public Transform barraP2;

    [Header("Conf tamanho barra:")]
    public float MAX_WIDTH = 500f;

    [Header("Info Level:")]
    public SpriteRenderer TextLevel;
    public List<Sprite> sptTxtLevel;
    public SpriteRenderer sprRenFiguraLevel;
    public Figure FiguraLevel;

    private float TAMANHO_ATUAL = 0;

    public int LocalLevelPlayer = 1;


    // Update is called once per frame
    void Update()
    {
        TextLevel.sprite = sptTxtLevel[LocalLevelPlayer];
        
        FiguraLevel = PC.unlockFiguresLevelPlayer[LocalLevelPlayer - 1];
        sprRenFiguraLevel.sprite = FiguraLevel.sprite;

        float VALOR_CONT = 1f;
        float VALOR_END = 1f;

        // Debug.Log("LocalLevelPlayer: "+LocalLevelPlayer +" | LevelPlayer:"+PCSettings.LevelPlayer);

        if (LocalLevelPlayer == PCSettings.LevelPlayer)
        {
            //Calculo porcentagem da barra:
            VALOR_CONT = PCSettings.LevelPlayer_XP - PCSettings.LevelPlayer_XP_MIN;
            VALOR_END = PCSettings.LevelPlayer_XP_MAX - PCSettings.LevelPlayer_XP_MIN;
        }
        else
        {
            if (TAMANHO_ATUAL > MAX_WIDTH - 0.1f)
            {
                //LevelUp!

                //Debug:
                Debug.Log("LevelUp: " + LocalLevelPlayer);

                //A atualização da barra ocorre quando o popup de levelup aparecer. com UpdateBarraEmLevelUp().

                //Desbloqueia figura:
                FiguraLevel = PC.unlockFiguresLevelPlayer[PCSettings.LevelPlayer - 2];
                if (FiguraLevel != figuraNull && FiguraLevel != null) // so por segurança
                {
                    //Desbloqueia:
                    if (!PC.figuresExtrasLevel.Contains(FiguraLevel))
                    {
                        PC.figuresExtrasLevel.Add(FiguraLevel);

                        //Atualiza listas de figuras no deal:
                        DealController dealController = FindObjectOfType<DealController>();
                        dealController.UpdateKnownMergesAndFiguresToDeal();
                    }
                }

                //Animação de popup levelUp:
                GameObject.Find("PopUpLevelUp").GetComponent<PopUpLevelUp>().ConfAnimation(FiguraLevel.sprite);

            }
        }

        float NOVO_TAMANHO = ((float)VALOR_CONT / (float)VALOR_END) * MAX_WIDTH;
        if (NOVO_TAMANHO > MAX_WIDTH) { NOVO_TAMANHO = MAX_WIDTH; }
        TAMANHO_ATUAL = Mathf.Lerp(TAMANHO_ATUAL, NOVO_TAMANHO, .2f);
        //Segurança de código, anti NaN:
        if (float.IsNaN(TAMANHO_ATUAL)) { TAMANHO_ATUAL = 0f; }

        //Atualiza valores:
        barraP1.localScale = new Vector3(TAMANHO_ATUAL, 1f, 1f);
        barraP2.localPosition = new Vector2(barraP1.localPosition.x + (TAMANHO_ATUAL / 100f), barraP2.localPosition.y);
    }

    public void UpdateBarraEmLevelUp()
    {
        //Atualiza numero no escudo (e LocalLevelPlayer):
        LocalLevelPlayer = PCSettings.LevelPlayer;
        FindObjectOfType<PCSettings>().SaveGame();
    }
}
