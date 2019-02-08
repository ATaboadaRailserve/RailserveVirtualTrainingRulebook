using UnityEngine;
using System.Collections;

public class TriggerZone : MonoBehaviour {

	private GameObject world;
	
	void Start() {
		world = GameObject.Find("Start");
	}
	
	void OnTriggerEnter (Collider col) {
		world.SendMessage("Trigger");
	}
}
