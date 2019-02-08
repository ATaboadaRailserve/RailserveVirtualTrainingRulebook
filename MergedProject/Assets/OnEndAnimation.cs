using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEndAnimation : MonoBehaviour {


	public GameObject ObjectToSpawn, ObjectToKill;

	// Use this for initialization
	void Awake () {
		
		ObjectToSpawn.gameObject.SetActive (false);
	}


	public void SpawnOnEnd(){
		ObjectToSpawn.gameObject.SetActive (true);

		ObjectToKill.gameObject.SetActive (false);
	}



	// Update is called once per frame
	void Update () {
		
	}
}
