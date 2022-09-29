﻿using System;
using UnityEngine;

namespace NineEightOhThree.Objects
{
    [RequireComponent(typeof(GridTransform)), RequireComponent(typeof(MovementHandler))]
    public class Pushable : MonoBehaviour
    {
        private GridTransform gridTransform;
        private MovementHandler movementHandler;

        private void Awake()
        {
            gridTransform = GetComponent<GridTransform>();
            movementHandler = GetComponent<MovementHandler>();
            
            movementHandler.CollisionEnter += MovementHandlerOnCollisionEnter;
        }

        private void MovementHandlerOnCollisionEnter(CollisionInfo info)
        {
            Debug.Log($"{info.Origin.name} hit me!");
        }
    }
}