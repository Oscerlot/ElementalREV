using UnityEngine;
using System.Collections;

public class LeverHandle : MonoBehaviour
{

    public Vector3 pullRotation;
    public Vector3 pushRotation;

    public enum HandleState
    {
        Pulled,
        Pushed,
        Pushing,
        Pulling
    }
    public HandleState CurrentHandleState = HandleState.Pushed;

    public void PullLever()
    {
        if (CurrentHandleState != HandleState.Pulling)
        {
            CurrentHandleState = HandleState.Pulling;
            iTween.RotateTo(gameObject,
                new Hashtable() {{"time", 1f}, {"rotation", pullRotation}, {"onComplete", "SetToPulledState"}});
        }
    }

    public void PushLever()
    {
        if (CurrentHandleState != HandleState.Pushing)
        {
            CurrentHandleState = HandleState.Pushing;
            iTween.RotateTo(gameObject,
                new Hashtable() {{"time", 1f}, {"rotation", pushRotation}, {"onComplete", "SetToPushedState"}});
        }
    }

    private void SetToPushedState()
    {
        CurrentHandleState = HandleState.Pushed;
    }

    private void SetToPulledState()
    {
        CurrentHandleState = HandleState.Pulled;
    }

}