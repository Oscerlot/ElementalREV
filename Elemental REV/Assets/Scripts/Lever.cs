using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;

public class Lever : Interactable
{

    public Transform interactPosition;
    public LeverHandle leverHandle;


	// Use this for initialization
	void Start () {
	    if (!interactPosition)
            Debug.LogWarning("There is no interact position assigned to this lever");
        if (!leverHandle)
            Debug.LogWarning("No lever handle attached");
	}

    public bool IsInUse()
    {
        return leverHandle.CurrentHandleState == LeverHandle.HandleState.Pulling || leverHandle.CurrentHandleState == LeverHandle.HandleState.Pushing;
    }

    public void ToggleLever()
    {
        switch (leverHandle.CurrentHandleState)
        {
                case LeverHandle.HandleState.Pulled:
                    leverHandle.PushLever();
                break;

                case LeverHandle.HandleState.Pushed:
                    leverHandle.PullLever();
                break;

        }
    }

    protected override List<Vector3> GetInteractPosition()
    {
        return new List<Vector3>() {interactPosition.position};
    }

    protected override Vector3 GetInteractLookAtPosition()
    {
        return interactPosition.position + Vector3.right;
    }
}
