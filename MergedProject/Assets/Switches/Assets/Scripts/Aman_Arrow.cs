using UnityEngine;
using System.Collections;

public class Aman_Arrow : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 temp = transform.localEulerAngles;
		transform.localEulerAngles = new Vector3 (0, temp.y + 2, 0);
	}
}
