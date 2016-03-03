using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Collider))]
public class HeroInteract : MonoBehaviour
{


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

    private HeroMove _heroMove;
    private HeroAwareness _heroAwareness;
    public enum AttachState { Attaching, Attached, Detached }

    [HideInInspector]
    public AttachState currentAttachState = AttachState.Detached;
    [HideInInspector]
    public PlayerInput.InteractState currentInteractState;

    private float _attachRotateSpeed = 10f;


    void Start()
    {
        _heroMove = GetComponent<HeroMove>();
        _heroAwareness = GetComponent<HeroAwareness>();
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
        currentInteractState = interactState;
        // Interactable Detected
        if (_heroAwareness.CurrentObjectInAwareness && _heroAwareness.CurrentObjectInAwareness.gameObject.layer.Equals(LayerMask.NameToLayer("Interactable")))
        {            
            if (interactState == PlayerInput.InteractState.BeingHeld || interactState == PlayerInput.InteractState.Began && _currentInteractable == null)
            {
                var detectedInteractable = _heroAwareness.CurrentObjectInAwareness.GetComponent<Interactable>();
                if (YAxisDistanceComparison(FindNearestPositionToPlayer(detectedInteractable.InteractPositions), transform.position, .1f))
                    _currentInteractable = detectedInteractable;
      
            }
        }

        if (_currentInteractable != null)
        {
            if ((interactState == PlayerInput.InteractState.Released && currentAttachState != AttachState.Attached) || !_heroAwareness.CurrentObjectInAwareness || !TargetGridPositionIsAccessible())
            {                                 
                DetachHero();
            }
        }
    }

    private bool YAxisDistanceComparison(Vector3 positionA, Vector3 positionB, float maxDistance)
    {
        return Mathf.Abs(Mathf.Abs(positionA.y) - Mathf.Abs(positionB.y)) < maxDistance;
    }


    private bool TargetGridPositionIsAccessible()
    {
        return GridTools.Instance.PositionIsAccessible(FindNearestPositionToPlayer(_currentInteractable.InteractPositions), new[] { gameObject, _currentInteractable.gameObject });
    }


    //TODO: this function is repeated on the push ability... do domething about it mate.
    private Vector3 FindNearestPositionToPlayer(List<Vector3> attachPositions)
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


    private void AttachHeroTo(Interactable interactable)
    {
        var targetPosition = FindNearestPositionToPlayer(interactable.InteractPositions);
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
        currentAttachState = AttachState.Detached;
        _heroMove.PlayerCanMoveHero = true;
        _currentInteractable = null;
 
    }

    private void OnAttachPositionReached()
    {
        currentAttachState = AttachState.Attached;        
    }


#region Gizmos

    void OnDrawGizmos()
    {
        HighlightAttachTargetPos();
    }

    private void HighlightAttachTargetPos()
    {
        if (_currentInteractable && Application.isPlaying)
        {
            var targetPosition = FindNearestPositionToPlayer(_currentInteractable.InteractPositions);
            if (GridTools.Instance.PositionIsAccessible(targetPosition, new []{gameObject, _currentInteractable.gameObject}))
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;

            Gizmos.DrawWireCube(GridTools.Instance.SnapVectorToGrid(targetPosition), new Vector3(.8f, .8f, .8f));
        }
    }


#endregion

}




