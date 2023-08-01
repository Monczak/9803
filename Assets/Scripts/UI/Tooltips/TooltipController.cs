using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NineEightOhThree.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace NineEightOhThree.UI.Tooltips
{
    public class TooltipController : MonoBehaviour
    {
        private UIControls controls;

        public RectTransform referenceTransform;
        private RectTransform referenceCanvasTransform;

        public RectTransform rescaleTransform;

        private List<TooltipDataProvider> dataProviders;

        private void Awake()
        {
            controls = new UIControls();
            controls.UI.Point.performed += OnPoint;
            controls.Enable();

            referenceCanvasTransform = referenceTransform.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

            dataProviders = FindObjectsOfType<TooltipDataProvider>().ToList();
        }

        private void OnPoint(InputAction.CallbackContext obj)
        {
            Vector2 mousePos = obj.ReadValue<Vector2>();
            Vector2 referenceMousePos = GetReferenceMousePos(mousePos) / referenceTransform.sizeDelta;

            Vector2 gamePos = GameManager.Instance.GameCamera.ViewportToWorldPoint(referenceMousePos);
            // Debug.Log(referenceMousePos);
            
            var providers = FindOverlappingDataProviders(gamePos);
            Debug.Log($"{providers.Count} providers");
            foreach (var provider in providers)
            {
                StringBuilder builder = new();
            
                builder.Append(provider.gameObject.name).Append(":\n");
            
                var data = provider.GetAllData();
                foreach (TooltipData dataPiece in data)
                {
                    builder.Append("-- ").Append(dataPiece.Header).Append(" --\n");
                    foreach (var (line, address) in dataPiece.Lines)
                    {
                        builder.AppendLine($"{line} ({address:X4})");
                    }
                }
                
                Debug.Log(builder.ToString());
            }
        }

        private HashSet<TooltipDataProvider> FindOverlappingDataProviders(Vector2 pos)
        {
            HashSet<TooltipDataProvider> overlappingProviders = new();
            foreach (var provider in dataProviders.Where(provider => provider.collider.OverlapPoint(pos)))
            {
                overlappingProviders.Add(provider);
            }
            return overlappingProviders;
        }

        private Vector2 GetReferenceMousePos(Vector2 pos)
        {
            Vector2 mouseViewportPos = GameManager.Instance.UICamera.ScreenToViewportPoint(pos);
            Vector2 mouseScreenPos = mouseViewportPos * referenceCanvasTransform.sizeDelta;

            Vector2 referenceScreenBottomLeft = referenceTransform.anchoredPosition + Vector2.up * referenceCanvasTransform.sizeDelta / 2 - referenceTransform.sizeDelta / 2;
            // Vector2 referenceScreenTopRight = referenceScreenBottomLeft + referenceTransform.sizeDelta;

            Vector2 referenceMousePos = mouseScreenPos - referenceScreenBottomLeft;

            if (rescaleTransform)
            {
                Vector2 sizeDelta = rescaleTransform.sizeDelta;
                
                referenceMousePos = new Vector2(
                    Mathf.LerpUnclamped(0, sizeDelta.x, (referenceMousePos / sizeDelta).x),
                    Mathf.LerpUnclamped(0, sizeDelta.y, (referenceMousePos / sizeDelta).y)
                );
            }

            return referenceMousePos;
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
