using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Collider))]
public class HeroInteract : MonoBehaviour
{

    public LayerMask whatIsInteractable;

    public GameObject CurrentInteractable {
        get { return _currentInteractable; }
    }


    private GameObject _currentInteractable;
    private float _detectionSphereRadius = .40f;
    private float _detectionSphereForwardDistance = .4f;
    private Collider _col;

    private Vector3 DetectionSpherePosition {
        get { return _col.bounds.center + transform.forward*_detectionSphereForwardDistance; }
    }


    void Start()
    {
        _col = GetComponent<Collider>();                
    }


    public void CheckForInteractables(PlayerInput.interactState interactState)
    {        
        Collider[] colliders = Physics.OverlapSphere(DetectionSpherePosition, _detectionSphereRadius);
        List<GameObject> interactablesDetected = FilterInteractables(colliders);

        // Interactable Detected
        if (interactablesDetected.Count > 0)
        {
            if (interactState == PlayerInput.interactState.begun && _currentInteractable == null)
            {
                _currentInteractable = GetNearestInteractable(interactablesDetected);
            }
        }

        if (_currentInteractable != null)
        {
        
            if (interactState == PlayerInput.interactState.ended || !interactablesDetected.Contains(_currentInteractable))
            {
                _currentInteractable = null;              
            }
        }       
    }


    private GameObject GetNearestInteractable(List<GameObject> interactables)
    {
        GameObject nearestInteractable = interactables[0];
        foreach (var interactable in interactables)
        {
            var distanceToInteractable = Vector3.Distance(interactable.transform.position, DetectionSpherePosition);
            var distanceToNearestInteractable = Vector3.Distance(nearestInteractable.transform.position, DetectionSpherePosition);

            if (distanceToInteractable < distanceToNearestInteractable)
            {
                nearestInteractable = interactable;
            }
        }
        return nearestInteractable;
    }


    private List<GameObject> FilterInteractables(Collider[] colliders)
    {
        List<GameObject> interactables = new List<GameObject>();

        foreach (Collider col in colliders)
        {
            if (whatIsInteractable == (whatIsInteractable | (1 << col.gameObject.layer)))
            {
                interactables.Add(col.gameObject);
            }
        }

        return interactables;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (_currentInteractable)
            Gizmos.DrawWireCube(_currentInteractable.transform.position, Vector3.one * 1.3f);

        Gizmos.color = Color.white;
        if (_col)
            Gizmos.DrawWireSphere(DetectionSpherePosition, _detectionSphereRadius);

    }

}




