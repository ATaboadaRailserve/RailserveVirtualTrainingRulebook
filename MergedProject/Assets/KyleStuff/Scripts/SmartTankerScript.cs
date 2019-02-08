/// <summary>
/// SmartTankerScript v0.1
/// Author: Jeff Chastine
/// Summary:  This scripts should be attached to a full car.  The car should have two
///  wheel sets as children, and those wheel sets should have a CurveFollower component.
///  The primary responsibility is to update the position/orientation of the car by pushing
///  the wheel sets.  It also recursively pushes the other cars.
/// 
///  This script also marks a car as part of the train (for pushing/pulling) and can derail
///  the car if the wheel distance exceeds a certain threshold.
/// 
/// Notes: the execution order is Bezier, CurveFollower, SmartTanker
/// </summary>
using UnityEngine;
using System.Collections;

public class SmartTankerScript : MonoBehaviour {

	public Transform innerEmpty;
	private AvatarChanger avatarScript; // For changing the loco's view
	public Transform startingBezier;
	public Transform frontWheels;
	public AudioClip couplingAudioClip;
	private AudioSource couplingAudio;

	private static int carCount = 0;
	private int carID = 0;

	public Transform rearWheels;
	public bool autoCalcDistAlongCurve = true;
	public float distanceAlongCurve;
	public float wheelDistance = 7;
	public Vector3 bodyOffset;
	
	public bool driverCar;
	public bool partOfTrain;
	public bool airBrakesFilled = false;
	public bool printOut;
	public bool rotate90 = false;
	public bool isDerailable = true;
	[HideInInspector]
	public bool isDerailed = false;
	private bool isInitialized = false;

	public float distanceBetweenWheels = 7;
	
	private Vector3 deltaPos;
	private float rotation;
	private float radsToDegs;
	private float PIover2;
	private CurveFollower frontScript;
	private CurveFollower rearScript;
	// The car that is connected to us.  This is recursive.
	public SmartTankerScript childCarScript;
	public SmartTankerScript parentCarScript;
	public TextToTexture plate;
	
	// Kyle Addatives
	public CouplerRange couplerRange;
	public bool isPulled;
	public IDController idController;
	public Collider[] notRigidBodyColliders;
	public LocoScript leaf;
	public Vector3 raycastOffset = Vector3.zero;
	
	void Start () {
		idController = GameObject.FindWithTag("IDController").GetComponent<IDController>();
		if (tag == "Leaf") {
			First();
		}
	}
	
	// Initialize the Leaf
	void First () {
		innerEmpty = GameObject.Find("InnerEmpty").transform;
		
		avatarScript = innerEmpty.GetComponent<AvatarChanger>();
		
		carID = ++carCount;
		couplingAudio = this.GetComponent<AudioSource>();
		if (!transform.parent || !transform.parent.parent || !transform.parent.parent.parent || transform.parent.parent.parent.gameObject.tag != "CarSet") {
			Initialize();
		}
	}
	
