using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroInteract : MonoBehaviour
{

    public LayerMask whatIsInteractable;

    private List<GameObject> _interactablesDetected = new List<GameObject>();
    private GameObject _currentInteractable;




    public void CheckForInteractables(PlayerInput.interactState interactState)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward * 0.5f, .30f);
        List<GameObject> interactablesDetected = FilterInteractables(colliders);

        // Interactable Detected
        if (interactablesDetected.Count > 0)
        {
            if (interactState == PlayerInput.interactState.begun && _currentInteractable == null)
            {
                _currentInteractable = interactablesDetected[0].gameObject;
            }
        }

        if (_currentInteractable != null)
        {
        
            if (interactState == PlayerInput.interactState.released)
            {
                _currentInteractable = null;              
            }
        }       
    }


    private List<GameObject> FilterInteractables(Collider[] colliders)
    {
        List<GameObject> interactables = new List<GameObject>();

        foreach (Collider col in colliders)
        {
            if (whatIsInteractable == (whatIsInteractable | (1 << col.gameObject.layer)))
            {
                Debug.Log("Interactable Detected");
                interactables.Add(col.gameObject);
            }
        }

        return interactables;
    }

    void OnDrawGizmos()
    {
        if (_currentInteractable)
            Gizmos.DrawWireCube(_currentInteractable.transform.position, Vector3.one * 3f);
    }

}




