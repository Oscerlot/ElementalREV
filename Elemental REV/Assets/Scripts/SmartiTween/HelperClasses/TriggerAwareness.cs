using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps track of units/objects inside of a trigger. If one of the objects is deactivated while inside the trigger, it is removed from the list.
/// </summary>
public class TriggerAwareness : MonoBehaviour {

    //Public Variables
    //----------------------------------------------------------------------------------------------
    [System.NonSerialized]
    public List<GameObject> collidingUnitsList;         //A list containing all the objects detected inside of the trigger (after layer filter)
    public LayerMask activatedBy;                       //Filter used to detect specific layers
    public enum AwarenessType { collision, trigger }
    public AwarenessType colliderType = AwarenessType.trigger;

    //EventCalls
    //----------------------------------------------------------------------------------------------
    void Awake()
    {
        collidingUnitsList = new List<GameObject>();

    }

    void Update()
    {
        removeDeactivatedObjects();

    }

    void OnEnable()
    {
        collidingUnitsList.Clear();
    }

    //Keep track of the objects entering the trigger
    void OnTriggerEnter(Collider other)
    {
        if (colliderType == AwarenessType.trigger)
            FilterIntoList(other.gameObject);


    }

    //Remove the objects who leave the trigger from the list of colliding objects
    void OnTriggerExit(Collider other)
    {
        if (colliderType == AwarenessType.trigger)
            FilterOutOfList(other.gameObject);
    }

    void OnCollisionEnter(Collision col)
    {
        if (colliderType == AwarenessType.collision)
            FilterIntoList(col.gameObject);
    }

    void OnCollisionExit(Collision col)
    {
        if (colliderType == AwarenessType.collision)
            FilterOutOfList(col.gameObject);
    }



    //----------------------------------------------------------------------------------------------

    private void FilterIntoList(GameObject go)
    {
        if ((activatedBy.value & 1 << go.layer) != 0)
        {
            if (!collidingUnitsList.Contains(go.gameObject))
            {
                collidingUnitsList.Add(go.gameObject);
            }
        }
    }

    private void FilterOutOfList(GameObject go)
    {
        if ((activatedBy.value & 1 << go.layer) != 0)
        {

            if (collidingUnitsList.Contains(go.gameObject))
            {
                collidingUnitsList.Remove(go.gameObject);
            }
        }
    }

    /// <summary>
    /// Remove inactive objects from the list of colliding objects
    /// </summary>
    private void removeDeactivatedObjects()
    {
        List<GameObject> tempColObjects = collidingUnitsList;
        foreach (GameObject colObject in tempColObjects)
        {
            if (!colObject.activeInHierarchy)
            {
                tempColObjects.Remove(colObject);
                break;
            }
        }
    }


    //-----------------------------------------------------------------------------------------
}
