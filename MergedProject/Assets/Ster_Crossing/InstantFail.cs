using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantFail : MonoBehaviour {

	public GameObject Fail;

	// Use this for initialization
	void Start () {

		Fail.SetActive (false);
	}



	void OnTriggerEnter(Collider guy) {
		if (guy.gameObject.tag == "Player"){
			Fail.SetActive (true);
			Debug.Log ("you hit a no no");
		}
	}


	// Update is called once per frame
	void Update () {
		
	}
}
