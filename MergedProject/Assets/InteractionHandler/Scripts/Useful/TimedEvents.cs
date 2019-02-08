using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedEvents : MonoBehaviour {

	[System.Serializable]
	public struct TimedEvent {
		public float triggerTime;
		public InteractionHandler.InvokableState onTriggered;
	}
	public TimedEvent[] timedEventList;

	public void TriggerEvent (int index) {
		StartCoroutine(Countdown(timedEventList[index].triggerTime, index));
	}		

		IEnumerator Countdown (float time, int index) {
		yield return new WaitForSeconds(time);
		timedEventList[index].onTriggered.Invoke();
	}
}