	void Initialize() {
		
		// Grab the Leaf's script
		leaf = (LocoScript)FindObjectOfType(typeof(LocoScript));

		// Grab the wheel scripts
		frontScript = frontWheels.GetComponent<CurveFollower>();
		rearScript = rearWheels.GetComponent<CurveFollower>();

		// Auto-calculate distance along the curve??
		if (autoCalcDistAlongCurve)
			distanceAlongCurve = startingBezier.GetComponent<BezierScript>().lengthOfCurve-(this.transform.position.z - startingBezier.GetComponent<BezierScript>().end.position.z);

		// Initialize the front and rear wheel sets
		frontScript.startingBezier = this.startingBezier;
		rearScript.startingBezier = this.startingBezier;
		frontScript.distanceAlongCurve = distanceAlongCurve+wheelDistance;
		rearScript.distanceAlongCurve = distanceAlongCurve-wheelDistance;
		frontScript.Initialize();
		rearScript.Initialize();

		// Set up variables to be used in pose calculations
		radsToDegs = 180.0f/(float)Mathf.PI;
		if (rotate90) {
			PIover2 = Mathf.PI/2.0f;
		}
		else {
			PIover2 = Mathf.PI;
		}
		childCarScript = null;

		// Update the position of the entire car
		if (printOut) {
			Debug.Log("SmartTanker::Init()::PushingfrontWith0");
		}
		frontScript.Push (0);
		if (printOut) {
			Debug.Log("SmartTanker::Init()::PushingrearWith0");
		}
		rearScript.Push (0);
		if (printOut) {
			Debug.Log("SmartTanker::Init()::PushingselfWith0");
		}
		Push (0);

		// The tolerable distance between wheels before derailing
		distanceBetweenWheels = wheelDistance*2+1;
		isInitialized = true;



		this.transform.position = (frontWheels.transform.position+rearWheels.transform.position)/2 +bodyOffset;
		//Debug.Log(Vector3.Distance(frontHack, this.transform.position));
		
		// Orientation
		deltaPos = (frontWheels.transform.position-rearWheels.transform.position);
		rotation = Mathf.Atan2(deltaPos.x, deltaPos.z)+PIover2;
		this.transform.rotation = Quaternion.AngleAxis(rotation*radsToDegs, Vector3.up);
		if (rotate90) {
			this.transform.Rotate(-90, 0, 90);
		}
	}

	/// <summary>
	/// Critical, realtime.
	/// </summary>
	/// <param name="amount">Amount.</param>
	// Move this, and all recursive children, by a fixed amount along their path
	public void Push(float amount) {
		if (!isInitialized) {
			return;
		}
		if (isDerailed) {
			return;
		}
		if (!partOfTrain) {
			return;
		}
		/*if (!airBrakesFilled) {
			return;
		}*/
		frontScript.Push(amount);	// Push the front wheels
		if (printOut) {
			Debug.Log("SmartTanker::Init()::PushingrearWith0");
		}
		rearScript.Push(amount);	// Push the back wheels

		// Calculate the position and orientation of the car
		if (printOut) {
			Debug.Log("SmartTanker::Push::"+this.transform.position);
		}

		this.transform.position = (frontWheels.transform.position+rearWheels.transform.position)/2 +bodyOffset;
		//Debug.Log(Vector3.Distance(frontHack, this.transform.position));

		// Orientation
		deltaPos = (frontWheels.transform.position-rearWheels.transform.position);
		rotation = Mathf.Atan2(deltaPos.x, deltaPos.z)+PIover2;
		this.transform.rotation = Quaternion.AngleAxis(rotation*radsToDegs, Vector3.up);
		if (rotate90) {
			this.transform.Rotate(-90, 0, 90);
		}


		// If it's time to derail
		if ((isDerailable)&&(distanceBetweenWheels != 0)) {
		
			if (Vector3.Distance(frontWheels.transform.position, rearWheels.transform.position) > distanceBetweenWheels)
			{
				leaf.velocity = 0;
				Debug.Log("Derailing");Debug.Log ("DBW is " + distanceBetweenWheels);
				Debug.Log (Vector3.Distance(frontWheels.transform.position, rearWheels.transform.position));
				partOfTrain = false;
				airBrakesFilled = false;
				isDerailed = true;
				frontWheels.transform.parent = this.transform;
				rearWheels.transform.parent = this.transform;
				foreach (Collider c in notRigidBodyColliders) {
					c.enabled = false;
				}
				GetComponent<Rigidbody>().GetComponent<Collider>().isTrigger = false;
				GetComponent<Rigidbody>().isKinematic = false;
				GetComponent<Rigidbody>().useGravity = true;
				GetComponent<Rigidbody>().AddTorque(0, 0, -1000);
				leaf.warner.Warn(2);
			}
		}

		// Recursively call Push on the child car
		if (childCarScript != null) {
			childCarScript.Push(amount);
		}
	}

