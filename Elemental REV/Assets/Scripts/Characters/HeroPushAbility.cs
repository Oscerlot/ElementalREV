using System.Collections.Generic;
using UnityEngine;

public class HeroPushAbility : MonoBehaviour
{

    private HeroInteract _heroInteract;
    private Animator _animationControl;
    private HeroMove _heroMove;

    private PushableObject _pushable;
    static readonly int PushAnimState = Animator.StringToHash("Base.Push");

    void Start()
    {
        _animationControl = GetComponent<Animator>();
        _heroInteract = GetComponent<HeroInteract>();
        _heroMove = GetComponent<HeroMove>();
    }


    void FixedUpdate()
    {
        if (_pushable)
        {
            AttachHeroToPushable();
        }
    }

    void Update()
    {
        CheckForPushables();

        HandleObjectPushing();
    }

    private void HandleObjectPushing()
    {
        
        if (_pushable && !_pushable.beingPushed)
        {
            if (_heroInteract.InteractState == PlayerInput.InteractState.BeingHeld)
            {
                _animationControl.SetBool("isPushing", true);

                if (PushAnimationIsPlaying())       //Todo: Have the option to not require push animation to push?
                    _pushable.Push((_pushable.transform.position - transform.position).normalized, 1.5f);
            }
            else
            {
                _heroInteract.DetachHero();
                _animationControl.SetBool("isPushing", false);
            }
        }
    }

    private bool PushAnimationIsPlaying()
    {
        var currentBaseState = _animationControl.GetCurrentAnimatorStateInfo(0);
        return (currentBaseState.fullPathHash == PushAnimState);
    }

    private void AttachHeroToPushable()
    {
        var destination = FindNearestPositionToPlayer(_pushable.InteractPositions);
        _heroMove.RotateHeroTowards(_pushable.transform.position - transform.position, 1f);
        _heroMove.MovePosition(destination);
    }

    private void CheckForPushables()
    {
        if (_heroInteract.CurrentInteractable && _heroInteract.CurrentInteractable.tag.Equals("Pushable"))
        {
            if (!_pushable)
            {
                _pushable = (PushableObject) _heroInteract.CurrentInteractable;
            }
        }
        else if (_pushable && !_pushable.beingPushed)
        {
            _pushable = null;
        }
    }

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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (_heroInteract && _heroInteract.CurrentInteractable)
            Gizmos.DrawWireCube(_heroInteract.CurrentInteractable.transform.position + (Vector3.up * .5f), Vector3.one * 1.25f);


        if (!Application.isPlaying)
            return;

        if (GridTools.Instance.PositionIsAccessible(transform.position, new []{gameObject}))
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawWireCube(GridTools.Instance.SnapVectorToGrid(transform.position), Vector3.one);


    }

}
