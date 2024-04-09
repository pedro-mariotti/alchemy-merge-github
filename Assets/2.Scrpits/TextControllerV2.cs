using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class TextControllerV2 : MonoBehaviour
{

    [Header("Prefab base:")]
    public GameObject textInCanvaPrefab;

    [Header("Posição & Escala:")]
    public Vector3 offsetVector;
    public float scale = 1f;

    [Header("Informações auto completáveis:")]
    GameObject canvasObject;
    Canvas canvasComponent;
    TextMeshProUGUI textComponent;

    public GameObject textObject;

    [Header("Estilização da fonte")]
    public Color textColor, outlineColor;


    // Awake is called before the first frame update and Start
    void Awake()
    {

        //Criando o Gameobject do texto, coloca ele como filho do canvas:
        canvasObject = Instantiate(textInCanvaPrefab, transform.position + offsetVector, Quaternion.identity);
        canvasObject.transform.SetParent(gameObject.transform);
        canvasObject.transform.localScale = new Vector3(scale, scale, 1f);

        textObject = canvasObject.transform.Find("myText").gameObject;
        // salva o componente de texto para a edição no futuro
        textComponent = textObject.GetComponentInChildren<TextMeshProUGUI>();
        // Obtenha o componente Canvas do objeto
        canvasComponent = canvasObject.GetComponent<Canvas>();
        // Encontre a câmera principal da cena
        Camera mainCamera = Camera.main;
        // Atribua a câmera principal ao componente Canvas
        canvasComponent.worldCamera = mainCamera;
        textComponent.fontStyle = FontStyles.Bold;
        textComponent.color = textColor;
        textComponent.outlineColor = outlineColor;
        textComponent.outlineWidth = 0.2f;

    }

    public void ChangeText(string texto)
    {
        textComponent.text = texto;
    }
    public void ChangeLayer(string layer)
    {
        canvasComponent.sortingLayerName = layer;
    }

    public void ChangeColorText(Color cor)
    {
        textColor = cor;
        textComponent.color = textColor;
    }

    public void ChangeAlphaText(float alpha)
    {
        textColor = new Color(textColor.r,textColor.g,textColor.b,alpha);
        textComponent.color = textColor;
    }

    public void SetSortingOrder(int valor)
    {
        // Define o order in layer do Canvas
        canvasComponent.sortingOrder = valor;
    }
    public void SetSortingOrderExt(int valor, string layerName)
    {
        // Define o order in layer do Canvas
        canvasComponent.sortingOrder = valor;

        // Altera a camada do objeto
        canvasComponent.sortingLayerName = layerName;
    }

    public void changePos(float posX, float posY)
    {
        RectTransform rectTransform = textObject.GetComponent<RectTransform>();

        rectTransform.anchoredPosition = new Vector2(posX, posY);
    }
    public void ChangeTextSize(float novoTamanho)
    {
        textComponent.fontSize = novoTamanho;
    }

}
