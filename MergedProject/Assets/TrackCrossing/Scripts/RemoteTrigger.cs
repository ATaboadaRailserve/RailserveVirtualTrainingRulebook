using UnityEngine;
using System.Collections;

public class RemoteTrigger : MonoBehaviour {
	
	public GameObject target;
	
	void OnTriggerEnter (Collider col) {
		target.SendMessage("RemoteTrigger", col);
	}
}
