using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeCursor : MonoBehaviour {
	
	[System.Serializable]
	public enum Behavior { Down, Up, Hold }
	
	[System.Serializable]
	public struct KeyBehavior {
		public KeyCode key;
		public Behavior behavior;
	}
	
	public KeyBehavior[] keys;
	public CursorLockMode overrideState;
	
	private CursorLockMode savedMode;
	private bool currentlyEscaped;
	private bool wasHeld;

	void Start()
	{
		EscapeCursor[] c = GameObject.FindObjectsOfType<EscapeCursor>();
		if(c.Length > 1) {
			Destroy(this.gameObject);
		} else {
		    DontDestroyOnLoad(this.gameObject);
		}
	}
	
	void LateUpdate()
	{
		for (int i = 0; i < keys.Length; i++) {
			switch (keys[i].behavior) {
				case Behavior.Down:
					if (Input.GetKeyDown(keys[i].key)) {
						if (currentlyEscaped) {
							Escaped(false);
							return;
						} else {
							Escaped(true);
							return;
						}
					}
					break;
				case Behavior.Up:
					if (Input.GetKeyUp(keys[i].key)) {
						if (currentlyEscaped) {
							Escaped(false);
							return;
						} else {
							Escaped(true);
							return;
						}
					}
					break;
				case Behavior.Hold:
					if (!wasHeld && Input.GetKey(keys[i].key)) {
						wasHeld = true;
						Escaped(true);
						return;
					} else if (wasHeld && !Input.GetKey(keys[i].key)) {
						wasHeld = false;
						Escaped(false);
						return;
					}
					break;
			}
		}
		
		if (currentlyEscaped) {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}
	
	void Escaped (bool state) {
		if (!state) {
			if (overrideState != CursorLockMode.None)
				Cursor.lockState = overrideState;
			else
				Cursor.lockState = savedMode;
			Cursor.visible = false;
			currentlyEscaped = false;
			Debug.Log("Cursor No Longer Escaped");
		} else {
			savedMode = Cursor.lockState;
			currentlyEscaped = true;
			Debug.Log("Cursor Escaped");
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
