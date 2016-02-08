using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PushableObject : Interactable, IAttachable {

    List<Vector3> validAttachPositions = new List<Vector3>(); 

	// Use this for initialization
	void Start ()
	{
	    tag = "Pushable";
	    gameObject.layer = LayerMask.NameToLayer("Interactable");
        LoadValidAttachPositions();
	}

    private void LoadValidAttachPositions()
    {
        validAttachPositions.Add(transform.position + Vector3.forward);
        validAttachPositions.Add(transform.position + -Vector3.forward);
        validAttachPositions.Add(transform.position + Vector3.right);
        validAttachPositions.Add(transform.position + -Vector3.right);
    }

    public Vector3 GetAttachPosition(Transform goAttachingToAttachable)
    {
        return FindNearestAttachPosition(goAttachingToAttachable);
    }

    public Vector3 GetLookAtPosition()
    {
        return transform.position;
    }

    private Vector3 FindNearestAttachPosition(Transform goAttachingToAttachable)
    {
        var nearestAttachPosition = validAttachPositions[0];
        foreach (var validAttachPosition in validAttachPositions)
        {
            var distanceToCurrent = Vector3.Distance(validAttachPosition, goAttachingToAttachable.position);
            var distanceToNearest = Vector3.Distance(nearestAttachPosition, goAttachingToAttachable.position);

            if (distanceToCurrent < distanceToNearest)
                nearestAttachPosition = validAttachPosition;
        }

        return nearestAttachPosition;
    }

    void OnDrawGizmosSelected()
    {
        foreach (var validAttachPosition in validAttachPositions)
        {
            Gizmos.DrawWireCube(validAttachPosition + (Vector3.up * .5f), new Vector3(.8f, .8f, .8f));
        }
    }

}
