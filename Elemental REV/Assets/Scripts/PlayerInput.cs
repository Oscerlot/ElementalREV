using UnityEngine;
using System.Collections.Generic;
using InControl;


[RequireComponent(typeof(HeroMove))]
public class PlayerInput : MonoBehaviour
{

    private Transform _cam;
    private HeroMove _heroMovement;
    private HeroInteract _heroInteract;
    private Vector3 _moveInDirection;

    public enum interactState { begun, beingHeld, released };
    public interactState currentInteractState = interactState.released;

    void Start()
    {
        _cam = Camera.main.transform;
        _heroMovement = GetComponent<HeroMove>();
        _heroInteract = GetComponent<HeroInteract>();

    }

    void Update()
    {
        InputDevice device = InputManager.ActiveDevice;
        if (!device)
            return;

        _moveInDirection = Vector3.zero;

        //Get axis Input
        float horizontalAxis = device.LeftStickX;
        float verticalAxis = device.LeftStickY;

        // calculate move direction to pass to the heroMovement
        if (_cam != null)
        {
            // calculate camera relative direction to move:
            _moveInDirection = verticalAxis * Vector3.Scale(_cam.forward, new Vector3(1, 0, 1)).normalized + horizontalAxis * _cam.right;

        }
        else
        {
            // we use world-relative directions in the case of no main camera
            _moveInDirection = verticalAxis * Vector3.forward + horizontalAxis * Vector3.right;

        }

        if (device.Action1.WasReleased)
            currentInteractState = interactState.released;
        else if (device.Action1.WasPressed)
            currentInteractState = interactState.begun;
        else if (device.Action1.IsPressed)
            currentInteractState = interactState.beingHeld;

        _heroInteract.CheckForInteractables(currentInteractState);

    }

    void FixedUpdate()
    {
        _heroMovement.DoMovement(_moveInDirection);
    }
}