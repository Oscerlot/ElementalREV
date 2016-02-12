using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using InControl;

public class PushableObject : Interactable {

    private List<Vector3> _attachPositions = new List<Vector3>();

    private bool _moving = false;
    private float _gridMoveTime = .7f;
    private float _fallingTime = .2f;

	void Start ()
	{
	    tag = "Pushable";
	    gameObject.layer = LayerMask.NameToLayer("Interactable");
	}

    void Update()
    {
        InputDevice device = InputManager.ActiveDevice;

        var moveDirection = Vector3.zero;

        if (device)
        {
            if (device.DPadRight.WasPressed)
            {
                moveDirection = Vector3.right;
            }
            else if (device.DPadLeft.WasPressed)
            {
                moveDirection = Vector3.left;
            }
            else if (device.DPadUp.WasPressed)
            {
                moveDirection = Vector3.forward;
            }
            else if (device.DPadDown.WasPressed)
            {
                moveDirection = Vector3.back;
            }
        }

        if (moveDirection != Vector3.zero &&
            GridTools.Instance.PositionIsAccessible(transform.position + moveDirection, new[] {gameObject}))
        {
            GridMove(moveDirection, _gridMoveTime);
            Debug.Log(GridTools.Instance.SnapVectorToGrid(transform.position + moveDirection));
        }

    }

    private void GridMove(Vector3 direction, float timeTaken)
    {
        var destination = transform.position + direction;
        if (!_moving)
        {
            iTween.MoveTo(gameObject,
                new Hashtable() {{"position", destination}, {"time", timeTaken}, {"onComplete", "DestinationReached"}, {"easeType", "linear"} });
            _moving = true;
        }
    }

    void GroundCheck()
    {
        if (GridTools.Instance.PositionIsAccessible(transform.position + (Vector3.down), new[] {gameObject}))
        {
            GridMove(Vector3.down, _fallingTime);
        }

    }

    private void DestinationReached()
    {
        _moving = false;
        GroundCheck();
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

        if (GridTools.Instance.PositionIsAccessible(transform.position + (Vector3.down), new[] { gameObject }))
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;
            
        Gizmos.DrawWireCube(GridTools.Instance.SnapVectorToGrid(transform.position + (Vector3.down)), Vector3.one);

       
    }

}
