using UnityEngine;
using System.Collections;

public class CameraWaypoints : MonoBehaviour {
	
	public Transform[] waypoints;
	public bool isActive;
	public Transform cameraObj;

	private int index;
	
	private bool keyPressed;

	void Start(){
		cameraObj.parent = waypoints[0];
		cameraObj.localPosition = Vector3.zero;
		cameraObj.localEulerAngles = Vector3.zero;
	}
	
	void Update(){
		if(!keyPressed && isActive && Input.GetAxis("Change View") != 0){
			keyPressed = true;
			index++;
			if(index > waypoints.Length -1)
				index = 0;
			cameraObj.parent = waypoints[index];
			cameraObj.localPosition = Vector3.zero;
			cameraObj.localEulerAngles = Vector3.zero;
		}
		else if(Input.GetAxis("Change View") == 0)
			keyPressed = false;
	}
	
	public void Toggle(){
		isActive = !isActive;
		index = 0;
		cameraObj.parent = waypoints[index];
		cameraObj.localPosition = Vector3.zero;
		cameraObj.localEulerAngles = Vector3.zero;
	}
}
