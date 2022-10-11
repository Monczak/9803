using System;
using System.Collections.Generic;
using System.Linq;
using NineEightOhThree.Math;
using NineEightOhThree.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NineEightOhThree.Controllers
{
    [RequireComponent(typeof(GridTransform)), RequireComponent(typeof(MovementHandler))]
    public class PlayerController : MonoBehaviour
    {
        public float speed;             // Pixels per second
        public float sidePushZoneSize;  // Pixels
        public float pushAmount;        // Pixels per push
        
        private GridTransform gridTransform;
        private MovementHandler movementHandler;

        private new BoxCollider2D collider;

        private Pushable pushedObject;
        private List<Pushable> touchedPushables;
        private Pushable nearestTouchedPushable;
        private float lastPushTime;

        private PlayerControls controls;

        private float SidePushZoneSize => sidePushZoneSize / gridTransform.pixelsPerUnit;

        private Vector2 input;
        private float relativeAngle;

        private Vector2 desiredVelocity;

        private List<RaycastHit2D> hits = new();    // Used in CheckCorners() as a discard
        
        private void Awake()
        {
            controls = new PlayerControls();
            gridTransform = GetComponent<GridTransform>();
            movementHandler = GetComponent<MovementHandler>();
            collider = GetComponent<BoxCollider2D>();

            touchedPushables = new List<Pushable>();

            controls.Movement.Move.performed += OnMove;
            controls.Movement.Move.canceled += OnMove;
            
            controls.Enable();
            
            movementHandler.CollisionEnter += MovementHandlerOnCollisionEnter;
            movementHandler.CollisionStay += MovementHandlerOnCollisionStay;
            movementHandler.CollisionExit += MovementHandlerOnCollisionExit;
        }

        private void MovementHandlerOnCollisionEnter(CollisionInfo info)
        {
            if (ColliderCache.Instance.TryGet(info.Collider, out Pushable pushable))
            {
                touchedPushables.Add(pushable);
            }
        }
        
        private void MovementHandlerOnCollisionStay(CollisionInfo info)
        {
            nearestTouchedPushable = GetNearestPushable();
        }
        
        private void MovementHandlerOnCollisionExit(CollisionInfo info)
        {
            if (ColliderCache.Instance.TryGet(info.Collider, out Pushable pushable))
            {
                touchedPushables.Remove(pushable);
                if (nearestTouchedPushable == pushable)
                    nearestTouchedPushable = null;
            }
        }

        private void OnMove(InputAction.CallbackContext obj)
        {
            input = obj.ReadValue<Vector2>();
        }

        // Start is called before the first frame update
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {
            CheckCorners();
            desiredVelocity = MathExtensions.RotateDegrees(input, relativeAngle) * speed;

            movementHandler.velocity = desiredVelocity;

            // Debug.Log($"Pushed object: {(pushedObject is not null ? pushedObject.gameObject.name : "(none)") }");
            
            HandlePushables();
        }

        private void HandlePushables()
        {
            if (nearestTouchedPushable is null || touchedPushables.Count == 0)
            {
                pushedObject = null;
                return;
            }
            pushedObject = nearestTouchedPushable;
            
            if (!MathExtensions.IsDiagonal(desiredVelocity))
            {
                PushPushable();
            }
        }

        private void PushPushable()
        {
            if (Time.time > lastPushTime + pushedObject.pushDelay)
            {
                Vector2 delta = desiredVelocity.normalized * pushAmount;
                
                pushedObject.movementHandler.Translate(delta);
                movementHandler.Translate(delta);

                lastPushTime = Time.time;
            }
        }

        private Pushable GetNearestPushable()
        {
            if (touchedPushables.Count == 0)
                return null;
            
            Pushable nearestPushable = touchedPushables[0];
            foreach (var pushable in touchedPushables)
                if (Vector2.Distance(transform.position, pushable.transform.position) <
                    Vector2.Distance(transform.position, nearestPushable.transform.position))
                    nearestPushable = pushable;

            return nearestPushable;
        }

        private void CheckCorners()
        {
            int BoxCast(Vector2 origin, Vector2 boxSize)
            {
                int layer = gameObject.layer;
                gameObject.layer = 0;   // Temporarily move this object to the default layer (or at least one not hit by the wall filter)
                
                int hitCount = Physics2D.BoxCast(
                    origin,
                    boxSize,
                    0,
                    input,
                    movementHandler.wallFilter,
                    hits,
                    gridTransform.UnitsPerPixel
                );
                
                gameObject.layer = layer;
                return hitCount;
            }
            
            Vector2 GetBoxCenter(Vector2 pos) =>
                new(pos.x * (collider.size.x - SidePushZoneSize) / 2,
                    pos.y * (collider.size.y - SidePushZoneSize) / 2);
        
            Vector2 cornerDetectionBoxSize = Vector2.one * SidePushZoneSize;
        
            Vector2 horizontalDetectionBoxSize = new(collider.size.x - 2 * SidePushZoneSize, SidePushZoneSize);
            Vector2 verticalDetectionBoxSize = new(SidePushZoneSize, collider.size.y - 2 * SidePushZoneSize);

            if (MathExtensions.IsDiagonal(input) || input == Vector2.zero)
            {
                relativeAngle = 0;
                return;
            } 

            Vector2 leftCornerPos = input.normalized + MathExtensions.RotateDegrees(input.normalized, 90);
            Vector2 centralPos = input.normalized;
            Vector2 rightCornerPos = input.normalized + MathExtensions.RotateDegrees(input.normalized, -90);

            Vector2 leftOrigin = (Vector2)transform.position + GetBoxCenter(leftCornerPos);
            Vector2 centralOrigin = (Vector2)transform.position + GetBoxCenter(centralPos) - input.normalized * gridTransform.UnitsPerPixel;
            Vector2 rightOrigin = (Vector2)transform.position + GetBoxCenter(rightCornerPos);

            int leftHitCount = BoxCast(leftOrigin,
                cornerDetectionBoxSize - Vector2.one * (gridTransform.UnitsPerPixel * 2));
            int centralHitCount = BoxCast(centralOrigin,
                (input.x == 0 ? horizontalDetectionBoxSize : verticalDetectionBoxSize));
            int rightHitCount = BoxCast(rightOrigin,
                cornerDetectionBoxSize - Vector2.one * (gridTransform.UnitsPerPixel * 2));
            
            // Debug.Log($"L {leftHitCount} C {centralHitCount} R {rightHitCount}");

            if (leftHitCount != 0 && centralHitCount == 0 && rightHitCount == 0)
                relativeAngle = -90;
            else if (leftHitCount == 0 && centralHitCount == 0 && rightHitCount != 0)
                relativeAngle = 90;
            else
                relativeAngle = 0;
        }
    }
}

