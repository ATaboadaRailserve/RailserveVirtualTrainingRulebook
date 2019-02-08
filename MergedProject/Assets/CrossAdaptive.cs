using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossAdaptive : MonoBehaviour {
	public GameObject Looks;
	// Use this for initialization
	void Start () {
		
	}

	void OnTriggerEnter(Collider guy) {
		if (guy.gameObject.tag == "Player"){
			Looks.SetActive (true);
			Debug.Log ("Spawning Looks");
		}
	}


	// Update is called once per frame
	void Update () {
		
	}
}
