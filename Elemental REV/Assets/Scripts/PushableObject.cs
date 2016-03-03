using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class PushableObject : Interactable {

    public bool beingPushed = false;

    private List<Vector3> _attachPositions = new List<Vector3>();
    private Rigidbody _rgBody;    
    private float _gridMoveTime = 1f;


	void Start ()
	{
	    tag = "Pushable";
	    gameObject.layer = LayerMask.NameToLayer("Interactable");
	    _rgBody = GetComponent<Rigidbody>();
	}

    void Update()
    {       

        GroundCheck();

    }


    public void Push(Vector3 direction, float timeTaken)
    {
        if (direction != Vector3.zero &&
            GridTools.Instance.PositionIsAccessible(transform.position + direction, new[] { gameObject }))
        {
            direction.y = 0;
            GridMove(direction, _gridMoveTime);
        }
    }

    private void GridMove(Vector3 direction, float timeTaken)
    {
        var destination = transform.position + direction;
        if (!beingPushed && _rgBody.isKinematic)
        {
            iTween.MoveTo(gameObject,
                new Hashtable() {{"position", destination}, {"time", timeTaken}, {"onComplete", "DestinationReached"}, {"easeType", "easeInOutSine" } });
            beingPushed = true;
        }
    }

    void GroundCheck()
    {
        //If not grounded
        if (!beingPushed && !Physics.Raycast(transform.position + (Vector3.up * .01f), -transform.up, .02f))
        {
            //Fall
            _rgBody.constraints = (RigidbodyConstraints) 122;
            _rgBody.isKinematic = false;           
        }
        else
        {
            _rgBody.constraints = RigidbodyConstraints.FreezeRotation;
            _rgBody.isKinematic = true;
        }

    }

    private void DestinationReached()
    {        
        beingPushed = false;
        GroundCheck(); //To keep the player from pushing an extra grid space when the pushable is not grounded
    }

    protected override List<Vector3> GetInteractPosition()
    {
        var list = new List<Vector3>()
        {
            transform.position + Vector3.forward,
            transform.position + -Vector3.forward,
            transform.position + Vector3.right,
            transform.position + -Vector3.right
        };
        _attachPositions = new List<Vector3>(list);
        return _attachPositions;
    }

    void OnDrawGizmosSelected()
    {
        foreach (var validAttachPosition in _attachPositions)
        {
            Gizmos.DrawWireCube(validAttachPosition + (Vector3.up * .5f), new Vector3(.8f, .8f, .8f));
        }
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        if (GridTools.Instance.PositionIsAccessible(transform.position + (Vector3.down * .99f), new[] { gameObject }))
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;
            
        Gizmos.DrawWireCube(GridTools.Instance.SnapVectorToGrid(transform.position + (Vector3.down * .99f)), Vector3.one);

       
    }

}
