using UnityEngine;
using System.Collections;

public class rtDemoCamera : MonoBehaviour
{
	Camera cam;
	
	void Awake()
	{
		cam = this.gameObject.AddComponent(typeof(Camera)) as Camera;
		cam.fieldOfView = 75f;
		Input.simulateMouseWithTouches = true;
	}
	
	private Quaternion xRot;
	private Quaternion yRot;
	private Vector3 camVector;
	void Update()
	{
		GatherKeyboardInput();
				
		xRot = Quaternion.Euler(-20,0f,0f);
		yRot = Quaternion.Euler(0f,degreesAround,0f);
		
		camVector = Vector3.forward;
		camVector = xRot * camVector;
		camVector = yRot * camVector;
		
		transform.position = (Vector3.up * 2f) + (12f * camVector);
		transform.LookAt((Vector3.up * 2f));
		
		if(rotate == false || rotationDir == 0f)
			acceleration = 0f;
		
		else
			Rotate();
		
		GatherMouseInput();
	}
	
	private RepTile.CastInfo tileCastInfo;
	private void GatherMouseInput()
	{
		if (Input.GetMouseButtonDown(0) && GUIUtility.hotControl == 0)
		{
			Ray tileCastRay = cam.ScreenPointToRay(Input.mousePosition);
			tileCastInfo = RepTile.Public.TileCast(rtDemoGameManager.gameMap,tileCastRay.origin,tileCastRay.direction);
			
			if(tileCastInfo != null && tileCastInfo.hitSuccess == true)
			{
				LaunchMisile(tileCastInfo.hitIndex);
			}
		}
		
	}
	
	private float rotationDir = 0f;
	private bool rotate = false;
	private void GatherKeyboardInput()
	{
		if(Input.GetKey(KeyCode.RightArrow))
		{
			rotationDir -= 1f;
			rotate = true;
		}
		
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			rotationDir += 1f;
			rotate = true;
		}
	}
	
	private static int screenHeight;
	private static Vector3 screenPt;
	void OnGUI()
	{
		
		if(GUI.Button(new Rect(0, 0, 100, 75), "RESET Map1"))
		{
			rtDemoGameManager.SpawnMap(0);
		}
		
		if(GUI.Button(new Rect(100, 0, 100, 75), "RESET Map2"))
		{
			rtDemoGameManager.SpawnMap(1);
		}
		if(GUI.Button(new Rect(200, 0, 100, 75), "QUIT"))
		{
			Application.Quit();
		}
		
		screenHeight = Mathf.RoundToInt(Screen.height);
		screenPt = cam.ViewportToScreenPoint(new Vector3(1f,0.5f,0f));
		if(GUI.RepeatButton(new Rect(0, 150, 100, screenHeight - 150), RotateCameraButton[0], RotateCameraButtonStyle))
		{
			rotationDir += 1f;
			rotate = true;
		}

		if(GUI.RepeatButton(new Rect(screenPt.x - 100, 150, 100, screenHeight - 150),RotateCameraButton[1], RotateCameraButtonStyle))
		{
			rotationDir -= 1f;
			rotate = true;
		}
	}
	
	private static Texture2D[] rotateCameraButton;
	private static Texture2D[] RotateCameraButton
	{
		get{
			if(rotateCameraButton == null || rotateCameraButton.Length == 0)
			{
				rotateCameraButton = new Texture2D[2];
				rotateCameraButton[0] = Resources.Load("rtDemo_RotateCameraButtonL") as Texture2D;
				rotateCameraButton[1] = Resources.Load("rtDemo_RotateCameraButtonR") as Texture2D;
			}
			return rotateCameraButton;
		}	
	}
	
	private static GUIStyle rotateCameraButtonStyle;
	private static GUIStyle RotateCameraButtonStyle
	{
		get{
			if(rotateCameraButtonStyle == null)
			{
				rotateCameraButtonStyle = new GUIStyle();
				rotateCameraButtonStyle.alignment = TextAnchor.MiddleCenter;
			}
			return rotateCameraButtonStyle;
		}
	}
	
	private const float maxSpeed = 50f;
	private float acceleration = 0f;
	private float degreesAround = 45f;
	void Rotate()
	{
		acceleration =  Mathf.Clamp01(acceleration + Time.deltaTime * 0.75f);
		
		degreesAround += Time.deltaTime * (acceleration * maxSpeed) * rotationDir;
		degreesAround = degreesAround % 360;
		
		rotate = false;
		rotationDir = 0f;
	}
	
	private static GameObject misilePrefab = Resources.Load("rtDemoMisile") as GameObject;
	private void LaunchMisile(RepTile.int3 targetIndex)
	{
		GameObject misileObject = (GameObject)GameObject.Instantiate(misilePrefab,this.transform.position + (this.transform.forward * 0.5f) + (-this.transform.up),Quaternion.LookRotation(this.transform.up,this.transform.forward));
		rtDemoMisile misile = (rtDemoMisile)misileObject.AddComponent(typeof(rtDemoMisile));
		misile.SetTarget(rtDemoGameManager.gameMap,targetIndex);
	}
}
