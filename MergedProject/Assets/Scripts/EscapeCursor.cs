using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeCursor : MonoBehaviour {

	public KeyCode key;
	public CursorLockMode overrideState;
	
	private CursorLockMode savedMode;
	private bool currentlyEscaped;

	void Start()
	{
		EscapeCursor[] c = GameObject.FindObjectsOfType<EscapeCursor>();
		if(c.Length > 1)
		{
			Destroy(this.gameObject);
		}
		else
		{
		    DontDestroyOnLoad(this.gameObject);
		}
	}
	
	void LateUpdate()
	{
		if(Input.GetKeyDown(key))
		{
			if(currentlyEscaped)
			{
				if (overrideState != CursorLockMode.None)
					Cursor.lockState = overrideState;
				else
					Cursor.lockState = savedMode;
				Cursor.visible = false;
				currentlyEscaped = false;
				Debug.Log("Cursor No Longer Escaped");
			}
			else
			{
				savedMode = Cursor.lockState;
				currentlyEscaped = true;
				Debug.Log("Cursor Escaped");
			}
		}
		
		if(currentlyEscaped)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}
	
	public bool IsCurrentlyEscaped {
		get { return currentlyEscaped; }
		set { if(!value)
			{
				if (overrideState != CursorLockMode.None)
					Cursor.lockState = overrideState;
				else
					Cursor.lockState = savedMode;
				Cursor.visible = false;
				currentlyEscaped = false;
				Debug.Log("Cursor No Longer Escaped");
			}
			else
			{
				savedMode = Cursor.lockState;
				currentlyEscaped = true;
				Debug.Log("Cursor Escaped");
			} }
	}
}
