using System;
using System.Collections.Generic;
using NineEightOhThree.Math;
using NineEightOhThree.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NineEightOhThree.Controllers
{
    [RequireComponent(typeof(GridTransform)), RequireComponent(typeof(MovementHandler))]
    public class PlayerController : MonoBehaviour
    {
        private GridTransform gridTransform;
        private MovementHandler movementHandler;

        private new BoxCollider2D collider;

        private PlayerControls controls;

        public float speed; // Pixels per second
        public float sidePushZoneSize;
        private float SidePushZoneSize => sidePushZoneSize / gridTransform.pixelsPerUnit;

        private Vector2 input;
        private float relativeAngle;

        private List<RaycastHit2D> hits = new();    // Used in CheckCorners() as a discard
        
        private void Awake()
        {
            controls = new PlayerControls();
            gridTransform = GetComponent<GridTransform>();
            movementHandler = GetComponent<MovementHandler>();
            collider = GetComponent<BoxCollider2D>();

            controls.Movement.Move.performed += OnMove;
            controls.Movement.Move.canceled += OnMove;
            
            controls.Enable();
            
            movementHandler.CollisionEnter += MovementHandlerOnCollisionEnter;
            movementHandler.CollisionStay += MovementHandlerOnCollisionStay;
            movementHandler.CollisionExit += MovementHandlerOnCollisionExit;
        }

        private void MovementHandlerOnCollisionEnter(CollisionInfo info)
        {
            // Debug.Log($"Enter {info.GameObject.name} {info.Point}");
        }
        
        private void MovementHandlerOnCollisionStay(CollisionInfo info)
        {
            // Debug.Log($"Stay {info.GameObject.name} {gridTransform.UnitPosition - info.Point}");
        }
        
        private void MovementHandlerOnCollisionExit(CollisionInfo info)
        {
            // Debug.Log($"Exit {info.GameObject.name} {info.Point}");
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
            movementHandler.velocity = MathExtensions.RotateDegrees(input, relativeAngle) * speed;
        }

        private void CheckCorners()
        {
            Vector2 GetBoxCenter(Vector2 pos) =>
                new(pos.x * (collider.size.x - SidePushZoneSize) / 2,
                    pos.y * (collider.size.y - SidePushZoneSize) / 2);
        
            Vector2 cornerDetectionBoxSize = Vector2.one * SidePushZoneSize;
        
            Vector2 horizontalDetectionBoxSize = new(collider.size.x - 2 * SidePushZoneSize, SidePushZoneSize);
            Vector2 verticalDetectionBoxSize = new(SidePushZoneSize, collider.size.y - 2 * SidePushZoneSize);
            
            if (input.x * input.y != 0 || input == Vector2.zero) // If going in more than one direction (diagonal)
                return;

            Vector2 leftCornerPos = input.normalized + MathExtensions.RotateDegrees(input.normalized, 90);
            Vector2 centralPos = input.normalized;
            Vector2 rightCornerPos = input.normalized + MathExtensions.RotateDegrees(input.normalized, -90);

            Vector2 leftOrigin = (Vector2)transform.position + GetBoxCenter(leftCornerPos);
            Vector2 centralOrigin = (Vector2)transform.position + GetBoxCenter(centralPos) - input.normalized * gridTransform.UnitsPerPixel;
            Vector2 rightOrigin = (Vector2)transform.position + GetBoxCenter(rightCornerPos);

            int leftHitCount = Physics2D.BoxCast(
                leftOrigin,
                cornerDetectionBoxSize - Vector2.one * (gridTransform.UnitsPerPixel * 2),
                0,
                input,
                movementHandler.wallFilter,
                hits,
                gridTransform.UnitsPerPixel
            );
            int centralHitCount = Physics2D.BoxCast(
                centralOrigin,
                (input.x == 0 ? horizontalDetectionBoxSize : verticalDetectionBoxSize),
                0,
                input,
                movementHandler.wallFilter,
                hits,
                gridTransform.UnitsPerPixel
            );
            int rightHitCount = Physics2D.BoxCast(
                rightOrigin,
                cornerDetectionBoxSize - Vector2.one * (gridTransform.UnitsPerPixel * 2),
                0,
                input,
                movementHandler.wallFilter,
                hits,
                gridTransform.UnitsPerPixel
            );
            
            Debug.Log($"L {leftHitCount} C {centralHitCount} R {rightHitCount}");

            if (leftHitCount != 0 && centralHitCount == 0 && rightHitCount == 0)
                relativeAngle = -90;
            else if (leftHitCount == 0 && centralHitCount == 0 && rightHitCount != 0)
                relativeAngle = 90;
            else
                relativeAngle = 0;
        }
    }
}

