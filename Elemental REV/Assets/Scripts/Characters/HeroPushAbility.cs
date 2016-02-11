using System.Collections;
using UnityEngine;

public class HeroPushAbility : MonoBehaviour
{

    private HeroInteract _heroInteract;
    private Animator _animationControl;
    private HeroMove _heroMove;

    private PushableObject _pushable;
    private Vector3 _destination;
    private bool _pushing = false;

    void Start()
    {
        _animationControl = GetComponent<Animator>();
        _heroInteract = GetComponent<HeroInteract>();
        _heroMove = GetComponent<HeroMove>();
    }


    void Update()
    {
        if (_heroInteract.CurrentInteractable && _heroInteract.CurrentInteractable.tag.Equals("Pushable"))
        {
            if (!_pushable)
            {
                Debug.Log("Found a motherfucking pushable");
                _pushable = (PushableObject) _heroInteract.CurrentInteractable;
                _pushable.transform.SetParent(transform);
                _destination = _pushable.transform.position;
                _animationControl.SetBool("isPushing", true);
            }
        }
        else if (!_pushing && _pushable)
        {
            Debug.Log("No longer pushing");
            _pushable.transform.SetParent(null);
            _pushable = null;
            _animationControl.SetBool("isPushing", false);
        }

    }

    void FixedUpdate()
    {
        if (_pushable)
        {
            if (transform.position != _destination)
            {
                _pushing = true;
                Debug.Log(_destination);
                _heroMove.MoveHeroTo(_destination, OnFinishedMoving);                
            }

        }
    }

    private void OnFinishedMoving()
    {
        _pushing = false;
        _heroMove.PlayerCanMoveHero = true;
        _pushable.transform.SetParent(null);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (_heroInteract && _heroInteract.CurrentInteractable)
            Gizmos.DrawWireCube(_heroInteract.CurrentInteractable.transform.position + (Vector3.up * .5f), Vector3.one * 1.25f);


        if (!Application.isPlaying)
            return;

        if (GridTools.Instance.PositionIsAccessible(GridTools.Instance.SnapVectorToGrid(transform.position), new []{gameObject}))
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawWireCube(GridTools.Instance.SnapVectorToGrid(transform.position), Vector3.one);


    }

}
