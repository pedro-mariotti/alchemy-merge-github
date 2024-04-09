using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponsiveScale : MonoBehaviour
{
    [SerializeField] private bool update = false;
    [SerializeField] private SpriteRenderer spriteRendererReference;

    public float tolerance;
    public float defaultScale = 1;

    private Vector2 viewport = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        setScale();
    }

    // Update is called once per frame
    void Update()
    {
        setScale();
    }



    private void OnValidate()
    {
        if (update)
        {
            setScale();
            update = false;
        }
    }

    void setScale()
    {

        float worldHeight = Camera.main.orthographicSize * 2.0f; //multiplica por 2 pq o ortographicSize pega a metade do valor total do tamnaho
        float worldWidth = worldHeight / Screen.height * Screen.width;
        Vector2 tempViewport = new Vector2 ( worldHeight, worldWidth );

        if (viewport!=tempViewport || update) //Ocorreu mudança na câmera:
        {
            //Salva nova tela:
            viewport = tempViewport;
            //Debug.Log("Nova tela!");

            //Descarta se ainda não for pequeno de mais o mundo:
            float myWidth = (spriteRendererReference==null) ? tolerance : spriteRendererReference.bounds.size.x + tolerance;

            if (worldWidth<myWidth)
            {
                var newScaleU = worldWidth/myWidth;
                Vector3 newScale;
                if (newScaleU>defaultScale)
                {newScale = new Vector3 ( defaultScale, defaultScale , defaultScale );}
                else
                {newScale = new Vector3 ( newScaleU, newScaleU , newScaleU );}

                transform.localScale = newScale;

            }
            else
            {
                Vector3 newScale = new Vector3 ( defaultScale, defaultScale , defaultScale );
                transform.localScale = newScale;
            }

        }
    }
}
