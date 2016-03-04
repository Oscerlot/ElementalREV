using UnityEngine;

public class LookAtCamera : MonoBehaviour
{

    private Transform _target;

    void Start()
    {
        _target = Camera.main.transform;
    }

	// Update is called once per frame
	void Update () {
	    transform.LookAt(_target);
	}
}
