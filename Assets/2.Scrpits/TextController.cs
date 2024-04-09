using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class TextController : MonoBehaviour
{
    public GameObject textPrefab;
    public GameObject canvasObject;
    public Vector3 offsetVector;
    public TextMeshProUGUI textComponent;
    public GameObject textObject;
    public Color textColor, outlineColor;
    public float textFontSize = 0.2f;

    // Awake is called before the first frame update and Start
    void Awake()
    {
        if (canvasObject == null)
        {
            canvasObject = GameObject.Find("Canvas");
        }
        //Criando o Gameobject do texto, coloca ele como filho do canvas:
        textObject = Instantiate(textPrefab, transform.position + offsetVector, Quaternion.identity);
        textObject.transform.SetParent(canvasObject.transform);
        // salva o componente de texto para a edição no futuro
        textComponent = textObject.GetComponentInChildren<TextMeshProUGUI>();

        textComponent.fontStyle = FontStyles.Bold;
        textComponent.outlineColor = outlineColor;
        textComponent.outlineWidth = 0.2f;
        textComponent.color = textColor;
        textComponent.fontSize = textFontSize;
    }

    // Update is called once per frame
    void Update()
    {
        textObject.transform.position = transform.position + (offsetVector*transform.lossyScale.x);
        textObject.transform.localScale = transform.lossyScale;

        textComponent.fontSize = textFontSize;
    }

    public void ChangeText(string texto)
    {
        textComponent.text = texto;
    }


    public void changePos(float posX, float posY)
    {
        RectTransform rectTransform = textObject.GetComponent<RectTransform>();

        rectTransform.pivot = new Vector2(posX, posY);
    }
}
