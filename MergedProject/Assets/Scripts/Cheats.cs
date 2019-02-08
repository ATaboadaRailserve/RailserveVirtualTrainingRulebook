using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cheats : MonoBehaviour {

	public string scene = "MainMenu";
	public KeyCode[] skipKeys;
	
	// Update is called once per frame
	void Update () {
		foreach (KeyCode k in skipKeys) {
			if (Input.GetKeyUp(k)) {
				SceneManager.LoadScene(scene);
			}
		}
	}
}
