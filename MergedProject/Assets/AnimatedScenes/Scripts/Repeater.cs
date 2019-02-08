using UnityEngine;
using System.Collections;

public class Repeater : MonoBehaviour {
	
	public float timeToMove;
	public Vector3 targetPosition;
	public Vector3 degreesPerSecond;
	public AnimationScrubber scrubber;
	
	private float timer;
	private float deltaTime;
	private Vector3 origin;
	
	void Start () {
		timer = 0;
		deltaTime = 0;
		origin = transform.position;
	}
	
	void Update () {
		timer += scrubber.GetTime() - deltaTime;
		
		transform.position = Vector3.Lerp (origin, targetPosition + origin, (timer/timeToMove)%1);
		transform.localEulerAngles += degreesPerSecond * (scrubber.GetTime() - deltaTime);
		
		deltaTime = scrubber.GetTime();
	}
	
	void OnDrawGizmos () {
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, targetPosition + transform.position);
	}
}
