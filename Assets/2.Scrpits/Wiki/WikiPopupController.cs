using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WikiPopupController : MonoBehaviour
{
    float xStart;

    // Start is called before the first frame update
    void Start()
    {
        xStart = transform.localPosition.x;

        transform.localPosition = new Vector3(100f,transform.localPosition.y,transform.localPosition.z);
    }

    public void MoveToPosition()
    {   //leva o wiki para o meio da tela
        transform.localPosition = new Vector3(xStart,transform.localPosition.y,transform.localPosition.z);
    }
    public void MoveToStartPosition()
    {  
        transform.localPosition = new Vector3(100f,transform.localPosition.y,transform.localPosition.z);
    }
}

