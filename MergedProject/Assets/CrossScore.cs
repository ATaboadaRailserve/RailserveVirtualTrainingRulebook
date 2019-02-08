using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossScore : MonoBehaviour {

	public bool isRight, isFirstSet;
	public GameObject NextSet, Pass, CurSet, Killzone, NextRight, NextLeft, ExtraPass;
	// Use this for initialization
	void Start () {
		
	}


	void OnTriggerEnter(Collider guy) {
		if (guy.gameObject.tag == "Cars"){


			if (isRight) {
				if (isFirstSet) {
					NextRight.SetActive (false);
				}
			Debug.Log ("Checked Right");
			}
			if (!isRight) {
				if (isFirstSet) {
					NextLeft.SetActive (false);
				}
				Debug.Log ("Checked Left");
			}


			if (isFirstSet) {
				NextSet.SetActive (true);
				CurSet.SetActive (false);
			}
			if (!isFirstSet) {
				Killzone.SetActive (false);
				Pass.SetActive (true);
				ExtraPass.SetActive (true);

				CurSet.SetActive (false);
			}

		}
	}


	// Update is called once per frame
	void Update () {
		
	}
}

