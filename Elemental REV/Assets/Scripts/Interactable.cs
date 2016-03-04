using UnityEngine;
using System.Collections.Generic;

public abstract class Interactable : MonoBehaviour
{

    public virtual bool IsBeingUsed()
    {
        return false;
    }

    //TODO: Figure out how I want to handle this
    //public virtual bool CanBeAccessed(Transform heroTransform)
    //{
    //    return GridTools.Instance.PositionIsAccessible(FindNearestPositionToPlayer(GetInteractPosition()), new[] { gameObject, heroTransform.gameObject });
    //}

    //The position/s the hero must reach before interacting with the interactable
    public List<Vector3> InteractPositions
    {
        get { return GetInteractPosition(); }
    }

    //The direction the hero must face before interacting with the interactable
    public Vector3 InteractLookAtPosition
    {
        get { return GetInteractLookAtPosition(); }
    }

    protected abstract List<Vector3> GetInteractPosition();
   

    protected virtual Vector3 GetInteractLookAtPosition()
    {
        return transform.position;
    }

    //TODO: Figure out how I want to handle this
    //private Vector3 FindNearestPositionToPlayer(List<Vector3> attachPositions)
    //{

    //    var nearestAttachPosition = attachPositions[0];
    //    foreach (var validAttachPosition in attachPositions)
    //    {
    //        var distanceToCurrent = Vector3.Distance(validAttachPosition, transform.position);
    //        var distanceToNearest = Vector3.Distance(nearestAttachPosition, transform.position);

    //        if (distanceToCurrent < distanceToNearest)
    //            nearestAttachPosition = validAttachPosition;
    //    }

    //    return nearestAttachPosition;
    //}

}
