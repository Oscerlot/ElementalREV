using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Collider))]
public class HeroInteract : MonoBehaviour
{

    public LayerMask whatIsInteractable;

    public Interactable CurrentInteractable {
        get
        {
            if (_interactableIsReady)
                return _currentInteractable;
            else
                return null;
        }
    }


    private Interactable _currentInteractable;
    private float _detectionSphereRadius = .40f;
    private float _detectionSphereForwardDistance = .4f;
    private Collider _col;
    private bool _interactableIsReady = false;

    private HeroMove _heroMove;
    private enum AttachState { Attaching, Attached, NotAttached }
    private AttachState _currentAttachState = AttachState.NotAttached;

    private float _attachRotateSpeed = 10f;

    private Vector3 DetectionSpherePosition {
        get { return _col.bounds.center + transform.forward*_detectionSphereForwardDistance; }
    }


    void Start()
    {
        _col = GetComponent<Collider>();
        _heroMove = GetComponent<HeroMove>();
    }

    void FixedUpdate()
    {
        if (_currentInteractable && _currentInteractable.InteractPositions.Count > 0)
            AttachHeroTo(_currentInteractable);
    }


    public void ReceivePlayerInteractInput(PlayerInput.InteractState interactState)
    {
        CheckForInteractables(interactState);
    }

    private void CheckForInteractables(PlayerInput.InteractState interactState)
    {
        Collider[] colliders = Physics.OverlapSphere(DetectionSpherePosition, _detectionSphereRadius);
        List<Interactable> interactablesDetected = FilterInteractables(colliders);

        // Interactable Detected
        if (interactablesDetected.Count > 0)
        {
            if (interactState == PlayerInput.InteractState.Begun && _currentInteractable == null)
            {
                _currentInteractable = GetNearestInteractable(interactablesDetected);
            }
        }

        if (_currentInteractable != null)
        {
            if (interactState == PlayerInput.InteractState.Ended || !interactablesDetected.Contains(_currentInteractable))
            {
                DetachHero();
                _currentInteractable = null;
            }
        }
    }


    private Interactable GetNearestInteractable(List<Interactable> interactables)
    {
        Interactable nearestInteractable = interactables[0];
        foreach (var interactable in interactables)
        {
            var distanceToInteractable = Vector3.Distance(interactable.transform.position, DetectionSpherePosition);
            var distanceToNearestInteractable = Vector3.Distance(nearestInteractable.transform.position, DetectionSpherePosition);

            if (distanceToInteractable < distanceToNearestInteractable)
            {
                nearestInteractable = interactable;
            }
        }
        return nearestInteractable;
    }


    private List<Interactable> FilterInteractables(Collider[] colliders)
    {
        List<Interactable> interactables = new List<Interactable>();

        foreach (Collider col in colliders)
        {
            if (whatIsInteractable == (whatIsInteractable | (1 << col.gameObject.layer)))
            {
                interactables.Add(col.gameObject.GetComponent<Interactable>());
            }
        }

        return interactables;
    }



    private void AttachHeroTo(Interactable attachable)
    {
        var targetPosition = FindNearestAttachPosition(attachable.InteractPositions);
        if (PositionIsAccessible(targetPosition))
            MoveToInteractPosition(targetPosition, attachable.InteractLookAtPosition);

    }

    private Vector3 FindNearestAttachPosition(List<Vector3> attachPositions)
    {
        
        var nearestAttachPosition = attachPositions[0];
        foreach (var validAttachPosition in attachPositions)
        {
            var distanceToCurrent = Vector3.Distance(validAttachPosition, transform.position);
            var distanceToNearest = Vector3.Distance(nearestAttachPosition, transform.position);

            if (distanceToCurrent < distanceToNearest)
                nearestAttachPosition = validAttachPosition;
        }

        return nearestAttachPosition;
    }

    private void DetachHero()
    {
        _currentAttachState = AttachState.NotAttached;
        _heroMove.PlayerCanMoveHero = true;
    }

    private void MoveToInteractPosition(Vector3 interactPosition, Vector3 lookAtPosition)
    {
        _heroMove.PlayerCanMoveHero = false;

        var directionToPushable = (lookAtPosition - transform.position);
        var directionToTargetPosition = (interactPosition - transform.position).normalized;
        _heroMove.RotateHeroTowards(directionToPushable, _attachRotateSpeed);

        //Have we reached the target position and are we facing the attachable?
        if (Vector3.Distance(interactPosition, transform.position) > .02f)
        {
            _currentAttachState = AttachState.Attaching;
            _heroMove.MoveHeroTowards(directionToTargetPosition);
        }
        else if (_currentAttachState != AttachState.Attached)
        {
            _currentAttachState = AttachState.Attached;
            _heroMove.ResetVelocity();
            Debug.Log("Attached!");
            transform.position = interactPosition;

        }
    }


    private bool PositionIsAccessible(Vector3 position)
    {        
        Vector3 newPos = position + (Vector3.up * .5f);
        Collider[] collidingObjects = Physics.OverlapBox(newPos, new Vector3(.4f, .4f, .4f), Quaternion.identity);

        return !(collidingObjects.Length > 0 && collidingObjects.Any(collidingObject => !collidingObject.gameObject.Equals(gameObject)));

    }

#region Gizmos

    void OnDrawGizmos()
    {
        HighlightCurrentInteractable();

        DisplayDetectionRange();

        HighlightAttachTargetPos();

    }

    private void HighlightAttachTargetPos()
    {
        if (_currentInteractable)
        {
            var targetPosition = FindNearestAttachPosition(_currentInteractable.InteractPositions);
            if (PositionIsAccessible(targetPosition))
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;

            Gizmos.DrawWireCube(targetPosition + (Vector3.up*.5f),
                new Vector3(.8f, .8f, .8f));
        }
    }

    private void DisplayDetectionRange()
    {
        Gizmos.color = Color.white;
        if (_col)
            Gizmos.DrawWireSphere(DetectionSpherePosition, _detectionSphereRadius);
    }

    private void HighlightCurrentInteractable()
    {
        Gizmos.color = Color.red;
        if (_currentInteractable)
            Gizmos.DrawWireCube(_currentInteractable.transform.position + (Vector3.up*.5f), Vector3.one*1.3f);
    }

#endregion

}




