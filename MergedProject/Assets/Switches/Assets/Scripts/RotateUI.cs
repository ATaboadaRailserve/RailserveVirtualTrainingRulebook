using UnityEngine;
using System.Collections;

public class RotateUI : MonoBehaviour {

	GameObject player;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("MainCamera");
	}
	
	// Update is called once per frame
	void Update () {
		if (player != null) {
			this.transform.LookAt (player.transform);
			this.transform.Rotate (new Vector3 (0, 180, 0));
		}
	}
}