	public int GetCarCount() {
		if (childCarScript == null) {
			return 1;
		}
		else return 1 + childCarScript.GetCarCount();
	}
	
	public bool CheckPulls () {
		if (childCarScript == null) {
			return isPulled;
		}
		if (isPulled || gameObject.tag == "Leaf") {
			return childCarScript.CheckPulls();
		}
		return false;
	}

	public void UncoupleFromTrain() {
		if (childCarScript != null) {
			childCarScript.partOfTrain = false;
			childCarScript.airBrakesFilled = false;
			childCarScript.UncoupleFromTrain();
			childCarScript = null;
		}
		leaf.CheckAir();
	}
	
	public bool CheckAir () {
		if (!airBrakesFilled)
			return false;
		
		if (childCarScript)
			return childCarScript.CheckAir();
		
		return airBrakesFilled;
	}

	public void FillAirBrakes() {
		this.airBrakesFilled = true;
		//Debug.Log("STS::Filling air brakes");
		if (childCarScript != null) {
			childCarScript.FillAirBrakes();
		}
		else {
			leaf.airSystemComplete = true;
		}
	}

	public void CoupleToTrain() {
		couplingAudio.PlayOneShot(couplingAudioClip);
		avatarScript.UpdateLocoView();
		partOfTrain = true;
		airBrakesFilled = false;
		Vector3 fwd = transform.TransformDirection(Vector3.up);
		RaycastHit hit;
        if (Physics.Raycast(transform.position + raycastOffset, fwd, out hit, 6.75f) && hit.collider.tag == "SmartTanker") {
			childCarScript = hit.collider.gameObject.GetComponent<SmartTankerScript>();
		}
		if (childCarScript != null)
			childCarScript.CoupleToTrain();
	}

	void OnTriggerEnter(Collider other) {
		// Potentially looking for a couple with another car
		if ((other.tag == "SmartTanker")&&(this.partOfTrain)&&
		    (!other.gameObject.GetComponent<SmartTankerScript>().partOfTrain)) {
			childCarScript = other.GetComponent<SmartTankerScript>();
			childCarScript.parentCarScript = this;
			other.gameObject.GetComponent<SmartTankerScript>().CoupleToTrain();
			leaf.airSystemComplete = false;
			leaf.CheckSpeed();
		}
	}
	
	void SetBezier (Transform bezier){
		startingBezier = bezier;
		Initialize();
	}
	
	// When the loco crosses the line, it'll check to be sure all cars are pull cars then it'll call this function for all cars in the train.
	public void AndAcrossTheLine () {
		// Detach from everything.
		if (childCarScript != null) {
			childCarScript.partOfTrain = false;
			childCarScript.airBrakesFilled = false;
			childCarScript.AndAcrossTheLine();
		}
		
		// Start the "animation" for a car disappearing.
		if (gameObject.tag != "Leaf")
			StartCoroutine("Winnar");
	}
	
	IEnumerator Winnar () {
		idController.ChockOneOff(plate.number);
		
		float spinner = 0;
		float scaler;
		
		// Kill the wheels
		frontWheels.localScale = new Vector3(0,0,0);
		rearWheels.localScale = new Vector3(0,0,0);
		
		// While there's still scale left
		while(transform.parent.localScale.x > 0f) {
			spinner += Time.deltaTime*15f;
			scaler = Time.deltaTime/2f;
			transform.localEulerAngles += new Vector3 (0,spinner,0);
			transform.localScale -= new Vector3(scaler,scaler,scaler);
			// Done with this frame, pause until next frame
			yield return null;
		}
		
		// Once the scale is zero, kill the parent object (The main Tank Car)
		Destroy(transform.parent.gameObject);
	}
	
	void OnDrawGizmos () {
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position + raycastOffset, transform.position + raycastOffset + transform.TransformDirection(Vector3.up)*6.75f);
	}
}
