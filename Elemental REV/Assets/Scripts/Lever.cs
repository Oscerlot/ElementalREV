using UnityEngine;
using System.Collections.Generic;

public class Lever : Interactable
{
    public List<SmartItweenObject> objectsToActivate; 
    public Transform pullInteractPosition;
    public Transform pushInteractPosition;
    public LeverHandle leverHandle;


	// Use this for initialization
	void Start () {
	    if (!pullInteractPosition || ! pushInteractPosition)
            Debug.LogWarning("There is no interact position assigned to this lever");
        if (!leverHandle)
            Debug.LogWarning("No lever handle attached");
	}

    public void ToggleLever()
    {
        switch (leverHandle.CurrentHandleState)
        {
            case LeverHandle.HandleState.Pulled:
                leverHandle.PushLever(delegate() { objectsToActivate.ForEach(smartGo => smartGo.deActivate()); });
                
            break;

            case LeverHandle.HandleState.Pushed:
                leverHandle.PullLever(delegate() { objectsToActivate.ForEach(smartGo => smartGo.activate()); });
            break;

        }
    }

    public override bool IsBeingUsed()
    {
        return leverHandle.CurrentHandleState == LeverHandle.HandleState.Pulling || leverHandle.CurrentHandleState == LeverHandle.HandleState.Pushing;
    }


    protected override List<Vector3> GetInteractPosition()
    {
        return new List<Vector3>() {leverHandle.CurrentHandleState == LeverHandle.HandleState.Pushed ? pullInteractPosition.position : pushInteractPosition.position};
    }

    protected override Vector3 GetInteractLookAtPosition()
    {
        return pullInteractPosition.position + Vector3.right;
    }


    void OnDrawGizmosSelected()
    {
        HighlightObjectsToActivate();
    }

    private void HighlightObjectsToActivate()
    {
        if (objectsToActivate.Count > 0)
        {
            foreach (var smartGo in objectsToActivate)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, smartGo.transform.position);
                if (smartGo.GetComponent<MeshFilter>() == null)
                    return;
                else
                    Gizmos.DrawWireMesh(smartGo.GetComponent<MeshFilter>().sharedMesh, -1, smartGo.transform.position,
                        smartGo.transform.rotation, smartGo.transform.lossyScale);
            }
        }
    }
}
