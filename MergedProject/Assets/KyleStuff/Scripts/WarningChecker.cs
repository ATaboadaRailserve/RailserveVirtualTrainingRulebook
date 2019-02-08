using UnityEngine;
using System.Collections;

public class WarningChecker : MonoBehaviour {

	public Camera orthoCamera;
	public Camera rickCamera;
	private Transform controlReturn;
	public GameObject warning;

	// Use this for initialization
	void Start () {
		orthoCamera.enabled = true;
		rickCamera.enabled = false;
		warning.SetActive(false);
	}

	public void GoBackToOrthoCamera() {
		// Make our guy active again (to receive input)
		if (controlReturn.tag == "Coupler") {
			controlReturn.GetComponent<Coupler>().isActive = true;
			controlReturn.GetComponent<Coupler>().CreateWarningZone();
		}
		else if (controlReturn.tag == "Switcher"){
			controlReturn.GetComponent<Switcher>().isActive = true;
			controlReturn.GetComponent<Switcher>().CreateWarningZone();
		}
		orthoCamera.enabled = true;
		rickCamera.enabled = false;
		warning.SetActive(false);
	}

	// Remember who to return control to
	public void SetReturnCharacter(Transform t) {
		Debug.Log("WarningChecker::Setting return char");
		controlReturn = t;
	}


	// Switch to the warning camera
	public void SwitchCameras() {
		Debug.Log("WarnningChecker::Switching cameras");
			orthoCamera.enabled = false;
			rickCamera.enabled = true;
			warning.SetActive(true);
	}

}
