using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityToggle : MonoBehaviour {
	
	[Header("Trigger Interactions")]
	public InteractionHandler.InvokableState onTriggerEnter;
	public InteractionHandler.InvokableState onTriggerStay;
	public InteractionHandler.InvokableState onTriggerExit;
	public string[] tagWhiteList;
	public string[] tagBlackList;
	
	void OnTriggerEnter (Collider col) {
		if (tagWhiteList.Length != 0) {
			foreach (string t in tagWhiteList) {
				if (col.gameObject.tag == t) {
					onTriggerEnter.Invoke();
				}
			}
		} else {
			foreach (string t in tagBlackList) {
				if (col.gameObject.tag == t)
					return;
			}
			onTriggerEnter.Invoke();
		}
	}
	void OnTriggerStay (Collider col) {
		if (tagWhiteList.Length != 0) {
			foreach (string t in tagWhiteList) {
				if (col.gameObject.tag == t) {
					onTriggerStay.Invoke();
				}
			}
		} else {
			foreach (string t in tagBlackList) {
				if (col.gameObject.tag == t)
					return;
			}
			onTriggerStay.Invoke();
		}
	}
	void OnTriggerExit (Collider col) {
		if (tagWhiteList.Length != 0) {
			foreach (string t in tagWhiteList) {
				if (col.gameObject.tag == t) {
					onTriggerExit.Invoke();
				}
			}
		} else {
			foreach (string t in tagBlackList) {
				if (col.gameObject.tag == t)
					return;
			}
			onTriggerExit.Invoke();
		}
	}
}
