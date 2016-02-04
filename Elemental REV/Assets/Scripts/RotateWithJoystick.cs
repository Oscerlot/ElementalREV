using UnityEngine;
using System.Collections;
using InControl;

public class RotateWithJoystick : MonoBehaviour {

    

	// Update is called once per frame
	void Update () {
        InputDevice device = InputManager.ActiveDevice;
        Camera.main.transform.LookAt(transform);
        transform.Rotate(0, device.RightStickX, 0);
	}
}
