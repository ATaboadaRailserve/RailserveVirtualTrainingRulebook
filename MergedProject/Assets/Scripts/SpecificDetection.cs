using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecificDetection : MonoBehaviour {
	
	[Header("Specifications")]
	public string name;
	public string tag;
	
	[Header("Trigger Actions")]
	public InteractionHandler.InvokableState onTriggerEnter;
	public InteractionHandler.InvokableState onTriggerStay;
	public InteractionHandler.InvokableState onTriggerExit;
	
	[Header("Collision Actions")]
	public InteractionHandler.InvokableState onColliderEnter;
	public InteractionHandler.InvokableState onColliderStay;
	public InteractionHandler.InvokableState onColliderExit;
	
	
	void OnTriggerEnter (Collider col) {
		if (name != "" && col.gameObject.name != name)
			return;
		if (tag != "" && col.gameObject.tag != tag)
			return;
		onTriggerEnter.Invoke();
	}
	
	void OnTriggerStay (Collider col) {
		if (name != "" && col.gameObject.name != name)
			return;
		if (tag != "" && col.gameObject.tag != tag)
			return;
		onTriggerStay.Invoke();
	}
	
	void OnTriggerExit (Collider col) {
		if (name != "" && col.gameObject.name != name)
			return;
		if (tag != "" && col.gameObject.tag != tag)
			return;
		onTriggerExit.Invoke();
	}
	
	
	void OnCollisionEnter (Collision col) {
		if (name != "" && col.gameObject.name != name)
			return;
		if (tag != "" && col.gameObject.tag != tag)
			return;
		onColliderEnter.Invoke();
	}
	
	void OnCollisionStay (Collision col) {
		if (name != "" && col.gameObject.name != name)
			return;
		if (tag != "" && col.gameObject.tag != tag)
			return;
		onColliderStay.Invoke();
	}
	
	void OnCollisionExit (Collision col) {
		if (name != "" && col.gameObject.name != name)
			return;
		if (tag != "" && col.gameObject.tag != tag)
			return;
		onColliderExit.Invoke();
	}
	
}
