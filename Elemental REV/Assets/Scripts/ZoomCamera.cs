using UnityEngine;
using InControl;

public class ZoomCamera : MonoBehaviour
{

    public float zoomSpeedMultiplier = 2;
    public float zoomInAmount = 10;

    private float _startZoom;
    private Camera _cam;

    private float _zoomMagnitude = 0;

	// Use this for initialization
	void Start ()
	{
	    _cam = GetComponentInChildren<Camera>();
	    _startZoom = _cam.fieldOfView;
	}
	
	// Update is called once per frame
	void Update () {
        InputDevice device = InputManager.ActiveDevice;
        if (device)
        {
            if (device.LeftTrigger.IsPressed)
            {
                if (_zoomMagnitude <= 1)
                {
                    ZoomIn();
                }
            }
            else if (_zoomMagnitude >= 0)
            {
                ZoomOut();
            }
        }
    }

    private void ZoomOut()
    {
        _zoomMagnitude -= Time.deltaTime*zoomSpeedMultiplier;
        _cam.fieldOfView = Mathf.Lerp(_startZoom, _startZoom - zoomInAmount, _zoomMagnitude);
    }

    private void ZoomIn()
    {
        _zoomMagnitude += Time.deltaTime*zoomSpeedMultiplier;
        _cam.fieldOfView = Mathf.Lerp(_startZoom, _startZoom - zoomInAmount, _zoomMagnitude);
    }


}
