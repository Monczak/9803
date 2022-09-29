using UnityEngine;

namespace NineEightOhThree.Objects
{
    public class CollisionInfo
    {
        public RaycastHit2D hit;

        public Vector2 Point => hit.point;
        public Vector2 Normal => hit.normal;
        public Transform Transform => hit.transform;
        public Collider2D Collider => hit.collider;
        public GameObject Origin { get; private set; }

        public CollisionInfo(RaycastHit2D hit)
        {
            this.hit = hit;
            Origin = hit.transform.gameObject;
        }

        public CollisionInfo WithOrigin(GameObject origin)
        {
            var newInfo = new CollisionInfo(hit)
            {
                Origin = origin
            };
            return newInfo;
        }

        public override bool Equals(object obj)
        {
            if (obj is not CollisionInfo info)
                return false;
            return info.Collider == Collider;
        }

        public override int GetHashCode()
        {
            return Collider.GetHashCode();
        }
    }
}