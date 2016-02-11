using UnityEngine;
using System.Collections.Generic;

public class PushableObject : Interactable {

    private List<Vector3> _attachPositions = new List<Vector3>(); 

	void Start ()
	{
	    tag = "Pushable";
	    gameObject.layer = LayerMask.NameToLayer("Interactable");
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

}
