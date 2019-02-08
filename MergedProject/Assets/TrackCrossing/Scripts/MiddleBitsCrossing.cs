using UnityEngine;
using System.Collections;

public class MiddleBitsCrossing : MonoBehaviour {
	
	public bool game = true;
	
	void OnTriggerExit (Collider col) {
		if (game) {
			if (col.gameObject.tag == "Player") {
				GameObject.FindWithTag("CrossingRailer").SendMessage("ScoreUp", false);
				game = false;
			}
		}
	}
}
