using UnityEngine;
using System.Collections;
using System.Linq;

public class HeroPushAbility : MonoBehaviour
{

    private HeroMove _heroMove;
    private HeroInteract _heroInteract;
    private Vector3? _targetPosition;

    private CapsuleCollider col;

    void Start()
    {
        _heroMove = GetComponent<HeroMove>();
        _heroInteract = GetComponent<HeroInteract>();
        col = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        if (_heroInteract.CurrentInteractable && _heroInteract.CurrentInteractable.tag.Equals("Pushable"))
        {
            AttachTo(_heroInteract.CurrentInteractable);
        }
        else
        {
            Detach();
        }
    }

    private void Detach()
    {
        _heroMove.CanMove = true;
        _targetPosition = null;
    }

    private void AttachTo(GameObject pushable)
    {
        Vector3 pushablePos = pushable.transform.position;
        pushablePos.y = transform.position.y;
        _targetPosition = pushablePos + (GetNearestAxis((transform.position - pushable.transform.position).normalized) * 1.15f);

        if (PositionIsAccessible(_targetPosition.Value))
        {
            _heroMove.CanMove = false;
            transform.LookAt(pushable.transform);

            if (Vector3.Distance(transform.position, _targetPosition.Value) > .1f)
            {
                Rigidbody rgBody = GetComponent<Rigidbody>();
                Vector3 direction = _targetPosition.Value - transform.position;
                rgBody.velocity = direction.normalized*2f;
            }
        }
        else
        {
            Detach();
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

        if (_targetPosition != null)
        {
            if (PositionIsAccessible(_targetPosition.Value))
                Gizmos.color = Color.green;
            else            
                Gizmos.color = Color.red;
            
            Gizmos.DrawWireCube(RoundPositionToNearestHalf(_targetPosition.Value) + (Vector3.up * .5f), new Vector3(.8f, .8f, .8f));
            
        }
       

    }

}
