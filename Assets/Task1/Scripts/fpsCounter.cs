using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class fpsCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _fpsText;
    int _framesCount = 30;
    Queue<float> _fpsQueue = new Queue<float>();
    float sum = 0;
    void Start()
    {
        StartCoroutine(FPSTextUpdate());
    }

    void Update()
    {
        float currentFPS = 1.0f / Time.deltaTime;
        _fpsQueue.Enqueue(currentFPS);
        sum += currentFPS;
        if (_fpsQueue.Count >= _framesCount)
        {
            sum -= _fpsQueue.Dequeue();
        }

    }
    IEnumerator FPSTextUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            _fpsText.text = $"FPS: {(int)(sum / _fpsQueue.Count)}";
        }
    }
}
