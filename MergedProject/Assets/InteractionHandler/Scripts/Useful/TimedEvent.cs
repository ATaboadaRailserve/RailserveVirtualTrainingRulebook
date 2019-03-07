using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedEvent : MonoBehaviour {
	
	public float defaultTime;
	public InteractionHandler.InvokableState onTriggered;
	
	bool canceled = false;
	
	public void TriggerEvent () {
		canceled = false;
		StartCoroutine(Countdown(defaultTime));
	}
	
	public void TriggerEvent (float time) {
		canceled = false;
		StartCoroutine(Countdown(time));
	}
	
	public void CancelEvent () {
		canceled = true;
	}
	
	IEnumerator Countdown (float time) {
		yield return new WaitForSeconds(time);
		if (!canceled) {
			onTriggered.Invoke();
		}
		canceled = false;
	}
}
