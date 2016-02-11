using System;
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
            if (currentAttachState == AttachState.Attached)
                return _currentInteractable;
            else
                return null;
        }
    }

    private Interactable _currentInteractable;
    private float _detectionSphereRadius = .40f;
    private float _detectionSphereForwardDistance = .4f;
    private Collider _col;

    private HeroMove _heroMove;
    public enum AttachState { Attaching, Attached, Detached }
    public AttachState currentAttachState = AttachState.Detached;

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
            if (interactState == PlayerInput.InteractState.Ended || !interactablesDetected.Contains(_currentInteractable) 
                || !GridTools.Instance.PositionIsAccessible(FindNearestAttachPosition(_currentInteractable.InteractPositions), new []{gameObject, _currentInteractable.gameObject}))
            {
                DetachHero();
            }
        }
    }


    private Interactable GetNearestInteractable(List<Interactable> interactables)
    {
        if (interactables.Count <= 0)
            return null;

        Interactable nearestInteractable = interactables[0];

        foreach (var interactable in interactables)
        {
            var distanceToInteractable = Vector3.Distance(interactable.transform.position, DetectionSpherePosition);
            var distanceToNearestInteractable = Vector3.Distance(nearestInteractable.transform.position, DetectionSpherePosition);

            if (distanceToInteractable < distanceToNearestInteractable)
                nearestInteractable = interactable;

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



    private void AttachHeroTo(Interactable interactable)
    {
        var targetPosition = FindNearestAttachPosition(interactable.InteractPositions);
        var objectsToIgnore = new[] {gameObject, _currentInteractable.gameObject};

        if (GridTools.Instance.PositionIsAccessible(targetPosition, objectsToIgnore))
        {
            if (currentAttachState != AttachState.Attached)
            {
                currentAttachState = AttachState.Attaching;
                _heroMove.MoveHeroTo(targetPosition, OnAttachPositionReached);
            }
            _heroMove.RotateHeroTowards(interactable.InteractLookAtPosition - transform.position, _attachRotateSpeed);
        }

    }

    public void DetachHero()
    {
        if (currentAttachState != AttachState.Detached)
        {
            currentAttachState = AttachState.Detached;
            _heroMove.PlayerCanMoveHero = true;
            _currentInteractable = null;
        }
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

    private void OnAttachPositionReached()
    {
        currentAttachState = AttachState.Attached;        
    }


#region Gizmos

    void OnDrawGizmos()
    {
        DisplayDetectionRange();

        HighlightAttachTargetPos();
    }

    private void HighlightAttachTargetPos()
    {
        if (_currentInteractable && Application.isPlaying)
        {
            var targetPosition = FindNearestAttachPosition(_currentInteractable.InteractPositions);
            if (GridTools.Instance.PositionIsAccessible(targetPosition, new []{gameObject, _currentInteractable.gameObject}))
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;

            Gizmos.DrawWireCube(GridTools.Instance.SnapVectorToGrid(targetPosition), new Vector3(.8f, .8f, .8f));
        }
    }

    private void DisplayDetectionRange()
    {
        Gizmos.color = Color.white;
        if (_col)
            Gizmos.DrawWireSphere(DetectionSpherePosition, _detectionSphereRadius);
    }

#endregion

}




