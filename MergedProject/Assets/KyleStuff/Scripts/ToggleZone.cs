using UnityEngine;
using System.Collections;

public class ToggleZone : MonoBehaviour {
	
	public GameObject[] stuffToToggle;
	
	private bool first = true;
	
	void OnTriggerEnter (Collider col) {
		if (first) {
			for (int i = 0; i < stuffToToggle.Length; i++) {
				stuffToToggle[i].SetActive(false);
			}
			first = false;
		}
	}
}
