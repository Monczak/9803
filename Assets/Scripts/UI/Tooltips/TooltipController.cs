using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NineEightOhThree.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NineEightOhThree.UI.Tooltips
{
    public class TooltipController : MonoBehaviour
    {
        [Header("Positioning")]
        public RectTransform referenceTransform;
        public RectTransform rescaleTransform;
        private RectTransform referenceCanvasTransform;

        [Header("Prefabs")] public GameObject tooltipPrefab;

        private RectTransform rectTransform;
        
        private UIControls controls;
        
        private List<TooltipDataProvider> dataProviders;
        private HashSet<TooltipDataProvider> activeDataProviders;
        private Vector2 gameMousePos;

        private Dictionary<TooltipDataProvider, Tooltip> tooltipDict;

        public event EventHandler<TooltipDataProvider> OnTooltipActivate;
        public event EventHandler<TooltipDataProvider> OnTooltipDeactivate;

        private void Awake()
        {
            controls = new UIControls();
            controls.UI.Point.performed += OnPoint;
            controls.Enable();

            referenceCanvasTransform = referenceTransform.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

            dataProviders = FindObjectsOfType<TooltipDataProvider>().ToList();
            activeDataProviders = new HashSet<TooltipDataProvider>();
            tooltipDict = new Dictionary<TooltipDataProvider, Tooltip>();

            rectTransform = GetComponent<RectTransform>();
        }

        private void OnPoint(InputAction.CallbackContext obj)
        {
            Vector2 mousePos = obj.ReadValue<Vector2>();
            rectTransform.anchoredPosition = GetRescaledMousePos(mousePos);
            
            Vector2 referenceMousePos = GetReferenceMousePos(mousePos);
            gameMousePos =
                GameManager.Instance.GameCamera.ViewportToWorldPoint(referenceMousePos / referenceTransform.sizeDelta);
            
            // Debug.Log($"{providers.Count} providers");
            // foreach (var provider in providers)
            // {
            //     StringBuilder builder = new();
            //
            //     builder.Append(provider.gameObject.name).Append(":\n");
            //
            //     var data = provider.GetAllData();
            //     foreach (TooltipData dataPiece in data)
            //     {
            //         builder.Append("-- ").Append(dataPiece.Header).Append(" --\n");
            //         foreach (var (line, address) in dataPiece.Lines)
            //         {
            //             builder.AppendLine($"{line} ({address:X4})");
            //         }
            //     }
            //     
            //     Debug.Log(builder.ToString());
            // }
        }

        private void Update()
        {
            HandleTooltipProviders(gameMousePos);
        }

        private void SpawnTooltip(TooltipDataProvider provider)
        {
            Tooltip tooltip = Instantiate(tooltipPrefab, transform).GetComponent<Tooltip>();
            tooltip.Make(provider);

            tooltipDict[provider] = tooltip;
        }

        private void DespawnTooltip(TooltipDataProvider provider)
        {
            if (tooltipDict.ContainsKey(provider))
            {
                tooltipDict[provider].Despawn();
                tooltipDict.Remove(provider);
            }
            else
            {
                Logger.LogWarning($"Tried to despawn nonexistent tooltip for {provider.gameObject.name}!");
            }
        }

        private void HandleTooltipProviders(Vector2 mousePos)
        {
            var providers = FindOverlappingDataProviders(mousePos);

            foreach (var provider in providers.Where(provider => !activeDataProviders.Contains(provider)))
            {
                OnTooltipActivate?.Invoke(this, provider);
                // Debug.Log($"Activate {provider.gameObject.name}");
                
                SpawnTooltip(provider);
            }
            
            foreach (TooltipDataProvider provider in activeDataProviders)
            {
                tooltipDict[provider].UpdateData();
            }

            foreach (var provider in activeDataProviders.Where(provider => !providers.Contains(provider)))
            {
                OnTooltipDeactivate?.Invoke(this, provider);
                // Debug.Log($"Deactivate {provider.gameObject.name}");
                
                DespawnTooltip(provider);
            }

            activeDataProviders = new HashSet<TooltipDataProvider>(providers);
        }

        // TODO: Probably needs optimization since we're doing this every frame
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
            var mouseScreenPos = GetRescaledMousePos(pos);

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

        private Vector2 GetRescaledMousePos(Vector2 pos)
        {
            Vector2 mouseViewportPos = GameManager.Instance.UICamera.ScreenToViewportPoint(pos);
            Vector2 mouseScreenPos = mouseViewportPos * referenceCanvasTransform.sizeDelta;
            return mouseScreenPos;
        }
    }
}
