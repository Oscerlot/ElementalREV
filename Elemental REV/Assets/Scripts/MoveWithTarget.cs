using UnityEngine;
using System.Collections;

public class MoveWithTarget : MonoBehaviour
{

    public Transform target;
    public float moveDamp = 1.5f;

    private Vector3 _velocity = Vector3.zero;


    void FixedUpdate()
    {
        transform.position = Vector3.SmoothDamp(this.transform.position, target.position, ref _velocity , moveDamp); ;
    }

}
