using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    private TMP_Text text;

    public float sampleInterval = 1f;
    private float lastSampleTime;
    private int frameCount, lastFrameCount;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > lastSampleTime + sampleInterval)
        {
            frameCount = Time.frameCount - lastFrameCount;
            text.text = $"{Mathf.RoundToInt( frameCount / sampleInterval).ToString()} FPS";
            lastFrameCount = Time.frameCount;
            lastSampleTime = Time.time;
        }
    }
}
