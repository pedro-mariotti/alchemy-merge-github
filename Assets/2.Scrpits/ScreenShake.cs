using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    Transform transform;
    float shakeDuration;
    float shakeMagnitude = 0.05f, endSpeed = .1f;
    Vector3 initialPosition;
    void Awake()
    {
        if (transform == null)
        {
            transform = gameObject.transform;
        }
    }

    // Update is called once per frame
    void OnEnable()
    {
        initialPosition = transform.localPosition;
    }
    private void Update()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = new Vector3(initialPosition.x + Random.insideUnitSphere.x * shakeMagnitude, initialPosition.y + Random.insideUnitSphere.y * shakeMagnitude, transform.localPosition.z);

            shakeDuration -= Time.deltaTime * endSpeed;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = initialPosition;
        }
    }
    public void TriggerShake()
    {
        shakeDuration = .02f;
    }
}
