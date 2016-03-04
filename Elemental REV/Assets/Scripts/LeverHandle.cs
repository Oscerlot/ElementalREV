using System;
using UnityEngine;
using System.Collections;

public class LeverHandle : MonoBehaviour
{

    public Vector3 pullRotation;
    public Vector3 pushRotation;

    private Action _endAction;

    public enum HandleState
    {
        Pulled,
        Pushed,
        Pushing,
        Pulling
    }
    public HandleState CurrentHandleState = HandleState.Pushed;

    public void PullLever(Action endAction)
    {
        if (endAction != null)
            _endAction = endAction;

        if (CurrentHandleState != HandleState.Pulling)
        {
            CurrentHandleState = HandleState.Pulling;
            iTween.RotateTo(gameObject,
                new Hashtable() {{"time", 2f}, {"rotation", pullRotation}, {"onComplete", "SetToPulledState"}, {"easeType", "linear"} });
        }
    }

    public void PushLever(Action endAction)
    {
        if (endAction != null)
            _endAction = endAction;

        if (CurrentHandleState != HandleState.Pushing)
        {
            CurrentHandleState = HandleState.Pushing;
            iTween.RotateTo(gameObject,
                new Hashtable() {{"time", 2f}, {"rotation", pushRotation}, {"onComplete", "SetToPushedState"}, { "easeType", "linear" } });
        }
    }

    private void SetToPushedState()
    {
        CurrentHandleState = HandleState.Pushed;
        if (_endAction != null)
            _endAction.Invoke();
    }

    private void SetToPulledState()
    {
        CurrentHandleState = HandleState.Pulled;
        if (_endAction != null)
            _endAction.Invoke();
    }

}