using System.Collections;
using UnityEngine;

public class HeroPushAbility : MonoBehaviour
{

    public GameObject[] objectsToIgnore;//TODO: this is just for debug remove as soon as finished

    private HeroInteract _heroInteract;
    private Animator _animationControl;
    private HeroMove _heroMove;

    private PushableObject _pushable;
    private bool _pushing = false;

    void Start()
    {
        _animationControl = GetComponent<Animator>();
        _heroInteract = GetComponent<HeroInteract>();
        _heroMove = GetComponent<HeroMove>();
    }


    void Update()
    {
        //if (_heroInteract.CurrentInteractable && _heroInteract.CurrentInteractable.tag.Equals("Pushable"))
        //{
        //    _pushable = (PushableObject)_heroInteract.CurrentInteractable;
        //    _animationControl.SetBool("isPushing", true);
        //}
        //else if (!_pushing)
        //{
        //    _pushable = null;
        //    _animationControl.SetBool("isPushing", false);
        //}

    }

    void FixedUpdate()
    {
        if (_pushable)
        {
            //_heroMove.PlayerCanMoveHero = false;
            var targetPos = _pushable.transform.position;

            _pushable.transform.SetParent(transform);

            if (!_pushing)
            {
                MoveTo(targetPos);
            }

            Debug.DrawLine(transform.position, targetPos, Color.red);
        }
    }

    private void MoveTo(Vector3 interactPosition)
    {
        _heroMove.PlayerCanMoveHero = false;

        var directionToTargetPosition = (interactPosition - transform.position).normalized;

        if (Vector3.Distance(interactPosition, transform.position) > .1f)
        {
            _pushing = true;
            _heroMove.MoveHeroTowards(directionToTargetPosition);
        }
        else
        {
            Debug.Log("Stahp");
            _pushing = false;
            _heroMove.ResetVelocity();
            transform.position = interactPosition;
        }
    }

    private void OnFinishedMoving()
    {
        _pushing = false;
        _pushable.transform.SetParent(null);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (_heroInteract && _heroInteract.CurrentInteractable)
            Gizmos.DrawWireCube(_heroInteract.CurrentInteractable.transform.position + (Vector3.up * .5f), Vector3.one * 1.25f);


        if (GridTools.Instance.PositionIsAccessible(GridTools.Instance.SnapVectorToGrid(transform.position), objectsToIgnore))
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawWireCube(GridTools.Instance.SnapVectorToGrid(transform.position), Vector3.one);


    }

}
