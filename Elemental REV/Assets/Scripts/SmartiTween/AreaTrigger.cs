using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// Detects objects that enter to the trigger collider and activates/de-activates attached interactable objects. 
/// </summary>

[RequireComponent(typeof(TriggerAwareness))]
public class AreaTrigger : MonoBehaviour
{

    [System.NonSerialized]
    public List<GameObject> objectsInTrigger;

    public TriggerObject[] objectsAffected;
    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;


    void OnDrawGizmosSelected()
    {
        if (objectsAffected == null)
            return;

        Gizmos.color = Color.blue;

        foreach (TriggerObject tObject in objectsAffected)
        {
            if (!tObject)
                return;
            Gizmos.DrawLine(this.transform.position, tObject.transform.position);
            if (tObject.GetComponent<MeshFilter>() == null)
                return;
            else
                Gizmos.DrawWireMesh(tObject.GetComponent<MeshFilter>().sharedMesh, -1, tObject.transform.position, tObject.transform.rotation, tObject.transform.lossyScale);
        }
    }

    void Start()
    {
        objectsInTrigger = GetComponent<TriggerAwareness>().collidingUnitsList;
    }

    void Update()
    {
        if (objectsInTrigger.Count > 0)
        {
            ActivateInteractables();
        }
        else
        {
            DeactivateInteractables();
        }
    }

    private void ActivateInteractables()
    {
        foreach (TriggerObject tObject in objectsAffected)
        {
            if (!tObject)
                return;
            if (!tObject.activated)
            {
                tObject.activate();

                if (OnActivate != null)
                    OnActivate.Invoke();
            }
        }
    }

    private void DeactivateInteractables()
    {
        foreach (TriggerObject tObject in objectsAffected)
        {
            if (!tObject)
                return;
            if (tObject.activated)
            {
                tObject.deActivate();

                if (OnDeactivate != null)
                    OnDeactivate.Invoke();
            }
        }
    }

    public void ActivateEvent()
    {
        ActivateInteractables();
        OnActivate.Invoke();
    }

}