using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedEvent : MonoBehaviour {
	
	public float defaultTime;
	public InteractionHandler.InvokableState onTriggered;
	
	public void TriggerEvent () {
		StartCoroutine(Countdown(defaultTime));
	}
	
	public void TriggerEvent (float time) {
		StartCoroutine(Countdown(time));
	}
	
	IEnumerator Countdown (float time) {
		yield return new WaitForSeconds(time);
		onTriggered.Invoke();
	}
}
