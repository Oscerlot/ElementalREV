using UnityEngine;
using System.Collections;
using InControl;

public class RotateWithJoystick : MonoBehaviour
{

    public float rotateSpeed = 5;


	// Update is called once per frame
	void Update () {
        InputDevice device = InputManager.ActiveDevice;
        Camera.main.transform.LookAt(transform);

	    if (!device)
	        return;

	    //Vector3 from = transform.rotation.eulerAngles;
        //Vector3 to = new Vector3(from.x, from.y + device.RightStickX, from.z + device.RightStickY);

        //transform.rotation = Quaternion.Slerp(Quaternion.Euler(from), Quaternion.Euler(to), Time.time * rotateSpeed);

	    transform.Rotate(0, device.RightStickX > 0.5? device.RightStickX : 0f, device.RightStickY, Space.Self);


	}

    void OnGUI()
    {
        //GUILayout.HorizontalSlider()
    }

}
