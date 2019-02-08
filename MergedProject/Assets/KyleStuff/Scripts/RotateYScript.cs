using UnityEngine;
using System.Collections;

public class RotateYScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(0.0f, 0.5f, 0.0f);
	}
}
