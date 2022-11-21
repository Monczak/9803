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
        private VisualElement container;

        private Vector2 targetStartPos;
        private Vector3 pointerStartPos;
        private Vector3 pointerPos;

        private bool dragging;

        public event EventHandler<(bool success, VisualElement slot, Vector2 startPos)> OnDrop;

        public DragAndDropManipulator(VisualElement root, VisualElement container, VisualElement target)
        {
            this.root = root;
            this.container = container;
            this.target = target;
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
                pointerPos = evt.position;
                Vector3 pointerDelta = evt.position - pointerStartPos;

                /*target.transform.position = new Vector2(
                    Mathf.Clamp(targetStartPos.x + pointerDelta.x, 0, target.panel.visualTree.worldBound.width),
                    Mathf.Clamp(targetStartPos.y + pointerDelta.y, 0, target.panel.visualTree.worldBound.height));*/
                target.transform.position = (Vector3)targetStartPos + pointerDelta;
            }
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (dragging && target.HasPointerCapture(evt.pointerId))
                target.ReleasePointer(evt.pointerId);
        }
        
        private void OnPointerCaptureOut(PointerCaptureOutEvent evt)
        {
            Vector3 RootPos(VisualElement elem)
            {
                return root.WorldToLocal(root.LocalToWorld(elem.layout.position));
            }
            
            if (dragging)
            {
                List<VisualElement> overlappingSlots = root.Query<VisualElement>(className: "dragdrop-slot")
                    .Where(slot => slot.worldBound.Overlaps(new Rect(pointerPos, Vector2.one)))
                    .ToList();

                VisualElement closestOverlappingSlot = null;
                if (overlappingSlots.Count != 0)
                {
                    closestOverlappingSlot = overlappingSlots.MinBy(slot => (RootPos(slot) - target.transform.position).sqrMagnitude);
                }

                OnDrop?.Invoke(target, (closestOverlappingSlot is not null, closestOverlappingSlot, targetStartPos));

                dragging = false;
            }
        }
    }
}

