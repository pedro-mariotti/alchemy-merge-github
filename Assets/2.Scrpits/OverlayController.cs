using UnityEngine;

public class OverlayController : MonoBehaviour
{
    private Vector2 viewport = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        updateBackground();
    }

    // Update is called once per frame
    void Update()
    {
        updateBackground();
    }


    void updateBackground()
    {

        float worldHeight = Camera.main.orthographicSize * 2.0f; //multiplica por 2 pq o ortographicSize pega a metade do valor total do tamnaho
        float worldWidth = worldHeight / Screen.height * Screen.width;
        Vector2 tempViewport = new Vector2(worldHeight, worldWidth);

        if (viewport != tempViewport) //Ocorreu mudança na câmera:
        {
            viewport = tempViewport; //Salva nova tela;

            SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();//GetComponentInChildren pegara um componente em <>
            Vector3 scaleTemp = GetComponentInChildren<Transform>().transform.localScale;

            scaleTemp.x = 1;
            scaleTemp.y = 1;
            transform.localScale = scaleTemp;

            float width = sprite.bounds.size.x;
            float height = sprite.bounds.size.y;
            float tempCameraHeight = Camera.main.orthographicSize * 2.0f; //multiplica por 2 pq o ortographicSize pega a metade do valor total do tamnaho
            float tempWorldWidth = tempCameraHeight / Screen.height * Screen.width;

            scaleTemp.x = tempWorldWidth / width;
            scaleTemp.y = tempCameraHeight / height;

            transform.localScale = scaleTemp + new Vector3(5f,5f,0f);
        }
    }
}
