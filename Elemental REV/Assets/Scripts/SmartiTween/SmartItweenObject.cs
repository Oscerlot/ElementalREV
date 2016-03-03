using UnityEngine;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// When activated the gameObject will move towards a destination. The gameObject will stop moving once the destination is reached or it is deactivated.
/// For use on single destination interactables.
/// </summary>
public class SmartItweenObject : TriggerObject
{

    //Public Variables
    //--------------------------------------------------------------------
    public TriggerType triggerType = TriggerType.activatedOnce;
    public ActionType actionType = ActionType.movement;
    public bool activeFromStart = false;

    [Range(0, 50)]
    public float speed = 3;
    public Vector3 destinationOffset;
    public EaseType toDestinationEaseType = EaseType.linear;

    [HideInInspector]
    public bool resetToOriginalPos = false;
    [HideInInspector]
    public float resetSpeed = 3;
    [HideInInspector]
    public Vector3 originalPosOffset;
    [HideInInspector]
    public EaseType toOriginEaseType = EaseType.linear;
    [HideInInspector]
    public bool callEventOnFinish = false;
    [HideInInspector]
    public UnityEvent OnPlayOnceFinished;


    public enum EaseType
    {
        easeInQuad,
        easeOutQuad,
        easeInOutQuad,
        easeInCubic,
        easeOutCubic,
        easeInOutCubic,
        easeInQuart,
        easeOutQuart,
        easeInOutQuart,
        easeInQuint,
        easeOutQuint,
        easeInOutQuint,
        easeInSine,
        easeOutSine,
        easeInOutSine,
        easeInExpo,
        easeOutExpo,
        easeInOutExpo,
        easeInCirc,
        easeOutCirc,
        easeInOutCirc,
        linear,
        spring,
        easeInBounce,
        easeOutBounce,
        easeInOutBounce,
        easeInBack,
        easeOutBack,
        easeInOutBack,
        easeInElastic,
        easeOutElastic,
        easeInOutElastic,
    }
    public enum ActionType
    {
        movement,
        rotation
    }
    public enum TriggerType
    {
        activatedOnce,
        pingPong
    }
    //--------------------------------------------------------------------

    //Private Variables
    //--------------------------------------------------------------------              
    private float elapsedTime;
    private Vector3 originalPos;
    private Vector3 destination;
    private Vector3 currentDestination;
    //--------------------------------------------------------------------


    //EventCalls
    //--------------------------------------------------------------------
    void Start()
    {
        switch (actionType)
        {
            case ActionType.movement:
                originalPos = transform.position + originalPosOffset;
                destination = transform.position + destinationOffset;
                break;

            case ActionType.rotation:
                originalPos = transform.rotation.eulerAngles;
                destination = (transform.rotation * Quaternion.Euler(destinationOffset)).eulerAngles;
                break;

        }
        if (activeFromStart == true)
        {
            activate();
        }
    }
    //--------------------------------------------------------------------

    //Private Functions
    //--------------------------------------------------------------------
    private void moveToDestination(Vector3 destination, float speed, string easeType)
    {
        currentDestination = destination;
        switch (actionType)
        {
            case ActionType.movement:
                if (triggerType == TriggerType.activatedOnce)
                {
                    iTween.MoveTo(gameObject, new Hashtable { { "position", destination }, { "speed", speed }, { "easeType", easeType } });
                    if (OnPlayOnceFinished != null)
                    {
                        OnPlayOnceFinished.Invoke();
                    }

                }
                else
                    iTween.MoveTo(gameObject, new Hashtable { { "position", destination }, { "speed", speed }, { "easeType", easeType }, { "onComplete", "switchDestination" } });
                break;

            case ActionType.rotation:
                if (triggerType == TriggerType.activatedOnce)
                {
                    iTween.RotateTo(gameObject, new Hashtable { { "rotation", destination }, { "speed", speed * 10 }, { "easeType", easeType } });
                    if (OnPlayOnceFinished != null)
                    {
                        OnPlayOnceFinished.Invoke();
                    }
                }
                else
                    iTween.RotateTo(gameObject, new Hashtable { { "rotation", destination }, { "speed", speed * 10 }, { "easeType", easeType }, { "onComplete", "switchDestination" } });
                break;

        }

    }

    private void switchDestination()
    {

        if (currentDestination == destination)
            moveToDestination(originalPos, speed, toDestinationEaseType.ToString());

        else
            moveToDestination(destination, speed, toDestinationEaseType.ToString());

    }
    //--------------------------------------------------------------------

    //Public Functions
    //--------------------------------------------------------------------
    public override void activate()
    {
        base.activate();

        Vector3 destinationBeforeDeactivation = currentDestination == Vector3.zero ? destination : currentDestination;

        if (transform.position != destination)
            moveToDestination(triggerType == TriggerType.pingPong ? destinationBeforeDeactivation : destination, speed, toDestinationEaseType.ToString());
    }

    public override void deActivate()
    {
        base.deActivate();
        Debug.Log("Deactivate");
        iTween.Stop(gameObject);
        if (resetToOriginalPos && triggerType != TriggerType.pingPong)
        {
            moveToDestination(originalPos, resetSpeed, toOriginEaseType.ToString());
        }
    }

    //--------------------------------------------------------------------

    //Gizmos and Editor 
    //--------------------------------------------------------------------

    //Show the destination using gizmos
    void OnDrawGizmosSelected()
    {
        switch (actionType)
        {
            case ActionType.movement:
                Vector3 currentDestination = destination == Vector3.zero ? transform.position + destinationOffset : destination;
                drawMoveToGizmos(currentDestination, Color.green);

                if (resetToOriginalPos)
                {
                    Vector3 returnDestination = originalPos == Vector3.zero ? transform.position + originalPosOffset : originalPos;
                    drawMoveToGizmos(returnDestination, Color.red);
                }
                break;

            case ActionType.rotation:
                Vector3 currentRotation = destination == Vector3.zero ? (transform.rotation * Quaternion.Euler(destinationOffset)).eulerAngles : destination;
                drawRotateToGizmos(currentRotation, Color.green);

                if (resetToOriginalPos)
                {
                    Vector3 resetRotation = originalPos == Vector3.zero ? (transform.rotation * Quaternion.Euler(originalPosOffset)).eulerAngles : originalPos;
                    drawRotateToGizmos(resetRotation, Color.red);
                }
                break;

        }
    }

    private void drawMoveToGizmos(Vector3 destination, Color color)
    {
        if (GetComponent<MeshFilter>())
        {
            Gizmos.color = color;
            Gizmos.DrawWireMesh(GetComponent<MeshFilter>().sharedMesh, -1, destination, transform.rotation, transform.lossyScale);
            Gizmos.DrawLine(transform.position, destination);
        }
    }

    private void drawRotateToGizmos(Vector3 rotationTarget, Color color)
    {
        if (GetComponent<MeshFilter>())
        {
            Gizmos.color = color;
            Gizmos.DrawWireMesh(GetComponent<MeshFilter>().sharedMesh, -1, transform.position, Quaternion.Euler(rotationTarget), transform.lossyScale);
        }
    }

    //--------------------------------------------------------------------
}