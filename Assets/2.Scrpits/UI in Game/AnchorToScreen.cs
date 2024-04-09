using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnchoToScreenType
{
    center,
    topLeft,
    topCenter,
    topRight,
    leftCenter,
    rightCenter,
    bottomLeft,
    bottomCenter,
    bottomRight
}

public class AnchorToScreen : MonoBehaviour
{
    [SerializeField] public bool update = false;

    [SerializeField] private AnchoToScreenType anchorType;

    public float horizontalDisplacement;
    public float verticalDisplacement;

    private Vector2 viewport = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        setPosition();
    }

    // Update is called once per frame
    void Update()
    {
        setPosition();
    }



    private void OnValidate()
    {
        if (update)
        {
            setPosition();
            update = false;
        }
    }

    public void setPosition()
    {

        float worldHeight = Camera.main.orthographicSize * 2.0f; //multiplica por 2 pq o ortographicSize pega a metade do valor total do tamnaho
        float worldWidth = worldHeight / Screen.height * Screen.width;
        Vector2 tempViewport = new Vector2 ( worldHeight, worldWidth );

        if (viewport!=tempViewport || update || true) //Ocorreu mudança na câmera:
        {
            //Salva nova tela:
            viewport = tempViewport;
            //Debug.Log("Nova tela!");

            //SafeArea:
            float bottomUnits = 0 , topUnits = 0, leftUnits = 0, rightUnits = 0;
            if (Application.isPlaying)
            {
                float bottomPixels = Screen.safeArea.y;
                float topPixels = Screen.height - (Screen.safeArea.y + Screen.safeArea.height);
                float leftPixels = Screen.safeArea.x;
                float rightPixels = Screen.width - (Screen.safeArea.x + Screen.safeArea.width);
        
                float referenceResolution = worldHeight/Screen.height;
                bottomUnits = referenceResolution * bottomPixels;
                topUnits = referenceResolution * topPixels;
                leftUnits = referenceResolution * leftPixels;
                rightUnits = referenceResolution * rightPixels;
            }


            Vector3 viewportOrigin;

            switch (anchorType)
            {
                case AnchoToScreenType.center:
                    viewportOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane));
                    viewportOrigin.x = viewportOrigin.x + ((leftUnits-rightUnits)/2);
                    viewportOrigin.y = viewportOrigin.y + ((bottomUnits-topUnits)/2);
                    break;

                case AnchoToScreenType.topLeft:
                    viewportOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, Camera.main.nearClipPlane));
                    viewportOrigin.x = viewportOrigin.x + leftUnits;
                    viewportOrigin.y = viewportOrigin.y - topUnits;
                    break;

                case AnchoToScreenType.topCenter:
                    viewportOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1, Camera.main.nearClipPlane));
                    viewportOrigin.x = viewportOrigin.x + ((leftUnits-rightUnits)/2);
                    viewportOrigin.y = viewportOrigin.y - topUnits;
                    break;

                case AnchoToScreenType.topRight:
                    viewportOrigin = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));
                    viewportOrigin.x = viewportOrigin.x - rightUnits;
                    viewportOrigin.y = viewportOrigin.y - topUnits;
                    break;

                case AnchoToScreenType.leftCenter:
                    viewportOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, Camera.main.nearClipPlane));
                    viewportOrigin.x = viewportOrigin.x + leftUnits;
                    viewportOrigin.y = viewportOrigin.y + ((bottomUnits-topUnits)/2);
                    break;

                case AnchoToScreenType.rightCenter:
                    viewportOrigin = Camera.main.ViewportToWorldPoint(new Vector3(1, 0.5f, Camera.main.nearClipPlane));
                    viewportOrigin.x = viewportOrigin.x - rightUnits;
                    viewportOrigin.y = viewportOrigin.y + ((bottomUnits-topUnits)/2);
                    break;

                case AnchoToScreenType.bottomLeft:
                    viewportOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
                    viewportOrigin.x = viewportOrigin.x + leftUnits;
                    viewportOrigin.y = viewportOrigin.y + bottomUnits;
                    break;

                case AnchoToScreenType.bottomCenter:
                    viewportOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0, Camera.main.nearClipPlane));
                    viewportOrigin.x = viewportOrigin.x + ((leftUnits-rightUnits)/2);
                    viewportOrigin.y = viewportOrigin.y + bottomUnits;
                    break;

                case AnchoToScreenType.bottomRight:
                    viewportOrigin = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, Camera.main.nearClipPlane));
                    viewportOrigin.x = viewportOrigin.x - rightUnits;
                    viewportOrigin.y = viewportOrigin.y + bottomUnits;
                    break;

                default:
                    viewportOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane));
                    viewportOrigin.x = viewportOrigin.x + ((leftUnits-rightUnits)/2);
                    viewportOrigin.y = viewportOrigin.y + ((bottomUnits-topUnits)/2);
                    break;
            }

            //Deslocasmento:
            viewportOrigin.x += horizontalDisplacement;
            viewportOrigin.y += verticalDisplacement;

            transform.position = viewportOrigin;

        }
    }
}
