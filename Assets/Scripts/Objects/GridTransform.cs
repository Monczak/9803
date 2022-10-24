using NineEightOhThree.Math;
using NineEightOhThree.VirtualCPU.Interfacing;
using UnityEngine;

namespace NineEightOhThree.Objects
{
    public class GridTransform : MemoryBindableBehavior
    {
        [BindableType(BindableType.Vector2Byte), HideInInspector]
        public Bindable pixelPos;

        private Vector2 truePosition;
        private bool truePositionDirty;

        public Vector2 TruePosition
        {
            get => truePosition;
            set
            {
                truePosition = value;
                truePositionDirty = true;
                
                SyncTransform();
            }
        }

        public Vector2 UnitPosition
        {
            get => TruePosition / pixelsPerUnit;
            set
            {
                TruePosition = value * pixelsPerUnit;
                truePositionDirty = true;
            }
        }

        public Vector2 QuantizedPosition
        {
            get => MathExtensions.Quantize(UnitPosition, pixelsPerUnit);
            set
            {
                UnitPosition = MathExtensions.Quantize(value, pixelsPerUnit);
            }
        }
        
        private Vector2 PosDelta => (Vector2)pixelPos.GetValue<Vector2Byte>() - truePosition;

        public int pixelsPerUnit;
        public float zPosition;

        public float UnitsPerPixel => 1.0f / pixelsPerUnit;

        protected override void Awake()
        {
            base.Awake();

            truePosition = transform.position;
            pixelPos.SetValue((Vector2Byte)truePosition);
        }

        // Start is called before the first frame update
        private void Start()
        {

        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            SyncPositions();
        }

        public void SyncPositions()
        {
            if (PosDelta.sqrMagnitude > 1)
            {
                if (pixelPos.dirty) truePosition = (Vector2)pixelPos.GetValue<Vector2Byte>();
                if (truePositionDirty) pixelPos.SetValue((Vector2Byte)truePosition);
            }

            if (pixelPos.dirty || truePositionDirty)
            {
                SyncTransformWithPixelPos();
            }
            else
            {
                SyncWithTransform();
            }

            pixelPos.SetValue((Vector2Byte)TruePosition);
            if (PosDelta.sqrMagnitude > 1)
            {
                SyncTransformWithPixelPos();
            }

            truePositionDirty = false;
        }

        private void SyncTransformWithPixelPos()
        {
            Vector2Byte pos = pixelPos.GetValue<Vector2Byte>();
            transform.position = new Vector3((float)pos.x / pixelsPerUnit, (float)pos.y / pixelsPerUnit, zPosition);
            Physics2D.SyncTransforms();
        }

        public void SyncWithTransform()
        {
            truePosition = transform.position * pixelsPerUnit;
        }

        public void SyncTransform()
        {
            pixelPos.SetValue((Vector2Byte)TruePosition);
            SyncTransformWithPixelPos();
            Physics2D.SyncTransforms();
        }
    }
}

