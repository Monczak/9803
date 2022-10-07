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
        private Vector2 PosDelta => (Vector2)pixelPos.GetValue<Vector2Byte>() - truePosition;

        public int pixelsPerUnit;
        public float zPosition;

        public float UnitsPerPixel => 1.0f / pixelsPerUnit;

        private new void Awake()
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
        private new void Update()
        {
            base.Update();

            SyncPositions();
        }

        private void SyncPositions()
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

            pixelPos.SetValue((Vector2Byte)truePosition);
            if (((Vector2)pixelPos.GetValue<Vector2Byte>() - truePosition).magnitude > 1)
            {
                SyncTransformWithPixelPos();
            }

            truePositionDirty = false;
        }

        private void SyncTransformWithPixelPos()
        {
            Vector2Byte pos = pixelPos.GetValue<Vector2Byte>();
            transform.position = new Vector3((float)pos.x / pixelsPerUnit, (float)pos.y / pixelsPerUnit, zPosition);
        }

        private new void LateUpdate()
        {
            base.LateUpdate();

            /*// Sync with transform.position in case it gets modified (by Rigidbody2D for example)
            if (((Vector2)transform.position - TruePosition).magnitude > 1.0f / pixelsPerUnit / 2)
                TruePosition = transform.position * pixelsPerUnit;*/
        }

        public void SyncWithTransform()
        {
            truePosition = transform.position * pixelsPerUnit;
        }
    }
}

