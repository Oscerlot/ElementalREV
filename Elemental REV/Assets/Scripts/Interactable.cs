using UnityEngine;
using System.Collections.Generic;

public abstract class Interactable : MonoBehaviour {

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
}
