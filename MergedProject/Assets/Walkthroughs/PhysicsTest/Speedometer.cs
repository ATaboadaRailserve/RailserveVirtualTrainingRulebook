using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour {

	public WheelPusher wheelPusher;

	[HideInInspector]
	public float currentSpeed; //approx speed based on distance traveled
	[HideInInspector]
	public float distanceTraveled; //approx distance traveled
	[HideInInspector]
	public float totalDistance;

	private Vector3 lastLocation; //location at last Clock()

	private const float mpsTOmph = 2.23694f;
	private const float mTOft = 3.28084f;
	private const float checksPerSecond = 4; //How many times the couroutine runs per second
	// Use this for initialization
	void Start () {
		StartCoroutine (Go ());
	}

	public void SetNewDistanceMeasure()
	{
		distanceTraveled = 0;
	}


	IEnumerator Go()
	{
		yield return new WaitForSeconds (1);
		lastLocation = gameObject.transform.position;
		StartCoroutine (Clock ());
	}

	IEnumerator Clock()
	{
		float d = Vector3.Distance (lastLocation, transform.position);
		//print ("Last: " + lastLocation + "    Current: " + transform.position);
		//if (wheelPusher.foward) {
			distanceTraveled += d*mTOft;
			totalDistance += d*mTOft;
		//} else {
			//distanceTraveled -= d*mTOft;
			//totalDistance -= d*mTOft;
		//}
		currentSpeed = d * mpsTOmph;
		lastLocation = transform.position;
		//print ("Speed: " + currentSpeed + "    Distance: " + distanceTraveled);
		yield return new WaitForSeconds (1 / checksPerSecond);
		StartCoroutine (Clock ());
	}
}
