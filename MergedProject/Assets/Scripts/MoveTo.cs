using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTo : MonoBehaviour {
	
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
	}
	
	public bool moveOnStart;
	public Waypoint[] waypoints;
	
	private int index = 0;
	private float timer;
	private IEnumerator movement;
	
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
		while (timer < 1) {
			timer += Time.deltaTime / waypoints[index].timeToMove;
			transform.position = Vector3.Lerp(waypoints[index].start.position, waypoints[index].end.position, Mathf.Clamp(waypoints[index].movementCurve.Evaluate(timer), 0, 1));
			if (waypoints[index].doRotation)
				transform.rotation = Quaternion.Lerp(waypoints[index].start.rotation, waypoints[index].end.rotation, Mathf.Clamp(waypoints[index].movementCurve.Evaluate(timer), 0, 1));
			if (waypoints[index].doScale)
				transform.localScale = Vector3.Lerp(waypoints[index].start.localScale, waypoints[index].end.localScale, Mathf.Clamp(waypoints[index].movementCurve.Evaluate(timer), 0, 1));
			yield return null;
		}
		transform.position = waypoints[index].end.position;
		if (waypoints[index].doRotation)
			transform.rotation = waypoints[index].end.rotation;
			if (waypoints[index].doScale)
				transform.localScale = waypoints[index].end.localScale;
		waypoints[index].onEndMove.Invoke();
		movement = null;
	}
}
