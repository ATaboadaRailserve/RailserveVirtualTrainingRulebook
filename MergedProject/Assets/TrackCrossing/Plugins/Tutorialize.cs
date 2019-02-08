using UnityEngine;
using System.Collections;

public class Tutorialize : MonoBehaviour {
	
	[System.Serializable]
	public struct Event {
		public GameObject target;
		
		[System.Serializable]
		public enum Type { OnMouseDown, OnMouseUp, OnCollisionEnter, OnCollisionExit, OnCollisionStay, OnTriggerEnter, OnTriggerExit, OnTriggerStay, OnObjectAngle }
		public Type type;
	}
	
	[System.Serializable]
	public struct Step {
		public bool active;
		public Event correctEvent;
		public Event inccorectEvent;
		public int correctStepIndex;
		public int incorrectStepIndex;
	}
	
	public Step[] steps;
	
	void Update () {
		
	}
}
