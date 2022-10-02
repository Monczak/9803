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
        private GridTransform gridTransform;
        private new BoxCollider2D collider;
        
        public Vector2 velocity;
        public ContactFilter2D wallFilter;

        private List<RaycastHit2D> horizontalCastHits = new(), verticalCastHits = new(), cornerHCastHits = new(), cornerVCastHits = new();
        private HashSet<CollisionInfo> allHits = new(), previousAllHits = new();

        public delegate void CollisionEventHandler(CollisionInfo info);

        public event CollisionEventHandler CollisionEnter;
        public event CollisionEventHandler CollisionStay;
        public event CollisionEventHandler CollisionExit;
        
        private void Awake()
        {
            collider = GetComponent<BoxCollider2D>();
            gridTransform = GetComponent<GridTransform>();
            
            ColliderCache.Instance.Register(collider, this);
            ColliderCache.Instance.Register(collider, gridTransform);
        }

        private void Update()
        {
            var collisionData = PredictCollisions(velocity);
            Move(collisionData.filteredDirection);
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
                if (allHits.Contains(info)) continue;
                
                CollisionExit?.Invoke(info);
                ColliderCache.Instance.Get<MovementHandler>(info.Collider)?.CollisionExit?.Invoke(info.WithOrigin(gameObject));
            }

            previousAllHits = new HashSet<CollisionInfo>(allHits);
        }

        private void Move(Vector2 filteredDirection)
        {
            gridTransform.TruePosition += velocity * filteredDirection * Time.deltaTime;
        }

        private (Vector2 filteredDirection, Vector2 hDelta, Vector2 vDelta) PredictCollisions(Vector2 direction)
        {
            float CastDistance(Vector2 dir)
            {
                return Mathf.Max(dir.magnitude * Time.deltaTime, gridTransform.UnitsPerPixel);
            }

            int BoxCast(Vector2 dir, List<RaycastHit2D> results, Vector2 origin)
            {
                return Physics2D.BoxCast(
                    origin,
                    collider.size - Vector2.one * (gridTransform.UnitsPerPixel / 2),
                    0,
                    dir,
                    wallFilter,
                    results,
                    CastDistance(dir));
            }

            Vector2 HandleHit(RaycastHit2D hit, ref Vector2 filter, Vector2 dir)
            {
                Vector2 delta = (hit.point - (Vector2)transform.position - collider.size / 2 * dir) *
                                MathExtensions.Abs(dir);
                delta = MathExtensions.Quantize(delta, gridTransform.pixelsPerUnit);
                // Debug.Log($"Point: {hit.point} Distance: {hit.distance} Normal: {hit.normal} Direction: {direction} Delta: {delta}");

                if (Vector2.Dot(direction, hit.normal) < 0)
                    filter *= new Vector2((int)Mathf.Abs(hit.normal.y), (int)Mathf.Abs(hit.normal.x));

                return delta;
            }

            int hHitCount = BoxCast(direction * Vector2.right, horizontalCastHits, transform.position);
            int vHitCount = BoxCast(direction * Vector2.up, verticalCastHits, transform.position);

            int cornerHitCount = 0;
            if (!BugManager.Instance.IsActive("clipThroughCorners"))
            {
                if (direction.x * direction.y != 0)
                {
                    int hHits = BoxCast(direction * Vector2.right, cornerHCastHits,
                        (Vector2)transform.position + Vector2.up * CastDistance(direction * Vector2.up) * direction.normalized);
                    int vHits = BoxCast(direction * Vector2.up, cornerVCastHits,
                        (Vector2)transform.position + Vector2.right * CastDistance(direction * Vector2.right) * direction.normalized);
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
            ColliderCache.Instance.Unregister(collider);
        }
    }
}