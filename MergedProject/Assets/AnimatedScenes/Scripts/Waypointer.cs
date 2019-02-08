using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Waypointer : MonoBehaviour {
	
	[System.Serializable]
	public struct Waypoint {
		public Transform startPoint;
		public Transform endPoint;
		public Vector2 timeFrame;
		public AnimationCurve transitionCurve;
		public bool fixAngleOverride;
        public bool quaternionOverride;
    }
	
	public AnimationScrubber scrubber;
	public Waypoint[] waypoints;
	public bool dontLerpBetweenGappedWaypoints;  // Whether or not to automatically lerp between waypoints that have empty time between them.
	public bool useLocalPosition;
	
	private float timer;
	private float deltaTime;
	private Vector3 workerVector;
	private bool found;
	private List<Waypoint> sortedWaypoints;
	
	void Start () {
		if (waypoints.Length <= 0)
			return;
		// If the waypoints are out of order, ORDER THEM
		// If there are gaps between the times, UN GAP THEM!
		
		// Convert to a List because it'll be easier to work with.
		sortedWaypoints = new List<Waypoint>();
		for (int i = 0; i < waypoints.Length; i++) {
			sortedWaypoints.Add(waypoints[i]);
		}
		
		// Sort by start time.
		for (int i = 1; i < sortedWaypoints.Count; i++) {
			for (int j = i; j > 0; j--) {
				if (sortedWaypoints[j-1].timeFrame.x > sortedWaypoints[j].timeFrame.x) {
					Waypoint tempPoint1 = sortedWaypoints[j];
					Waypoint tempPoint2 = sortedWaypoints[j-1];
					sortedWaypoints[j-1] = tempPoint1;
					sortedWaypoints[j] = tempPoint2;
				}
			}
		}
		
		// If the first sorted waypoint doesn't start at Zero or less, make one that does.
		if (sortedWaypoints[0].timeFrame.x > 0) {
			GameObject origin = new GameObject();
			if (useLocalPosition) {
				origin.transform.parent = transform.parent;
				origin.transform.localPosition = transform.localPosition;
			} else
				origin.transform.position = transform.position;
			origin.transform.localEulerAngles = transform.localEulerAngles;
			Waypoint tempPoint = new Waypoint();
			tempPoint.startPoint = origin.transform;
			
			// If we're not lerping, make the start and end of the augmented waypoint the end of the last point (AKA the Origin for this particular one)
			if (dontLerpBetweenGappedWaypoints)
				tempPoint.endPoint = origin.transform;
			else // Else make the end point the start point of the next waypoint
				tempPoint.endPoint = sortedWaypoints[0].startPoint;
			
			tempPoint.timeFrame = new Vector2(0, sortedWaypoints[0].timeFrame.x);
			tempPoint.transitionCurve = AnimationCurve.Linear(0,0,1,1);
			sortedWaypoints.Insert(0, tempPoint);
		}
		
		// Fill in the gaps.  For each waypoint, if there's a gap between it and the last one, make a waypoint between them.
		for (int i = 1; i < sortedWaypoints.Count; i++) {
			if (sortedWaypoints[i-1].timeFrame.y < sortedWaypoints[i].timeFrame.x) {
				Waypoint tempPoint = new Waypoint();
				tempPoint.startPoint = sortedWaypoints[i-1].endPoint;
				
				// Again, if we're not lerping then make the start and end of the augmented waypoint the endpoint of the last one
				if (dontLerpBetweenGappedWaypoints)
					tempPoint.endPoint = sortedWaypoints[i-1].endPoint;
				else // Else make the end point be the start point of the next waypoint
					tempPoint.endPoint = sortedWaypoints[i].startPoint;
				
				tempPoint.timeFrame = new Vector2(sortedWaypoints[i-1].timeFrame.y, sortedWaypoints[i].timeFrame.x);
				tempPoint.transitionCurve = AnimationCurve.Linear(0,0,1,1);
				sortedWaypoints.Insert(i, tempPoint);
			}
		}
	}
	
	void Update () {
		if (waypoints.Length <= 0)
			return;
		found = false;
		timer = scrubber.GetTime();
		for (int i = 0; i < sortedWaypoints.Count; i++) {
			if (timer >= sortedWaypoints[i].timeFrame.x && timer <= sortedWaypoints[i].timeFrame.y) {
				deltaTime = sortedWaypoints[i].timeFrame.y - sortedWaypoints[i].timeFrame.x;
				
				if (useLocalPosition)
					transform.localPosition = Vector3.Lerp(sortedWaypoints[i].startPoint.localPosition, sortedWaypoints[i].endPoint.localPosition, sortedWaypoints[i].transitionCurve.Evaluate(Mathf.Clamp((timer-sortedWaypoints[i].timeFrame.x)/deltaTime, 0, 1)));
				else
					transform.position = Vector3.Lerp(sortedWaypoints[i].startPoint.position, sortedWaypoints[i].endPoint.position, sortedWaypoints[i].transitionCurve.Evaluate(Mathf.Clamp((timer-sortedWaypoints[i].timeFrame.x)/deltaTime, 0, 1)));
				if (sortedWaypoints[i].quaternionOverride)
                {
                    transform.rotation = Quaternion.Lerp(sortedWaypoints[i].startPoint.rotation, sortedWaypoints[i].endPoint.rotation, sortedWaypoints[i].transitionCurve.Evaluate(Mathf.Clamp((timer - sortedWaypoints[i].timeFrame.x) / deltaTime, 0, 1)));
                }
                else
                {
                    if (sortedWaypoints[i].fixAngleOverride)
                    {
                        transform.localEulerAngles = Vector3.Lerp(sortedWaypoints[i].startPoint.localEulerAngles, sortedWaypoints[i].endPoint.localEulerAngles, sortedWaypoints[i].transitionCurve.Evaluate(Mathf.Clamp((timer - sortedWaypoints[i].timeFrame.x) / deltaTime, 0, 1)));
                    }
                    else {
                        transform.localEulerAngles = Vector3.Lerp(FixRotation(sortedWaypoints[i].startPoint.localEulerAngles), FixRotation(sortedWaypoints[i].endPoint.localEulerAngles), sortedWaypoints[i].transitionCurve.Evaluate(Mathf.Clamp((timer - sortedWaypoints[i].timeFrame.x) / deltaTime, 0, 1)));
                    }
                }
				found = true;
				break;
			}
		}
		if (!found) {
			int i = sortedWaypoints.Count-1;
			if (useLocalPosition)
				transform.localPosition = Vector3.Lerp(sortedWaypoints[i].startPoint.localPosition, sortedWaypoints[i].endPoint.localPosition, sortedWaypoints[i].transitionCurve.Evaluate(Mathf.Clamp((timer-sortedWaypoints[i].timeFrame.x)/deltaTime, 0, 1)));
			else
				transform.position = Vector3.Lerp(sortedWaypoints[i].startPoint.position, sortedWaypoints[i].endPoint.position, sortedWaypoints[i].transitionCurve.Evaluate(Mathf.Clamp((timer-sortedWaypoints[i].timeFrame.x)/deltaTime, 0, 1)));
			transform.localEulerAngles = Vector3.Lerp(FixRotation(sortedWaypoints[i].startPoint.localEulerAngles), FixRotation(sortedWaypoints[i].endPoint.localEulerAngles), sortedWaypoints[i].transitionCurve.Evaluate(Mathf.Clamp((timer-sortedWaypoints[i].timeFrame.x)/deltaTime, 0, 1)));
		}
	}
	
	Vector3 FixRotation (Vector3 angles) {
		Vector3 worker = angles;
		if (worker.x > 180f)
			worker.x -= 360f;
		if (worker.y > 180f)
			worker.y -= 360f;
		if (worker.z > 180f)
			worker.z -= 360f;
		return worker;
	}
}