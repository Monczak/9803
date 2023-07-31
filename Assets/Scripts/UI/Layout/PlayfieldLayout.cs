using System;
using System.Collections;
using System.Collections.Generic;
using NineEightOhThree.Math;
using NineEightOhThree.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace NineEightOhThree.UI.Layout
{
    public class PlayfieldLayout : MonoBehaviour
    {
        private RectTransform rectTransform;
        private CanvasSafeArea safeArea;

        public float positionOffset;
        public bool moveToLeft = true;

        public int marginOffset;
        
        private int CenterPos => safeArea.FullResolution.x / 2;
        private int SidePos => safeArea.safeMargin + marginOffset + (int)(rectTransform.sizeDelta.x / 2);

        private Vector2 startSize;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            safeArea = UIUtils.GetRootCanvas(this).GetComponent<CanvasSafeArea>();

            startSize = rectTransform.sizeDelta;
        }
        
        private void Update()
        {
            rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(moveToLeft ? 0f : 1f, 0f);
            
            rectTransform.anchoredPosition = new Vector2(
                (int)Mathf.Lerp(CenterPos, SidePos, positionOffset) * (moveToLeft ? 1 : -1),
                rectTransform.anchoredPosition.y
            );

            rectTransform.sizeDelta = startSize / (Mathf.Max(0, positionOffset - 1) + 1);
        }
    }
}

