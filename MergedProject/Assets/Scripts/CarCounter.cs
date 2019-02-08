using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarCounter : MonoBehaviour {
	
	[System.Serializable]
	public enum Unit { Meters, Feet, CarLengths };
	
	[System.Serializable]
	public struct RadioCall {
		public float distance;
		public AudioClip distanceCall;
	}
	
	[Header("Game Logic")]
	public InteractionHandler.InvokableState OnMissedHalfwayStop;
	public InteractionHandler.InvokableState OnMissedCall;
	
	[Header ("Distance Measurement")]
	//[Tooltip ("If this is less than 0, it won't set on Start")]
	//public float initialDistance = -1;
	public Transform rearKnuckle;
	public Transform appoachingPoint;
	//public Unit unitOfMeasurement;
	
	[Header ("Calling before Halfway")]
	public bool ignoreHalfWay;
	//public InteractionHandler.InvokableState onHalfWayPoint;
	
	[Header ("Calls by Distance")]
	public RadioCall[] carLengthCalls;
	public RadioCall[] feetLengthCalls;
	public AudioSource audio;
	
	[Header ("Debug")]
	public Text distDebug;
	public Image distDebugBackground;
	public Color yellow;
	public Color red;
	public float flashRate = 0.5f;
	private bool needsCall;
	private bool lastNeedCall;
	private bool wasCalled;
	private Color currentColor;
	private float flash_t;
	
	//private float distanceInitial;
	//private float distanceRemaining;
	//private Vector3 lastPos;
	//private bool halfwayMet;
	
	const float CarLength = 11.66238f; // meters
	const float FlexibleHalfWidthLarge = 6.0f; // feet
	const float FlexibleHalfWidthMedium = 3.0f; // feet
	const float FlexibleHalfWidthSmall = 1.5f; // feet
	const float MetersToFeet = 3.28084f;
	
	public float DistanceInMeters {get; private set;}
	
	public float DistanceInFeet
	{
		get
		{
			return DistanceInMeters * MetersToFeet;
		}
	}
	
	public float DistanceInCars
	{
		get
		{
			return DistanceInMeters / CarLength;
		}
	}
	
	private float lastCall;
	private bool usingFeetCalls;
	private int activeCallIndex;
	private bool isCoupled;
	
	void Start () {
		//lastPos = transform.position;
		
		System.Array.Sort<RadioCall>(carLengthCalls, (x,y) => x.distance.CompareTo(y.distance));
		System.Array.Sort<RadioCall>(feetLengthCalls, (x,y) => x.distance.CompareTo(y.distance));
		
		ResetDistanceCalling();
		/*
		
		if (initialDistance >= 0) {
			switch (unitOfMeasurement) {
				case Unit.Meters:
					InitialDistance = initialDistance;
					break;
				case Unit.Feet:
					DistanceInFeet = initialDistance;
					break;
				case Unit.CarLengths:
					DistanceInCars = initialDistance;
					break;
			}
		}
		*/
	}
	
	void Update () {
		//distanceRemaining -= Mathf.Abs(Vector3.Distance(transform.position, lastPos));
		//lastPos = transform.position;
		DistanceInMeters = Vector3.Distance(rearKnuckle.position, appoachingPoint.position);
		flash_t += Time.deltaTime;
		
		if (distDebug)
			distDebug.text = "Meters: " + DistanceInMeters.ToString("0.00") + "m | Cars: " + DistanceInCars.ToString("0.00")  + " | Feet: " + DistanceInFeet.ToString("0.00")  + "ft | Last Call: " + lastCall.ToString("0.00") + "ft";
		
		needsCall = false;
		wasCalled = false;
		
		// remember calls are made in feet
		if(usingFeetCalls)
		{
			if(isCoupled)
				return;
			float targetInFeet = feetLengthCalls[activeCallIndex].distance;
			float currentFlex = FlexibleHalfWidthMedium;
			if(DistanceInFeet < 10)
				currentFlex = FlexibleHalfWidthSmall;
			
			// Needs Call Check
			if(DistanceInFeet < targetInFeet + currentFlex)
				needsCall = true;
			if(lastCall < targetInFeet + currentFlex)
				wasCalled = true;
			
			// Call Was Made Check
			if(DistanceInFeet < targetInFeet - currentFlex)
			{
				if(lastCall > targetInFeet + currentFlex) // call is missed
				{
					MissedCall();
				}
				else // call is accepted
				{
					activeCallIndex--;
					if(activeCallIndex < 0)
					{
						activeCallIndex = 0;
						isCoupled = true;
						Debug.Log("Coupled");
					}
					else
					{
						Debug.Log("Next Distance: " + feetLengthCalls[activeCallIndex].distance);
					}
				}
			}
		}
		else
		{
			float targetInFeet = carLengthCalls[activeCallIndex].distance * CarLength * MetersToFeet;
			
			// Needs Call Check
			if(DistanceInFeet < targetInFeet + FlexibleHalfWidthLarge)
				needsCall = true;
			if(lastCall < targetInFeet + FlexibleHalfWidthLarge)
				wasCalled = true;
			
			// Call Was Made Check
			if(DistanceInFeet < targetInFeet - FlexibleHalfWidthLarge)
			{
				if(lastCall > targetInFeet + FlexibleHalfWidthLarge) // Call is missed
				{
					MissedCall();
				}
				else // Call is accepted
				{
					activeCallIndex--;
					if(activeCallIndex < 0) // Transition to fruit by the foot
					{
						usingFeetCalls = true;
						activeCallIndex = feetLengthCalls.Length-1;
						Debug.Log("Next Distance: " + feetLengthCalls[activeCallIndex].distance);
					}
					else
					{
						Debug.Log("Next Distance: " + carLengthCalls[activeCallIndex].distance * CarLength * MetersToFeet);
					}
				}
			}
		}
		
		//if(DistanceInCars > 1)
		//	ignoreHalfWay = false;
		
		if(DistanceInFeet < 26 && !ignoreHalfWay)
			MissedHalfwayStop();
			
		
		if(needsCall && !wasCalled)
			currentColor = red;
		else
			currentColor = yellow;
		
		if(lastNeedCall != needsCall)
		{
			lastNeedCall = needsCall;
			flash_t = 0.0f;
		}
		
		distDebugBackground.color = Color.Lerp(yellow, currentColor, Mathf.PingPong(Time.time, flashRate) / flashRate);
	}
	
	public void CheckHalfwayStop()
	{
		//if(DistanceInCars < 1)
		//	ignoreHalfWay = true;
	}
	
	public void ReadyToCouple(bool state) {
		ignoreHalfWay = state;
	}
	
	public void MissedHalfwayStop()
	{
		OnMissedHalfwayStop.Invoke();
	}
	
	public void MissedCall()
	{
		//Debug.LogAssertion("You Suck Lol");
		OnMissedCall.Invoke();
	}
	
	public void SetRearKnuckle(Transform t)
	{
		rearKnuckle = t;
	}
	
	public void SetApproach(Transform t)
	{
		appoachingPoint = t;
	}
	
	public void ResetDistanceCalling()
	{
		DistanceInMeters = Vector3.Distance(rearKnuckle.position, appoachingPoint.position);
		
		isCoupled = false;
		usingFeetCalls = false;
		activeCallIndex = carLengthCalls.Length-1;
		
		for(int i = 0; i < carLengthCalls.Length; i++)
		{
			if(carLengthCalls[i].distance * CarLength < DistanceInMeters)
			{
				activeCallIndex = i;
			}
		}
		
		Debug.Log("Next Distance: " + carLengthCalls[activeCallIndex].distance * CarLength * MetersToFeet);
	}
	
	public void CallDistance () {
		lastCall = DistanceInFeet;
		
		/*
		if (!audio) {
			Debug.LogWarning("Missing AudioSource for distance calls!");
			return;
		}
		*/
		
		if (audio.isPlaying)
			return;
		
		/*
		if (DistanceInFeet > 23f) {
			DoCall(carLengthCalls);
		} else {
			DoCall(feetLengthCalls, true);
		}
		*/
	}
	
	/*
	void DoCall (RadioCall[] calls, bool measureInFeet = false) {
		bool distanceCalled = false;
		for (int i = 0; i < calls.Length-1; i++) {
			if (Average(calls[i].distance, calls[i+1].distance) >= (measureInFeet ? DistanceInFeet : DistanceInCars)) { // Counting from smallest to largest because speed matters when the distances are smaller
				distanceCalled = true;
				if (calls[i].distanceCall)
					audio.clip = calls[i].distanceCall;
				else
					Debug.LogAssertion("Missing distance call for " + (measureInFeet ? "feet" : "car") + " length " + calls[i].distance + " at array index " + i);
				audio.Play();
				break;
			}
		}
		if (!distanceCalled) { // Unless math breaks, this means we are at the largest distance (Last in the array)
			if (calls[calls.Length-1].distanceCall)
				audio.clip = calls[calls.Length-1].distanceCall;
			else
				Debug.LogAssertion("Missing distance call for " + (measureInFeet ? "feet" : "car") + " length " + calls[calls.Length-1].distance + " at array index " + (calls.Length-1));
			audio.Play();
		}
	}
	*/
	
	float Average(float a, float b) {
		return (a+b)/2f;
	}
	
	/*
	public float InitialDistance {
		get { return distanceInitial; }
		set {
			distanceInitial = value;
			distanceRemaining = value;
			if (DistanceInFeet <= 5)
				ignoreHalfWay = true;
			else
				ignoreHalfWay = false;
		}
	}
	
	public float DistanceInFeet {
		get { return distanceRemaining*3.28084f; }
		set {
			InitialDistance = value/3.28084f;
			if (DistanceInFeet <= 5)
				ignoreHalfWay = true;
			else
				ignoreHalfWay = false;
		}
	}
	
	public float DistanceInCars {
		get { return (distanceRemaining*3.28084f)/38f; }
		set {
			InitialDistance = (value*38f)/3.28084f;
			if (DistanceInFeet <= 5)
				ignoreHalfWay = true;
			else
				ignoreHalfWay = false;
		}
	}
	*/
}
