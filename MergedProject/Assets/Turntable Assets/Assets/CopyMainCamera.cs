using UnityEngine;
using System.Collections;

public class CopyMainCamera : MonoBehaviour {
    GameObject mainCamera;
	// Use this for initialization
	void Start () {
        mainCamera = transform.parent.GetChild(0).gameObject;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.position = mainCamera.transform.position;
        transform.rotation = mainCamera.transform.rotation;
	}
}
