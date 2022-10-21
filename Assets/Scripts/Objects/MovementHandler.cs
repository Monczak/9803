using System;
using System.Collections.Generic;
using NineEightOhThree.Classes;
using NineEightOhThree.Managers;
using NineEightOhThree.Math;
using NineEightOhThree.VirtualCPU.Utilities;
using UnityEngine;

namespace NineEightOhThree.Objects
{
    [RequireComponent(typeof(GridTransform)), RequireComponent(typeof(BoxCollider2D))]
    public class MovementHandler : MonoBehaviour
    {
        [HideInInspector] public GridTransform gridTransform;

        public bool autoMove = true;

        public BoxCollider2D Collider { get; private set; }

        public Vector2 velocity;

        private List<RaycastHit2D> horizontalCastHits = new(), verticalCastHits = new(), cornerHCastHits = new(), cornerVCastHits = new();
        private HashSet<CollisionInfo> allHits = new(), previousAllHits = new();
        
        public delegate void CollisionEventHandler(CollisionInfo info);

        public event CollisionEventHandler CollisionEnter;
        public event CollisionEventHandler CollisionStay;
        public event CollisionEventHandler CollisionExit;
        
        private void Awake()
        {
            Collider = GetComponent<BoxCollider2D>();
            gridTransform = GetComponent<GridTransform>();
            
            ColliderCache.Instance.Register(Collider, this);
            ColliderCache.Instance.Register(Collider, gridTransform);
        }

        private void Update()
        {
            if (autoMove)
                Translate(velocity * Time.deltaTime);
        }

        public void Translate(Vector2 delta)
        {
            var collisionData = PredictCollisions(delta);
            Move(delta * collisionData.filteredDirection);

            gridTransform.TruePosition += collisionData.hDelta + collisionData.vDelta;

            UpdateListeners();
        }

        private void UpdateListeners()
        {
            allHits.Clear();

            float maxHitDistance = gridTransform.UnitsPerPixel / 2;
            
            foreach (var hit in horizontalCastHits)
            {
                if (hit.distance < maxHitDistance)
                    allHits.Add(new CollisionInfo(hit));
            }
            foreach (var hit in verticalCastHits)
            {
                if (hit.distance < maxHitDistance)
                    allHits.Add(new CollisionInfo(hit));
            }
            
            foreach (var info in allHits)
            {
                if (!previousAllHits.Contains(info))
                {
                    CollisionEnter?.Invoke(info);
                    ColliderCache.Instance.Get<MovementHandler>(info.Collider)?.CollisionEnter?.Invoke(info.WithOrigin(gameObject));
                }
                else
                {
                    CollisionStay?.Invoke(info);
                    ColliderCache.Instance.Get<MovementHandler>(info.Collider)?.CollisionStay?.Invoke(info.WithOrigin(gameObject));
                }
            }

            foreach (var info in previousAllHits)
            {
                if (allHits.Contains(info))
                    continue;
                
                CollisionExit?.Invoke(info);
                ColliderCache.Instance.Get<MovementHandler>(info.Collider)?.CollisionExit?.Invoke(info.WithOrigin(gameObject));
            }

            previousAllHits = new HashSet<CollisionInfo>(allHits);
        }

        private void Move(Vector2 delta)
        {
            gridTransform.TruePosition += delta;
        }

        private (Vector2 filteredDirection, Vector2 hDelta, Vector2 vDelta) PredictCollisions(Vector2 direction)
        {
            float CastDistance(Vector2 dir)
            {
                return Mathf.Max(dir.magnitude / gridTransform.pixelsPerUnit, gridTransform.UnitsPerPixel);
            }

            int BoxCast(Vector2 dir, List<RaycastHit2D> results, Vector2 origin)
            {
                int layer = gameObject.layer;
                gameObject.layer = 0;   // Temporarily move this object to the default layer (or at least one not hit by the wall filter)
                
                int hits = Physics2D.BoxCast(
                    origin,
                    Collider.size - Vector2.one * (gridTransform.UnitsPerPixel / 2),
                    0,
                    dir,
                    ContactFilters.WallFilter,
                    results,
                    CastDistance(dir));

                gameObject.layer = layer;
                return hits;
            }

            Vector2 HandleHit(RaycastHit2D hit, ref Vector2 filter, Vector2 dir)
            {
                if (Vector2.Dot(direction.normalized, hit.normal) < -0.001f)
                {
                    Vector2 delta = (hit.point - gridTransform.QuantizedPosition - Collider.size / 2 * dir) *
                                    MathExtensions.Abs(dir);
                    delta = MathExtensions.Quantize(delta, gridTransform.pixelsPerUnit);
                    // Debug.Log($"Point: {hit.point} Distance: {hit.distance} Normal: {hit.normal} Direction: {direction} Delta: {delta}");
                    
                    filter *= new Vector2((int)Mathf.Abs(hit.normal.y), (int)Mathf.Abs(hit.normal.x));
                    return delta;
                }
                return Vector2.zero;
            }

            int hHitCount = BoxCast(direction * Vector2.right, horizontalCastHits, gridTransform.QuantizedPosition);
            int vHitCount = BoxCast(direction * Vector2.up, verticalCastHits, gridTransform.QuantizedPosition);

            int cornerHitCount = 0;
            if (!BugManager.Instance.IsActive("clipThroughCorners"))
            {
                if (MathExtensions.IsDiagonal(direction))
                {
                    int hHits = BoxCast(direction * Vector2.right, cornerHCastHits,
                        gridTransform.QuantizedPosition + Vector2.up * CastDistance(direction * Vector2.up) * direction.normalized);
                    int vHits = BoxCast(direction * Vector2.up, cornerVCastHits,
                        gridTransform.QuantizedPosition + Vector2.right * CastDistance(direction * Vector2.right) * direction.normalized);
                    cornerHitCount = Mathf.Max(hHits, vHits);
                }
            }

            if (hHitCount == 0 && vHitCount == 0 && cornerHitCount == 0) return (Vector2.one, Vector2.zero, Vector2.zero);

            bool cornerClipDetected = false;
            if (hHitCount == 0 && vHitCount == 0 && cornerHitCount != 0)
            {
                cornerClipDetected = true;
                horizontalCastHits.AddRange(cornerHCastHits);
                verticalCastHits.AddRange(cornerVCastHits);
            }

            Vector2 filter = Vector2.one;
            Vector2 minHDelta = Vector2.right * (hHitCount == 0 ? 0 : 100000), minVDelta = Vector2.up * (vHitCount == 0 ? 0 : 100000);

            foreach (var hit in horizontalCastHits)
            {
                Vector2 delta = HandleHit(hit, ref filter, (direction * Vector2.right).normalized);
                if (Mathf.Abs(delta.x) < Mathf.Abs(minHDelta.x))
                    minHDelta = delta;
            }
            foreach (var hit in verticalCastHits)
            {
                Vector2 delta = HandleHit(hit, ref filter, (direction * Vector2.up).normalized);
                if (Mathf.Abs(delta.y) < Mathf.Abs(minVDelta.y))
                    minVDelta = delta;
            }
            
            // Prefer horizontal movement when corner clip is detected
            if (cornerClipDetected)
                filter.x = 1;

            return (filter, minHDelta, minVDelta);
        }

        private void OnDestroy()
        {
            ColliderCache.Instance.Unregister(Collider, this);
            ColliderCache.Instance.Unregister(Collider, gridTransform);
        }
    }
}