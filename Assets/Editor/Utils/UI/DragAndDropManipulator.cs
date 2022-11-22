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

        private VisualElement targetClone;
        private VisualTreeAsset targetTemplate;
        private VisualElementConstructor targetConstructor;
        private VisualElementBinder targetBinder;

        private Vector2 targetStartPos, targetCloneStartPos;
        private Vector3 pointerStartPos;
        private Vector3 pointerPos;

        private bool dragging;

        public delegate VisualElement VisualElementConstructor(VisualTreeAsset template);
        public delegate void VisualElementBinder(VisualElement item, object data);

        public event EventHandler<(bool success, VisualElement slot, Vector2 startPos)> OnDrop;

        public DragAndDropManipulator(VisualElement root, VisualTreeAsset targetTemplate, VisualElementConstructor targetConstructor, VisualElementBinder targetBinder)
        {
            this.root = root;
            this.targetTemplate = targetTemplate;
            this.targetConstructor = targetConstructor;
            this.targetBinder = targetBinder;
            
            target = ConstructElement().contentContainer;
        }

        private VisualElement ConstructElement()
        {
            return targetConstructor(targetTemplate);
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
            target.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            targetClone = CreateClone();
            targetClone.BringToFront();

            targetStartPos = target.transform.position;
            targetCloneStartPos = targetClone.transform.position;
            pointerStartPos = evt.position;
            target.CapturePointer(evt.pointerId);

            target.style.visibility = new StyleEnum<Visibility>(Visibility.Hidden);
            
            dragging = true;
        }

        private VisualElement CreateClone()
        {
            VisualElement clone = ConstructElement();
            targetBinder(clone, target.userData);
            
            clone.style.width = target.layout.width;
            clone.style.height = target.layout.height;
            clone.style.position = new StyleEnum<Position>(Position.Absolute);
            clone.transform.position = RootPos(target);
            
            root.Add(clone);
            return clone;
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
                targetClone.transform.position = (Vector3)targetCloneStartPos + pointerDelta;
            }
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (dragging && target.HasPointerCapture(evt.pointerId))
            {
                target.ReleasePointer(evt.pointerId);
                target.style.visibility = new StyleEnum<Visibility>(Visibility.Visible);
            }
        }
        
        private void OnPointerCaptureOut(PointerCaptureOutEvent evt)
        {
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
                root.Remove(targetClone);
            }
        }

        private Vector3 RootPos(VisualElement elem)
        {
            return root.WorldToLocal(elem.parent.LocalToWorld(elem.layout.position));
        }
    }
}

