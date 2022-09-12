using NineEightOhThree.Math;
using NineEightOhThree.VirtualCPU.Interfacing;
using UnityEngine;

namespace NineEightOhThree.Objects
{
    public class GridTransform : MemoryBindableBehavior
    {
        [BindableType(BindableType.Vector2Byte), HideInInspector]
        public Bindable pixelPos;

        public int pixelsPerUnit;
        public float zPosition;

        private new void Awake()
        {
            base.Awake();

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        private new void Update()
        {
            base.Update();

            Vector2Byte currentPos = pixelPos.GetValue<Vector2Byte>();
            transform.position = new Vector3((float)currentPos.x / pixelsPerUnit, (float)currentPos.y / pixelsPerUnit, zPosition);
        }
    }
}

