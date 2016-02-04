using UnityEngine;
using System.Collections.Generic;
using InControl;


[RequireComponent(typeof(HeroMove))]
public class PlayerInput : MonoBehaviour
{

    private Transform cam;
    private HeroMove hero;
    private Vector3 moveInDirection;
    private bool jumpPressed;
    private bool interactPressed;

    void Start()
    {
        cam = Camera.main.transform;
        hero = GetComponent<HeroMove>();

    }

    void Update()
    {
        InputDevice device = InputManager.ActiveDevice;
        if (!device)
            return;

        moveInDirection = Vector3.zero;
        interactPressed = device.RightTrigger;

        //Get axis Input
        float horizontalAxis = device.LeftStickX;
        float verticalAxis = device.LeftStickY;

        // calculate move direction to pass to the hero
        if (cam != null)
        {
            // calculate camera relative direction to move:
            moveInDirection = verticalAxis * Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized + horizontalAxis * cam.right;

        }
        else
        {
            // we use world-relative directions in the case of no main camera
            moveInDirection = verticalAxis * Vector3.forward + horizontalAxis * Vector3.right;

        }
    }

    void FixedUpdate()
    {
        hero.DoMovement(moveInDirection, interactPressed);
    }
}