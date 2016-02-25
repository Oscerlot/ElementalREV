using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HeroAwareness : MonoBehaviour {

    public LayerMask awareOfLayers;

    public GameObject CurrentObjectInAwareness { get { return _currentObjectInAwareness; } }

    private GameObject _currentObjectInAwareness;
    private float _detectionSphereRadius = .40f;
    private float _detectionSphereForwardDistance = .4f;
    private Collider _col;

    private Vector3 DetectionSpherePosition
    {
        get { return _col.bounds.center + transform.forward * _detectionSphereForwardDistance; }
    }

    // Use this for initialization
    void Awake ()
    {
        _col = GetComponent<Collider>();
    }
	
	// Update is called once per frame
	void Update () {
	    AwarenessCheck();
	}

    private void AwarenessCheck()
    {
        Collider[] colliders = Physics.OverlapSphere(DetectionSpherePosition, _detectionSphereRadius);
        List<GameObject> objectsDetected = FilterDetectedObjects(colliders);

        if (objectsDetected.Count > 0 && _currentObjectInAwareness == null)
            _currentObjectInAwareness = GetNearestObject(objectsDetected);
        
        if (_currentObjectInAwareness && !objectsDetected.Contains(_currentObjectInAwareness))
            _currentObjectInAwareness = null;
        
    }

    private List<GameObject> FilterDetectedObjects(Collider[] colliders)
    {
        return (from col in colliders where col.gameObject != gameObject && awareOfLayers == (awareOfLayers | (1 << col.gameObject.layer)) select col.gameObject).ToList();
    }

    private GameObject GetNearestObject(List<GameObject> interactables)
    {
        if (interactables.Count <= 0)
            return null;

        GameObject nearestObject = interactables[0];

        foreach (var interactable in interactables)
        {
            var distanceToObject = Vector3.Distance(interactable.transform.position, DetectionSpherePosition);
            var distanceToNearestObject = Vector3.Distance(nearestObject.transform.position, DetectionSpherePosition);

            if (distanceToObject < distanceToNearestObject)
                nearestObject = interactable;

        }

        return nearestObject;
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying && CurrentObjectInAwareness)
            Gizmos.DrawWireCube(CurrentObjectInAwareness.transform.position, Vector3.one);

        DisplayDetectionRange();
    }

    private void DisplayDetectionRange()
    {
        Gizmos.color = Color.white;
        if (_col)
            Gizmos.DrawWireSphere(DetectionSpherePosition, _detectionSphereRadius);
    }

}
