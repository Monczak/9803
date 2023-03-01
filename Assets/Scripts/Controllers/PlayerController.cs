using System;
using System.Collections.Generic;
using System.Linq;
using NineEightOhThree.Managers;
using NineEightOhThree.Math;
using NineEightOhThree.Objects;
using NineEightOhThree.Objects.Interactables;
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
        public float interactionDistance;

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

        private Interactable currentInteractable;
        
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
            controls.Movement.Interact.performed += OnInteracted;
            controls.Movement.Interact.canceled += OnStoppedInteracting;
            
            controls.Enable();
            
            movementHandler.CollisionEnter += MovementHandlerOnCollisionEnter;
            movementHandler.CollisionStay += MovementHandlerOnCollisionStay;
            movementHandler.CollisionExit += MovementHandlerOnCollisionExit;
        }

        private void OnStoppedInteracting(InputAction.CallbackContext obj)
        {
            if (currentInteractable is not null)
            {
                currentInteractable.StopInteracting(collider);
                currentInteractable = null;
            }
        }

        private void OnInteracted(InputAction.CallbackContext obj)
        {
            int hitCount = BoxCast(gridTransform.QuantizedPosition,
                movementHandler.Collider.size - Vector2.one * gridTransform.UnitsPerPixel, direction,
                interactionDistance, hits, ContactFilters.InteractableFilter);

            if (hitCount > 0)
            {
                // TODO: Nearest instead of first
                RaycastHit2D hit = hits[0];

                if (ColliderCache.Instance.TryGet(hit.collider, out Interactable interactable))
                {
                    currentInteractable = interactable;
                    interactable.Interact(collider);
                }
            }
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
                Ungrab();
            }
        }

        private void Ungrab()
        {
            grabbedObject = null;
            grabbedObjectCollider = null;
            isGrabbing = false;
        }

        private void Grab()
        {
            int hitCount = BoxCast(gridTransform.QuantizedPosition,
                movementHandler.Collider.size - Vector2.one * gridTransform.UnitsPerPixel, direction, pullDistance,
                hits, ContactFilters.WallFilter);
            if (hitCount > 0)
            {
                isGrabbing = true;
                
                RaycastHit2D nearestHit = hits[0];
                foreach (RaycastHit2D hit in hits)
                {
                    if ((gridTransform.QuantizedPosition - hit.point).sqrMagnitude <
                        (gridTransform.QuantizedPosition - nearestHit.point).sqrMagnitude)
                        nearestHit = hit;
                }
                grabbedObjectCollider = nearestHit.collider;
                ColliderCache.Instance.TryGet(grabbedObjectCollider, out grabbedObject);

                grabDirection = -nearestHit.normal;
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
            void Move(MovementHandler first, MovementHandler second, Vector2 delta)
            {
                Vector2 pos = first.gridTransform.QuantizedPosition;
                first.Translate(delta);
                bool couldMove = second.Translate(delta);
                if (!couldMove) first.gridTransform.QuantizedPosition = pos;
            }
            
            if (Time.time > lastPushTime + pushedObject.pushInterval)
            {
                Vector2 delta = velocity.normalized * pushAmount;

                if (isGrabbing && Vector2.Dot(velocity, grabDirection) < 0)
                {
                    Move(movementHandler, pushedObject.movementHandler, delta);
                }
                else
                {
                    Move(pushedObject.movementHandler, movementHandler, delta);
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

        private int BoxCast(Vector2 origin, Vector2 boxSize, Vector2 direction, float distancePixels, List<RaycastHit2D> hits, ContactFilter2D filter)
        {
            int layer = gameObject.layer;
            gameObject.layer = 0;   // Temporarily move this object to the default layer (or at least one not hit by the wall filter)
                
            int hitCount = Physics2D.BoxCast(
                origin,
                boxSize,
                0,
                direction,
                filter,
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
                cornerDetectionBoxSize - Vector2.one * (gridTransform.UnitsPerPixel * 2), input, 1, hits,
                ContactFilters.WallFilter);
            int centralHitCount = BoxCast(centralOrigin,
                input.x == 0 ? horizontalDetectionBoxSize : verticalDetectionBoxSize, input, 1, hits,
                ContactFilters.WallFilter);
            int rightHitCount = BoxCast(rightOrigin,
                cornerDetectionBoxSize - Vector2.one * (gridTransform.UnitsPerPixel * 2), input, 1, hits,
                ContactFilters.WallFilter);
            
            // Logger.Log($"L {leftHitCount} C {centralHitCount} R {rightHitCount}");

            if (leftHitCount != 0 && centralHitCount == 0 && rightHitCount == 0)
                relativeAngle = -90;
            else if (leftHitCount == 0 && centralHitCount == 0 && rightHitCount != 0)
                relativeAngle = 90;
            else
                relativeAngle = 0;
        }

        public void EnableControls()
        {
            controls.Enable();
        }

        public void DisableControls()
        {
            controls.Disable();
        }
    }
}

