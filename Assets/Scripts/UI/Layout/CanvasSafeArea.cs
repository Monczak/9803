using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasSafeArea : MonoBehaviour
{
    private CanvasScaler scaler;

    public int safeMargin;
    
    public Vector2Int FullResolution { get; private set; }

    public Vector2Int SafeResolution => FullResolution - Vector2Int.one * safeMargin;

    private void Awake()
    {
        scaler = GetComponent<CanvasScaler>();

        FullResolution = new Vector2Int((int)scaler.referenceResolution.x, (int)scaler.referenceResolution.y);
    }
}
