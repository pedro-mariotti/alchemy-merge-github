using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollControl : MonoBehaviour
{
    private Vector2 touchStartPosition;
    private Vector2 currentPosition;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 initPosition;
    public float sensitivity = 0.05f;
    public float smoothing = 0.1f;

    void Start()
    {
        initPosition = transform.localPosition;
        startPosition = transform.localPosition;
        targetPosition = transform.localPosition;
    }

    public void restart()
    {
        transform.localPosition = initPosition;
        startPosition = initPosition;
        targetPosition = initPosition;
    }

      void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                targetPosition = transform.localPosition;
                startPosition = transform.localPosition;
                
            }
            else if (touch.phase == TouchPhase.Moved)
            {   
                Vector2 touchDeltaPosition = touch.deltaPosition;
                targetPosition += new Vector3(0, touchDeltaPosition.y * sensitivity, 0);
                float distance = Vector3.Distance(startPosition, targetPosition);
                if(distance>= .5f )
                {
                    PCSettings.inScrolling = true;
                }
            }
        }
        else
        {
            PCSettings.inScrolling= false;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, smoothing);
    }
}