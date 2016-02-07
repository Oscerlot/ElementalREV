using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;

[RequireComponent(typeof(PlayerInput))]
public class HeroMove : MonoBehaviour {

    public LayerMask whatIsGround;

    // public modifier variables
    public float moveForceModifier = 3f;
    public float maxVelocityChange = 10;
    public float gravity = 10.0f;
    public float rotateSpeed = 5;

    public bool PlayerCanMoveHero {
        get { return _playerCanMoveHero; }
        set { _playerCanMoveHero = value; }
    }

    private Animator _animationControl;
    private Rigidbody _rgBody;
    private CapsuleCollider _capsCollider;           //To use the centre of the collider to determine groundcheckOffset instead of the objects's centre

    private bool _playerCanMoveHero = true;
    private const float GroundedRadius = 0.2f;
    private float _groundedOffsetDistance;
    private Vector3 _movingGroundVelocity = Vector3.zero;
    private bool _isGrounded;
    private const string MOVING_GROUND_LAYER_NAME = "MovingGround";

    private Vector3 _groundDetectPos;

    void Start()
    {
        // initializing variables
        _capsCollider = GetComponent<CapsuleCollider>();
        _groundedOffsetDistance = -_capsCollider.bounds.extents.y;
        _animationControl = GetComponent<Animator>();
        _rgBody = GetComponent<Rigidbody>();

    }


    void LateUpdate()
    {
       //If cant move and is grounded remove momentum
        if (!_playerCanMoveHero && _isGrounded && (_rgBody.velocity != Vector3.zero))
        {
            Vector3 vel = Vector3.Lerp(_rgBody.velocity, Vector3.zero, .4f);
            _rgBody.velocity = vel;
        }
    }


    public void DoMovement(Vector3 direction)
    {                
        GroundCheck();
        
        _animationControl.SetBool("Grounded", _isGrounded);

        if (_playerCanMoveHero)
        {
            direction *= moveForceModifier;

            MoveHeroTowards(direction);

            RotateHeroTowards(direction, rotateSpeed);

        }

        ApplyGravity();
        _animationControl.SetBool("isWalking", (direction.magnitude > .2f));
        _movingGroundVelocity = Vector3.zero;
    }

    public void MoveHeroTowards(Vector3 direction)
    {
        _animationControl.SetBool("isWalking", (direction.magnitude > .2f));

        Vector3 velocity = _rgBody.velocity - _movingGroundVelocity;
        Vector3 velocityChange = (direction - velocity);

        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;

        _rgBody.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    public void RotateHeroTowards(Vector3 direction, float rotationSpeed)
    {
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                Time.deltaTime*rotationSpeed);
        }
    }

    private void ApplyGravity()
    {
        _rgBody.AddForce(new Vector3(0, -gravity*_rgBody.mass, 0));
    }

    private void GroundCheck()
    {
        if (_capsCollider.enabled == true)
        {
            Vector3 groundCheckPos = transform.TransformPoint(_capsCollider.center);
            groundCheckPos.y += _groundedOffsetDistance;

            Collider[] colliders = Physics.OverlapSphere(groundCheckPos, GroundedRadius, whatIsGround);
            foreach (Collider collidingObject in colliders)
            {
                if (collidingObject.gameObject != gameObject)
                {
                    HandleMovingGround(collidingObject.gameObject);

                    _isGrounded = true;
                }
            }
        }
        ////Check for moving ground
        //if (_movingGroundVelocity != Vector3.zero)
        //    _movingGroundVelocity = Vector3.zero;

        _isGrounded = false;
    }

    private void HandleMovingGround(GameObject movingGround)
    {
        if ((movingGround.layer == LayerMask.NameToLayer(MOVING_GROUND_LAYER_NAME)) &&
            (_movingGroundVelocity == Vector3.zero))
        {
            _movingGroundVelocity = movingGround.gameObject.GetComponent<Rigidbody>().velocity;
        }
    }

}

