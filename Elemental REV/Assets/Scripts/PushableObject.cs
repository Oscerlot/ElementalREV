using UnityEngine;
using System.Collections.Generic;

public class PushableObject : Interactable {

    readonly List<Vector3> _attachPositions = new List<Vector3>(); 

	void Start ()
	{
	    tag = "Pushable";
	    gameObject.layer = LayerMask.NameToLayer("Interactable");
        LoadValidAttachPositions();
	}

    private void LoadValidAttachPositions()
    {
        _attachPositions.Add(transform.position + Vector3.forward);
        _attachPositions.Add(transform.position + -Vector3.forward);
        _attachPositions.Add(transform.position + Vector3.right);
        _attachPositions.Add(transform.position + -Vector3.right);
    }

    protected override List<Vector3> GetInteractPosition()
    {
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
