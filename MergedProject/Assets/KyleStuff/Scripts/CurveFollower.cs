/// <summary>
/// Track follower v0.1
/// Author: Jeff Chastine
/// Summary:  This script enables an object to follow a Bezier curve.  Each piece of
///  track in the scene contains one or more curves, only one of which is active. For 
///  example, a straight piece of track will likely have only one curve.  A split track
///  will have two (one straight and one curved).  In general, the game object that this
///  script is attached to is bound to a single piece of track, which it queries to find
///  which Bezier curve is currently active.  
/// 
///  For this current build, the wheel sets use this script.  The script depends on
///  Bezier.cs but is called from SmartTanker.cs
/// 
/// Notes: the execution order is Bezier, TrackFollower, SmartTanker
/// </summary>
using UnityEngine;
using System.Collections;

public class CurveFollower : MonoBehaviour {

	// The initial track piece the wheel set is bound to.  
	// This is initialized from SmartTanker script
	[HideInInspector]
	public Transform startingBezier;
	// How far we are along the curve in units.  This will be converted to t={0,1} later.
	[HideInInspector]
	public float distanceAlongCurve;


	// The active Bezier in the current track
	public BezierScript currentBezier;
	// The total length of the curve.  Used to figure out where we are on the curve
	private float lengthOfCurve;
	// Ubiquitous t={0,1}
	private float t = 0.5f; 
	// Current rotation of the wheels
	public float rotation;
	// Used to set the rotation
	private Vector3 lookAtTarget;  
	// Current location of the wheels
	private Vector4 currentLocation;
	
	private bool initialized;

	// Other
	private static float radsToDegs, PIover2;
	public bool printOut;

	// Use this for initialization
	void Start () {

	}

	// Must be called from SmartTankerScript
	public void Initialize() {

		// Ask the track for its active curve
		currentBezier = startingBezier.GetComponent<BezierScript>();

		// Get the length of the current curve
		lengthOfCurve = currentBezier.lengthOfCurve;

		// Math
		radsToDegs = 180.0f/(float)Mathf.PI;
		PIover2 = Mathf.PI/2.0f;
		initialized = true;
	}
	
	

	// Non-realtime
	// Get the active Bezier curve from the current Bezier set.  Though this is called
	// from Initialize(), it's most likely called when transitioning to a new
	// piece of track
	/*BezierScript GetActiveBezier() {
		return currentBezierSet.GetComponent<BezierSwitcher>().GetActiveBezierScript();
	} // function
	*/

	/// <summary>
	/// Non-realtime.  Called from Push() below.  Updates the current Bezier and
	///  length of curve.
	/// </summary>
	void SetupNextCurve() {
		//currentBezier = GetActiveBezier();
		lengthOfCurve = currentBezier.lengthOfCurve;
	}

	/// <summary>
	/// Critical, realtime.  Pushes the car along the Bezier by a small amount.
	///  Is resposible for marking the car for derailment as well 
	/// </summary>
	/// <param name="amount">Amount.</param>
	public void Push(float amount) {
		
		if (initialized){
			// If we've reached the of a track and are now null, derail
			if (currentBezier == null) {
				Debug.Log("TrackFollower::Push() NULL TRACK!");
				this.gameObject.GetComponentInParent<SmartTankerScript>().isDerailed = true;
				Debug.Log("t is " + t + " DAC is " +distanceAlongCurve);
				return;
			}

			// Move along the curve by a certain amount
			distanceAlongCurve += amount;

			// Normalize that to calculate t
			t=distanceAlongCurve/lengthOfCurve;

			// If we've now exceeded the range {0, 1} for t, then get either the
			// parent curve or child curve and attach to the next curve
			// CHILD
			if (t > 1.0f) {
				// Get the child Bezier either from a set or the actual curve
				currentBezier = currentBezier.GetChildCurve();
				SetupNextCurve();
				t = 0.0f;
				distanceAlongCurve = 0.0f;
			}
			// PARENT
			else if (t < 0.0f) {
				currentBezier = currentBezier.GetParentCurve();
				SetupNextCurve();
				t = 1.0f;
				distanceAlongCurve = lengthOfCurve;
			}

			// Retrieve the interpolated position and rotation of the wheel set
			currentLocation = currentBezier.FindPosFromDistance(distanceAlongCurve);
			rotation = currentLocation.w;
			rotation *= radsToDegs;
			// Set the orientation and position of the actual transform
			lookAtTarget = Quaternion.AngleAxis(rotation, Vector3.up)*Vector3.forward + transform.position;
			transform.localEulerAngles = new Vector3(0, rotation + 90, 0);
			//transform.LookAt(lookAtTarget);
			transform.position = currentLocation;

			if (printOut&&(amount == 0)) {
				Debug.Log("TrackFollower::EndOFPush(0)::"+this.transform.position);
			}
		}
	}
	
}
