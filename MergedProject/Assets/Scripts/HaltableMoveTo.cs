using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaltableMoveTo : MonoBehaviour {
	
	[System.Serializable]
	public struct Waypoint {
		public Transform start;
		public Transform end;
		public bool doRotation;
		public bool doScale;
		public float timeToMove;
		public AnimationCurve movementCurve;
		public InteractionHandler.InvokableState onStartMove;
		public InteractionHandler.InvokableState onEndMove;
		public InteractionHandler.InvokableState onForcedEndMove;
	}
	
	public bool moveOnStart;
	public Waypoint[] waypoints;
	public AnimationCurve forceEndCurve;
	public bool inertiaIsRotation;
	
	private int index = 0;
	private float timer;
	private IEnumerator movement;
	private Vector3 inertia;
	private bool haltMove;
	
	void Start () {
		if (moveOnStart) {
			MoveToIndex(0);
		}
	}
	
	public void MoveToIndex (int newIndex) {
		index = newIndex;
		if (movement != null) {
			StopCoroutine(movement);
		}
		movement = DoMove();
		StartCoroutine(movement);
	}
	
	IEnumerator DoMove () {
		timer = 0;
		waypoints[index].onStartMove.Invoke();
		while (timer < 1 && !haltMove) {
			if (inertiaIsRotation) {
				inertia = transform.localEulerAngles;
			} else {
				inertia = transform.position;
			}
			
			timer += Time.deltaTime / waypoints[index].timeToMove;
			transform.position = Vector3.Lerp(waypoints[index].start.position, waypoints[index].end.position, Mathf.Clamp(waypoints[index].movementCurve.Evaluate(timer), 0, 1));
			if (waypoints[index].doRotation)
				transform.rotation = Quaternion.Lerp(waypoints[index].start.rotation, waypoints[index].end.rotation, Mathf.Clamp(waypoints[index].movementCurve.Evaluate(timer), 0, 1));
			if (waypoints[index].doScale)
				transform.localScale = Vector3.Lerp(waypoints[index].start.localScale, waypoints[index].end.localScale, Mathf.Clamp(waypoints[index].movementCurve.Evaluate(timer), 0, 1));
			
			if (inertiaIsRotation) {
				inertia = transform.localEulerAngles - inertia;
			} else {
				inertia = transform.position - inertia;
			}
			
			yield return null;
		}
		if (haltMove) {
			timer = 0;
			while (timer < 1) {
				timer += Time.deltaTime;
				if (inertiaIsRotation) {
					transform.localEulerAngles += inertia * forceEndCurve.Evaluate(timer);
				} else {
					transform.position += inertia * forceEndCurve.Evaluate(timer);
				}
				yield return null;
			}
			waypoints[index].onForcedEndMove.Invoke();
			haltMove = false;
		} else {
			transform.position = waypoints[index].end.position;
			if (waypoints[index].doRotation)
				transform.rotation = waypoints[index].end.rotation;
				if (waypoints[index].doScale)
					transform.localScale = waypoints[index].end.localScale;
			waypoints[index].onEndMove.Invoke();
		}
		movement = null;
	}
	
	public void HaltMove () {
		haltMove = true;
	}
}
