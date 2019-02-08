using UnityEngine;
using System.Collections;

public class NPCTextCollider : MonoBehaviour {
	
	public GameObject target;
	
	void OnTriggerEnter (Collider col) {
		target.SetActive(true);
	}
	
	void OnTriggerExit (Collider col) {
		target.SetActive(false);
	}
}
