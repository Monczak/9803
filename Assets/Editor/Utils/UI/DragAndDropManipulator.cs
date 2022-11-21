using System;
using System.Collections.Generic;
using NineEightOhThree.Classes;
using UnityEngine;
using UnityEngine.UIElements;

namespace NineEightOhThree.Editor.Utils.UI 
{
    public class DragAndDropManipulator : PointerManipulator
    {
        private VisualElement root;

        private Vector2 targetStartPos;
        private Vector3 pointerStartPos;

        private bool dragging;

        public event EventHandler<VisualElement> OnDropSuccess;
        public event EventHandler<Vector2> OnDropFailure;

        public DragAndDropManipulator(VisualElement target)
        {
            this.target = target;
            root = target.parent;
        }
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
            target.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
        }


        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            targetStartPos = target.transform.position;
            pointerStartPos = evt.position;
            target.CapturePointer(evt.pointerId);
            dragging = true;
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (dragging && target.HasPointerCapture(evt.pointerId))
            {
                Vector3 pointerDelta = evt.position - pointerStartPos;

                target.transform.position = new Vector2(
                    Mathf.Clamp(targetStartPos.x + pointerDelta.x, 0, target.panel.visualTree.worldBound.width),
                    Mathf.Clamp(targetStartPos.y + pointerDelta.y, 0, target.panel.visualTree.worldBound.height));
            }
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (dragging && target.HasPointerCapture(evt.pointerId))
                target.ReleasePointer(evt.pointerId);
        }
        
        private void OnPointerCaptureOut(PointerCaptureOutEvent evt)
        {
            Vector3 RootPos(VisualElement elem) => root.WorldToLocal(elem.parent.LocalToWorld(elem.layout.position));
            
            if (dragging)
            {
                List<VisualElement> overlappingSlots = root.Query<VisualElement>(className: "dragdrop-slot")
                    .Where(slot => target.worldBound.Overlaps(slot.worldBound))
                    .ToList();

                VisualElement closestOverlappingSlot = null;
                if (overlappingSlots.Count != 0)
                {
                    closestOverlappingSlot = overlappingSlots.MinBy(slot => RootPos(slot) - target.transform.position);
                }

                if (closestOverlappingSlot != null)
                    OnDropSuccess?.Invoke(target, closestOverlappingSlot);
                else
                    OnDropFailure?.Invoke(target, targetStartPos);

                dragging = false;
            }
        }
    }
}

