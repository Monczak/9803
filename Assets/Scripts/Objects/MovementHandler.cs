using System.Collections.Generic;
using NineEightOhThree.Math;
using UnityEngine;

namespace NineEightOhThree.Objects
{
    [RequireComponent(typeof(GridTransform))]
    public class MovementHandler : MonoBehaviour
    {
        private GridTransform gridTransform;
        private new BoxCollider2D collider;
        
        public Vector2 velocity;
        public ContactFilter2D wallFilter;

        private List<RaycastHit2D> horizontalCastHits = new(), verticalCastHits = new();
        
        
        private void Awake()
        {
            collider = GetComponent<BoxCollider2D>();
            gridTransform = GetComponent<GridTransform>();
        }

        private void Update()
        {
            var collisionData = PredictCollisions(velocity);
            Move(collisionData.filteredDirection);
            gridTransform.TruePosition += collisionData.hDelta + collisionData.vDelta;
        }

        private void Move(Vector2 filteredDirection)
        {
            gridTransform.TruePosition += velocity * filteredDirection * Time.deltaTime;
        }

        private (Vector2 filteredDirection, Vector2 hDelta, Vector2 vDelta) PredictCollisions(Vector2 direction)
        {
            int BoxCast(Vector2 dir, List<RaycastHit2D> results)
            {
                return Physics2D.BoxCast(
                    transform.position,
                    collider.size - Vector2.one * (1.0f / gridTransform.pixelsPerUnit / 2),
                    0,
                    dir,
                    wallFilter,
                    results,
                    Mathf.Max(dir.magnitude * Time.deltaTime, 1.0f / gridTransform.pixelsPerUnit));
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

            int hHitCount = BoxCast(direction * Vector2.right, horizontalCastHits);
            int vHitCount = BoxCast(direction * Vector2.up, verticalCastHits);
            if (hHitCount == 0 && vHitCount == 0) return (Vector2.one, Vector2.zero, Vector2.zero);

            Vector2 filter = Vector2.one;
            Vector2 minHDelta = Vector2.right * (hHitCount == 0 ? 0 : 100000), minVDelta = Vector2.up * (vHitCount == 0 ? 0 : 100000);

            foreach (var hit in horizontalCastHits)
            {
                Vector2 delta = HandleHit(hit, ref filter, (direction * Vector2.right).normalized);
                if (delta.x < minHDelta.x)
                    minHDelta = delta;
            }
            foreach (var hit in verticalCastHits)
            {
                Vector2 delta = HandleHit(hit, ref filter, (direction * Vector2.up).normalized);
                if (delta.y < minVDelta.y)
                    minVDelta = delta;
            }

            return (filter, minHDelta, minVDelta);
        }
    }
}