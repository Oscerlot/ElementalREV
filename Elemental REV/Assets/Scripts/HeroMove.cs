using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;

public class HeroMove : MonoBehaviour {

    public enum characters { None, Earth, Water, Fire, Air };
    public characters character;

    public LayerMask whatIsGround;
    public LayerMask noJumpingLayers;   // Layers that are detected as ground yet we dont want the player to jump on (i.e. Water and Lava in the case of the fire and water elementals).

    // public modifier variables
    public float moveForceModifier = 3f;
    public float jumpHeight = 5f;
    public float maxVelocityChange = 10;
    public float gravity = 10.0f;
    public float rotateSpeed = 5;
    public bool enableFallDamage;


    private Animator animationControl;
    private Rigidbody rgBody;
    private CapsuleCollider capsCollider;           //To use the centre of the collider to determine groundcheckOffset instead of the objects's centre

    private bool canUseAbility = true;              //Ed changes: To stop the character from using their special ability when the camera is not looking at them
    private bool canMove = true;
    private const float groundedRadius = 0.2f;
    private float groundedOffsetDistance;
    private Vector3 movingGroundVelocity = Vector3.zero;
    private bool isGrounded;
    private bool manualGravity;                     //use manual gravity

    
    public bool IsGrounded { get { return isGrounded; } private set { isGrounded = value; } }

    public bool ManualGravity { get { return manualGravity; } set { manualGravity = value; } }
    public bool CanMove { get { return canMove; } set { canMove = value; } }
    public bool CanUseAbility { get { return canUseAbility; } set { canUseAbility = value; } }
    public bool IsMoving { get { return rgBody.velocity != Vector3.zero; } }

    void Start()
    {
        // initializing variables
        capsCollider = GetComponent<CapsuleCollider>();
        groundedOffsetDistance = -capsCollider.bounds.extents.y + (groundedRadius * 0.7f);
        animationControl = GetComponent<Animator>();
        rgBody = GetComponent<Rigidbody>();

    }

    void LateUpdate()
    {
       
        if (!canMove && isGrounded && (rgBody.velocity != Vector3.zero))
        {
            Vector3 vel = Vector3.Lerp(rgBody.velocity, Vector3.zero, .4f);
            rgBody.velocity = vel;
        }
    }


    public void DoMovement(Vector3 direction, bool interactInput)
    {
        animationControl.SetBool("isWalking", (direction != Vector3.zero) && canMove);
        animationControl.SetBool("Grounded", isGrounded);

        if (canMove)// movement handling
        {
            direction *= moveForceModifier;

            Vector3 velocity = rgBody.velocity - movingGroundVelocity;
            Vector3 velocityChange = (direction - velocity);

            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            rgBody.AddForce(velocityChange, ForceMode.VelocityChange);

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotateSpeed);
            }

        }

        if (manualGravity)
        {
            // We apply gravity manually for more tuning control
            rgBody.AddForce(new Vector3(0, -gravity * rgBody.mass, 0));
        }

        //Reset the moveDamp modifiers
        movingGroundVelocity = Vector3.zero;
    }

    private bool GroundCheck()
    {
        if (capsCollider.enabled == true)
        {
            Vector3 groundCheckPos = transform.TransformPoint(capsCollider.center);
            groundCheckPos.y += groundedOffsetDistance;

            Collider[] colliders = Physics.OverlapSphere(groundCheckPos, groundedRadius, whatIsGround);
            for (int i = 0; i < colliders.Length; ++i)
            {
                if ((colliders[i].gameObject != gameObject) && !colliders[i].gameObject.name.Equals("_SweepTestObj_"))
                {
                    //Check for moving ground
                    if ((colliders[i].gameObject.layer == LayerMask.NameToLayer("MovingGround")) && (movingGroundVelocity == Vector3.zero))
                    {
                        movingGroundVelocity = colliders[i].gameObject.GetComponent<Rigidbody>().velocity;
                    }

                    Vector3 currentGroundBlockTopPos = colliders[i].transform.position;
                    currentGroundBlockTopPos.y += colliders[i].bounds.center.y + colliders[i].bounds.extents.y;
                    return true;
                }
            }
        }
        //Check for moving ground
        if (movingGroundVelocity != Vector3.zero)
            movingGroundVelocity = Vector3.zero;


        return false;
    }

    public Vector3 GetNearestAxis(Vector3 direction)
    {
        if (Mathf.Abs(direction.z) > Mathf.Abs(direction.x))
        {
            if (direction.z > 0)
                return Vector3.forward;
            else
                return -Vector3.forward;
        }
        else
        {
            if (direction.x > 0)
                return Vector3.right;
            else
                return -Vector3.right;
        }
    }

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards moveDamp 
        // for the character to reach at the apex.
        // By using the Magic of Meth (yes meth)
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    public bool SweepTestCheck(GameObject obj, Vector3 direction, float distance, out RaycastHit hitInfo)// returns true if something other than this object is hit
    {
        if (obj.transform.FindChild("_SweepTestObj_") == null)
            throw new System.Exception("A Sweep Test Object needs to be attached to gameobject '" + obj.name + "' !!");

        if (!obj.transform.FindChild("_SweepTestObj_").GetComponent<Rigidbody>().SweepTest(direction, out hitInfo, distance, QueryTriggerInteraction.Ignore))
            return false;
        else
        {
            if (hitInfo.collider.gameObject == obj)
                return false;
            else
                return true;
        }
    }
}

