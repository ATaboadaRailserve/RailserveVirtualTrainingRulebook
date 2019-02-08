using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Camera))]
public class CameraView : MonoBehaviour {
	
	void Ding (bool state) {
		GetComponent<Camera>().enabled = state;
	}
}
