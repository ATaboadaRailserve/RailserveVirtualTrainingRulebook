using UnityEngine;
using System.Collections;

public class CameraScipt : MonoBehaviour {

	bool shiftdown;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.LeftShift)) 
		{
			shiftdown = true;
		} 
		else 
		{
			shiftdown = false;
		}
		if (shiftdown) 
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = true;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}
}
