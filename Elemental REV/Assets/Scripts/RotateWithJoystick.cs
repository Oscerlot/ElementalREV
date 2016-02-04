using UnityEngine;
using InControl;

public class RotateWithJoystick : MonoBehaviour
{

    public float rotateSpeed = 5;
    public float zRotationAngleLimit = 20;

    private float _maxZRotation;
    private float _minZrotation;

    void Start()
    {
        _maxZRotation = (transform.rotation.eulerAngles.z + zRotationAngleLimit) % 360;
        _minZrotation = ((transform.rotation.eulerAngles.z - zRotationAngleLimit) % 360 + 360) % 360;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Camera.main.transform.LookAt(transform);

        InputDevice device = InputManager.ActiveDevice;
        if (!device)
            return;

        var xRotation = device.RightStickX * rotateSpeed;
        var zRotation = device.RightStickY * rotateSpeed;

        SingleAxisRotation(xRotation, zRotation);

    }

    private void SingleAxisRotation(float xRotation, float zRotation)
    {
        if (Mathf.Abs(xRotation) > Mathf.Abs(zRotation))
        {
            RotateOnXAxis(xRotation);
        }
        else if (ZRotationIsWithinLimits(zRotation))
        {
            RotateOnZAxis(zRotation);
        }
    }

    private void RotateOnZAxis(float zRotation)
    {
        transform.Rotate(0, 0, zRotation, Space.Self);
    }

    private void RotateOnXAxis(float xRotation)
    {
        transform.Rotate(0, xRotation, 0, Space.World);
    }

    private bool ZRotationIsWithinLimits(float zRotation)
    {
        return Mathf.Abs(transform.rotation.eulerAngles.z + zRotation) < _maxZRotation || Mathf.Abs(transform.rotation.eulerAngles.z + zRotation) > _minZrotation;
    }
}
