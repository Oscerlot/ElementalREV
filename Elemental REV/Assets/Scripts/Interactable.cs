using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Interactable : MonoBehaviour {

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

    //TODO: Fix Bug, this throws a out of bounds index error if not overriten by pushable object
    protected virtual List <Vector3> GetInteractPosition()
    {
        return new List<Vector3>();
    }

    protected virtual Vector3 GetInteractLookAtPosition()
    {
        return transform.position;
    }
}
