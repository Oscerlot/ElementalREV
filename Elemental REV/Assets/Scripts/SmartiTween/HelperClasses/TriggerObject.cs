using UnityEngine;

public class TriggerObject : MonoBehaviour {

    [HideInInspector]
    public bool activated;

	public virtual void activate()
    {
        activated = true;
    }

	public virtual void deActivate()
    {
        activated = false;
    }
}
