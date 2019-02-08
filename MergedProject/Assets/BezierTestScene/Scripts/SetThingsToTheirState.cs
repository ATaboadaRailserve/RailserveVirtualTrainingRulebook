using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SetThingsToTheirState : MonoBehaviour {
	
	public bool setStuff = false;
	public GameObject[] thingsToOn;
	public GameObject[] thingsToOff;
	
	void Update () {
		if (setStuff) {
			setStuff = false;
			SetThings();
		}
	}
	
	void SetThings () {
		for (int i = 0; i < thingsToOn.Length; i++) {
			thingsToOn[i].SetActive(true);
		}
		for (int i = 0; i < thingsToOff.Length; i++) {
			thingsToOff[i].SetActive(false);
		}
	}
}
