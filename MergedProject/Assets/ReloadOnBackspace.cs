using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadOnBackspace : MonoBehaviour {

	public void Update() {
		if (Input.GetKeyDown(KeyCode.Backspace)) {
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}
