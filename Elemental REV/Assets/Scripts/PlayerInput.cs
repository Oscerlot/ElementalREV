﻿using UnityEngine;
using System.Collections.Generic;
using InControl;

public class PlayerInput : MonoBehaviour
{

    private Transform _cam;
    private HeroMove _heroMovement;
    private HeroInteract _heroInteract;
    private Vector3 _moveInDirection;

    public enum InteractState { Begun, BeingHeld, Ended, Released };


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
            _heroMovement.ReceivePlayerMovementInput(_moveInDirection);
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

            if (currentInteractState != InteractState.Released)
                _heroInteract.CheckForInteractables(currentInteractState);
        }
    }

    private InteractState GetCurrentInteractState(InputDevice device)
    {
        if (device.Action1.WasReleased)
            return InteractState.Ended;
        if (device.Action1.WasPressed)
            return InteractState.Begun;
        if (device.Action1.IsPressed)
            return InteractState.BeingHeld;

        return InteractState.Released;
    }

}