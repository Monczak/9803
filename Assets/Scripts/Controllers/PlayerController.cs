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

        private PlayerControls controls;

        public float speed; // Pixels per second

        private Vector2 input;

        private void Awake()
        {
            controls = new PlayerControls();
            gridTransform = GetComponent<GridTransform>();
            movementHandler = GetComponent<MovementHandler>();

            controls.Movement.Move.performed += OnMove;
            controls.Movement.Move.canceled += OnMove;
            
            controls.Enable();
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
            movementHandler.velocity = input * speed;
        }
    }
}

