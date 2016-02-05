﻿using UnityEngine;
using System.Collections.Generic;
using InControl;

public class PlayerInput : MonoBehaviour
{

    private Transform _cam;
    private HeroMove _heroMovement;
    private HeroInteract _heroInteract;
    private Vector3 _moveInDirection;

    public enum interactState { begun, beingHeld, ended, released };


    void Start()
    {
        _cam = Camera.main.transform;
        _heroMovement = GetComponent<HeroMove>();
        _heroInteract = GetComponent<HeroInteract>();

    }

    void Update()
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        if (_heroMovement)
            _heroMovement.DoMovement(_moveInDirection);
    }

    private void HandleInput()
    {
        InputDevice device = InputManager.ActiveDevice;

        if (device)
        {
            HandleMovementInput(device);

            HandleInteractionInput(device);
        }
    }

    private void HandleMovementInput(InputDevice device)
    {
        //Reset direction each frame
        _moveInDirection = Vector3.zero;

        float horizontalAxis = device.LeftStickX;
        float verticalAxis = device.LeftStickY;

        //Set direction relative to camera (to world in case no camera detected)
        if (_cam != null)
            _moveInDirection = verticalAxis*Vector3.Scale(_cam.forward, new Vector3(1, 0, 1)).normalized +
                               horizontalAxis*_cam.right;
        else
            _moveInDirection = verticalAxis*Vector3.forward + horizontalAxis*Vector3.right;
        
    }

    private void HandleInteractionInput(InputDevice device)
    {
        if (_heroInteract)
        {
            var currentInteractState = GetCurrentInteractState(device);

            if (currentInteractState != interactState.released)
                _heroInteract.CheckForInteractables(currentInteractState);
        }
    }

    private interactState GetCurrentInteractState(InputDevice device)
    {
        if (device.Action1.WasReleased)
            return interactState.ended;
        if (device.Action1.WasPressed)
            return interactState.begun;
        if (device.Action1.IsPressed)
            return interactState.beingHeld;

        return interactState.released;
    }

}