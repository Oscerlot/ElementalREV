using UnityEngine;
using System.Collections;
using System.Linq;
using System.Net;
using UnityEditor;

public class HeroPushAbility : MonoBehaviour
{

    private HeroMove _heroMove;
    private HeroInteract _heroInteract;
    private Vector3 _targetPosition;

    private enum AttachState { Attaching, Attached, NotAttached}
    private AttachState _currentAttachState = AttachState.NotAttached;

    private float _attachMoveSpeed = 2f;
    private float _attachRotateSpeed = 10f;


    void Start()
    {
        _heroMove = GetComponent<HeroMove>();
        _heroInteract = GetComponent<HeroInteract>();
    }

    void FixedUpdate()
    {
        if (_heroInteract.CurrentInteractable && _heroInteract.CurrentInteractable.tag.Equals("Pushable"))
        {
            AttachTo(_heroInteract.CurrentInteractable);
        }
        else
        {
            Detach();
        }
        Debug.Log(_currentAttachState.ToString());
    }

    private void AttachTo(GameObject pushable)
    {
        Vector3 pushablePos = pushable.transform.position;
        pushablePos.y = transform.position.y;
        _targetPosition = pushablePos + (GetNearestAxis((transform.position - pushable.transform.position).normalized) * 1.15f);

        if (PositionIsAccessible(_targetPosition))
        {
            AttachToInteractPosition(_targetPosition, pushablePos);
        }
        else
        {
            Detach();
        }

    }

    private void Detach()
    {
        _currentAttachState = AttachState.NotAttached;
        _heroMove.PlayerCanMoveHero = true;
    }

    private void AttachToInteractPosition(Vector3 interactPosition, Vector3 lookAtPosition)
    {        
        _heroMove.PlayerCanMoveHero = false;

        var directionToPushable = lookAtPosition - transform.position;
        var directionToTargetPosition = (interactPosition - transform.position).normalized*_attachMoveSpeed;
        _heroMove.RotateHeroTowards(directionToPushable, _attachRotateSpeed);

        if (Vector3.Distance(interactPosition, transform.position) > .02f)
        {
            _currentAttachState = AttachState.Attaching;
            _heroMove.MoveHeroTowards(directionToTargetPosition);
        }
        else if (_currentAttachState != AttachState.Attached)
        {
            _currentAttachState = AttachState.Attached;
            transform.position = interactPosition;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }


    private bool PositionIsAccessible(Vector3 position)
    {
        Vector3 newPos = RoundPositionToNearestHalf(position);
        Collider [] collidingObjects = Physics.OverlapBox(newPos + (Vector3.up*.5f), new Vector3(.4f, .4f, .4f), Quaternion.identity);

        return !(collidingObjects.Length > 0 && collidingObjects.Any(collidingObject => !collidingObject.gameObject.Equals(gameObject)));

    }

    private static Vector3 RoundPositionToNearestHalf(Vector3 position)
    {
        //Increments of .5 (e.g. .7 would get rounded to .5). 
        return new Vector3(Mathf.Round(position.x * 2f) * .5f, Mathf.Round(position.y * 2f) * .5f, Mathf.Round(position.z * 2f) *.5f);
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

    void OnDrawGizmos()
    {

        if (_heroInteract && _heroInteract.CurrentInteractable)
        {
            if (PositionIsAccessible(_targetPosition))
                Gizmos.color = Color.green;
            else            
                Gizmos.color = Color.red;
            
            Gizmos.DrawWireCube(RoundPositionToNearestHalf(_targetPosition) + (Vector3.up * .5f), new Vector3(.8f, .8f, .8f));
            
        }
       

    }

}
