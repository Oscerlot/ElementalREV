using UnityEngine;

public interface IAttachable
{

    Vector3 GetAttachPosition(Transform goAttachingToAttachable);
    Vector3 GetLookAtPosition();


}



