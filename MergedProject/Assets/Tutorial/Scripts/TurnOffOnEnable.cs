using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffOnEnable : MonoBehaviour {
	
	public GameObject[] targets;
	
	void OnEnable () {
		foreach (GameObject g in targets) {
			//g.SetActive(false);
			Destroy(g);
		}
	}
}
