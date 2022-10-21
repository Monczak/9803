using System;
using System.Collections.Generic;
using System.Linq;
using NineEightOhThree.Managers;
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

        public float pullDistance;

        private GridTransform gridTransform;
        private MovementHandler movementHandler;

        private new BoxCollider2D collider;

        private Pushable pushedObject;
        private List<Pushable> touchedPushables;
        private Pushable nearestTouchedPushable;
        private float lastPushTime, lastPushableTouchTime;
        private bool wasTouchingPushable;

        private Pushable grabbedObject;
        private Collider2D grabbedObjectCollider;
        private bool isGrabbing;
        private Vector2 grabDirection;

        private PlayerControls controls;

        private float SidePushZoneSize => sidePushZoneSize / gridTransform.pixelsPerUnit;

        private Vector2 input;
        private float relativeAngle;
        private bool grabInput, lastGrabInput;

        private Vector2 desiredVelocity, lastDesiredVelocity;
        private bool freezeInPlace;
        private Vector2 direction;

        private List<RaycastHit2D> hits = new();
        
        private void Awake()
        {
            controls = new PlayerControls();
            gridTransform = GetComponent<GridTransform>();
            movementHandler = GetComponent<MovementHandler>();
            collider = GetComponent<BoxCollider2D>();

            touchedPushables = new List<Pushable>();

            controls.Movement.Move.performed += OnMove;
            controls.Movement.Move.canceled += OnMove;
            controls.Movement.Grab.performed += OnGrab;
            controls.Movement.Grab.canceled += OnGrab;
            
            controls.Enable();
            
            movementHandler.CollisionEnter += MovementHandlerOnCollisionEnter;
            movementHandler.CollisionStay += MovementHandlerOnCollisionStay;
            movementHandler.CollisionExit += MovementHandlerOnCollisionExit;
        }

        private void OnGrab(InputAction.CallbackContext obj)
        {
            grabInput = obj.ReadValue<float>() != 0;
        }

        private void MovementHandlerOnCollisionEnter(CollisionInfo info)
        {
            if (ColliderCache.Instance.TryGet(info.Collider, out Pushable pushable))
            {
                touchedPushables.Add(pushable);
            }
            nearestTouchedPushable = GetNearestPushable();
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
            if (input != Vector2.zero)
                direction = input.normalized;
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

            HandleGrabbing();
            
            if (!freezeInPlace && !isGrabbing)
                movementHandler.Translate(desiredVelocity * Time.deltaTime);

            HandlePushables();

            lastDesiredVelocity = desiredVelocity;
            lastGrabInput = grabInput;
        }

        private void HandleGrabbing()
        {
            if (grabInput && !lastGrabInput)
            {
                Grab();
            }
            else if (!grabInput && lastGrabInput)
            {
                grabbedObject = null;
                grabbedObjectCollider = null;
                isGrabbing = false;
            }

            if (isGrabbing)
            {
                if (grabbedObject is null)
                {
                    Debug.Log($"{grabbedObjectCollider.gameObject.name} (immovable)");
                }
                else
                {
                    Debug.Log($"{grabbedObjectCollider.gameObject.name} (movable)");
                }
            }
        }

        private void Grab()
        {
            int hitCount = BoxCast(gridTransform.QuantizedPosition,
                movementHandler.Collider.size - Vector2.one * gridTransform.UnitsPerPixel, direction, pullDistance, hits);
            if (hitCount > 0)
            {
                isGrabbing = true;
                
                // TODO: Maybe find nearest instead of first?
                RaycastHit2D hit = hits[0];
                grabbedObjectCollider = hit.collider;
                ColliderCache.Instance.TryGet(grabbedObjectCollider, out grabbedObject);

                grabDirection = -hit.normal;
            }
        }

        private void HandlePushables()
        {
            if (isGrabbing && grabbedObject is not null)
            {
                pushedObject = grabbedObject;
            }
            else
            {
                if (nearestTouchedPushable is null || touchedPushables.Count == 0)
                {
                    pushedObject = null;
                    wasTouchingPushable = false;
                    return;
                }
                pushedObject = nearestTouchedPushable;
            }

            if (!wasTouchingPushable)
                lastPushableTouchTime = Time.time;

            if (!MathExtensions.IsDiagonal(desiredVelocity) && Time.time > lastPushableTouchTime + pushedObject.pushDelay)
            {
                if (!isGrabbing || (isGrabbing && !Mathf.Approximately(Vector2.Dot(desiredVelocity, grabDirection),0)))
                    MovePushable(desiredVelocity);
            }

            wasTouchingPushable = true;
        }

        private void MovePushable(Vector2 velocity)
        {
            if (Time.time > lastPushTime + pushedObject.pushInterval)
            {
                Vector2 delta = velocity.normalized * pushAmount;

                if (isGrabbing && Vector2.Dot(velocity, grabDirection) < 0)
                {
                    movementHandler.Translate(delta);
                    pushedObject.movementHandler.Translate(delta);
                }
                else
                {
                    pushedObject.movementHandler.Translate(delta);
                    movementHandler.Translate(delta);
                }

                lastPushTime = Time.time;
            }
        }

        private Pushable GetNearestPushable()
        {
            if (touchedPushables.Count == 0)
                return null;
            
            Pushable nearestPushable = touchedPushables[0];
            foreach (var pushable in touchedPushables)
                if (Vector2.Distance(gridTransform.QuantizedPosition, pushable.gridTransform.QuantizedPosition) <
                    Vector2.Distance(gridTransform.QuantizedPosition, nearestPushable.gridTransform.QuantizedPosition))
                    nearestPushable = pushable;

            return nearestPushable;
        }

        private int BoxCast(Vector2 origin, Vector2 boxSize, Vector2 direction, float distancePixels, List<RaycastHit2D> hits)
        {
            int layer = gameObject.layer;
            gameObject.layer = 0;   // Temporarily move this object to the default layer (or at least one not hit by the wall filter)
                
            int hitCount = Physics2D.BoxCast(
                origin,
                boxSize,
                0,
                direction,
                ContactFilters.Instance.wallFilter,
                hits,
                gridTransform.UnitsPerPixel * distancePixels
            );
                
            gameObject.layer = layer;
            return hitCount;
        }

        private void CheckCorners()
        {
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

            Vector2 leftOrigin = gridTransform.QuantizedPosition + GetBoxCenter(leftCornerPos);
            Vector2 centralOrigin = gridTransform.QuantizedPosition + GetBoxCenter(centralPos) - input.normalized * gridTransform.UnitsPerPixel;
            Vector2 rightOrigin = gridTransform.QuantizedPosition + GetBoxCenter(rightCornerPos);

            int leftHitCount = BoxCast(leftOrigin,
                cornerDetectionBoxSize - Vector2.one * (gridTransform.UnitsPerPixel * 2), input, 1, hits);
            int centralHitCount = BoxCast(centralOrigin,
                (input.x == 0 ? horizontalDetectionBoxSize : verticalDetectionBoxSize), input, 1, hits);
            int rightHitCount = BoxCast(rightOrigin,
                cornerDetectionBoxSize - Vector2.one * (gridTransform.UnitsPerPixel * 2), input, 1, hits);
            
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

