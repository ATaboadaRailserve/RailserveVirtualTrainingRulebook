using UnityEngine;
using System.Collections;

public class LookAtCamera : MonoBehaviour {
    GameObject camera;
	// Use this for initialization
	void Start () {
        camera = Camera.main.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(camera.transform);
	}
}
