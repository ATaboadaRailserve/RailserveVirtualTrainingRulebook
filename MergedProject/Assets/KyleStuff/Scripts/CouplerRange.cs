using UnityEngine;
using System.Collections;

public class CouplerRange : MonoBehaviour {
	
	[HideInInspector]
	public bool inRange;

	void OnTriggerStay (Collider collider) {
		if (collider.gameObject.tag == "Player")
			inRange = true;
	}

	void OnTriggerExit (Collider collider) {
		if (collider.gameObject.tag == "Player")
			inRange = false;
	}
	
	public void UncoupleFromTrain() {
		transform.parent.GetComponent<SmartTankerScript>().UncoupleFromTrain();
	}
}
