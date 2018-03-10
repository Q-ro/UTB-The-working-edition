using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewScreenShake : MonoBehaviour
{

    public float shakeAmount = 0.4f; //The force fo the shake
    public float shakeDuration = 1.5f; //The duration of the shake
    public float decreaseFactor = 1.0f;//How fast it should stop shaking
    private Vector3 _originalPos;//Where was the camera before it started shaking

    void OnEnable()
    {
        _originalPos = transform.localPosition;
    }

    public void SetShake()
    {
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        var duration = shakeDuration;
        _originalPos = transform.localPosition;
        while (duration > 0)
        {
            transform.localPosition = _originalPos + Random.insideUnitSphere * shakeAmount;
            duration -= Time.deltaTime;

            //NewGameManager.Instance.FreezePlayerActions();

            yield return null;
        }
        //NewGameManager.Instance.UnfreezePlayerActions();
        transform.localPosition = _originalPos;
    }
}
