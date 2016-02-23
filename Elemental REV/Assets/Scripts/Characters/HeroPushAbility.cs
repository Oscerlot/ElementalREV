using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPushAbility : MonoBehaviour
{

    private HeroInteract _heroInteract;
    private Animator _animationControl;
    private HeroMove _heroMove;

    private PushableObject _pushable;
    static int pushingState = Animator.StringToHash("Base.Push");

    void Start()
    {
        _animationControl = GetComponent<Animator>();
        _heroInteract = GetComponent<HeroInteract>();
        _heroMove = GetComponent<HeroMove>();
    }


    void Update()
    {
        CheckForPushables();

        //Check if the animator is on the "Push" animation
        var currentBaseState = _animationControl.GetCurrentAnimatorStateInfo(0);
        if (_pushable && (currentBaseState.fullPathHash == pushingState))
        {
            _pushable.Push((_pushable.transform.position - transform.position).normalized, 1.5f);
        }
    }

    private void CheckForPushables()
    {
        if (_heroInteract.CurrentInteractable && _heroInteract.CurrentInteractable.tag.Equals("Pushable"))
        {
            if (!_pushable)
            {
                _pushable = (PushableObject) _heroInteract.CurrentInteractable;
                _animationControl.SetBool("isPushing", true);
            }
        }
        else if (_pushable && !_pushable.beingPushed)
        {
            _heroInteract.DetachHero();
            _pushable = null;
            _animationControl.SetBool("isPushing", false);

        }
    }

    void FixedUpdate()
    {
        if (_pushable)
        {
            var destination = FindNearestPositionToPlayer(_pushable.InteractPositions);
            _heroMove.MoveHeroTo(destination);
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
