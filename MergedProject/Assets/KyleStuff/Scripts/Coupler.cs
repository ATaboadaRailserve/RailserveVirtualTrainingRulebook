using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// Should use inheritance for Switcher and Coupler here, but demo is in 3 days
public class Coupler : MonoBehaviour {

	Animator anim;
	public float runSpeed = 2.0f;
	public bool isActive = false;
	//private bool running = false;
	public Transform warningChecker;
	public Transform warningZone;
	public bool redZoneCalled;
	public Button redZoneButton;
	private LocoScript locoScript;


	private Collider lastKnownCollider;
	private bool inTriggerZone;

	Vector3 up, down, left, right;
	float radsToDegs;
	float dfs = 0;
	float h, v, vAbs, hAbs, maxInput;
	
	// Use this for initialization
	void Start () {
		isActive = false;
		anim = GetComponent<Animator>();
		anim.SetFloat("Speed", 0.0f);
		anim.SetFloat("Direction", 0.0f);
		//anim.speed = 1.0f;
		radsToDegs = 180.0f/(float)Mathf.PI;
		redZoneCalled = false;
		inTriggerZone = false;
		locoScript = FindObjectOfType<LocoScript>();
	}

	public void CreateWarningZone() {
		// Create a new warning zone and start it blinkin
		Transform o = Instantiate(warningZone, lastKnownCollider.transform.position, lastKnownCollider.transform.rotation) as Transform;
		o.transform.Rotate(0, 90, 0);
		o.GetComponent<BlinkScript>().StartDasBlinkins();
	}

	public void ToggleRedZone() {
		redZoneCalled = !redZoneCalled;
		if (redZoneCalled) {
			redZoneButton.GetComponent<Image>().color = Color.red;
			redZoneButton.GetComponentInChildren<Text>().text = "Clear Red Zone Call";
			locoScript.inRedZone = true;
		}
		else {
			redZoneButton.GetComponent<Image>().color = Color.yellow;
			redZoneButton.GetComponentInChildren<Text>().text = "Call a Red Zone";
			locoScript.inRedZone = false;
		}
	}

	public void PlayCouplingAnim() {
		StartCoroutine(RunFixedCouplingAnim());
	}

	public IEnumerator RunFixedCouplingAnim() {
		anim.SetBool("IsCoupling", true);
		yield return new WaitForSeconds(1f); 
		anim.SetBool("IsCoupling", false); 
	}

	void OnTriggerStay(Collider other) {

		// Set the thing he collided with as the last known collision (for creating the warning zone)

		//Debug.Log(other.tag);
		// The only collision (right now) is with the SmartTankers
		if (other.tag != "CouplerRange")  {
			return;
		}
		lastKnownCollider = other;
		inTriggerZone = true;
		if (!redZoneCalled) {
			// Disable this guy
			isActive = false;
			// Set this guy as the control return from meeting the master (Rick)
			warningChecker.GetComponent<WarningChecker>().SetReturnCharacter(this.gameObject.transform);
			// Switch cameras to go to Rick for the warning
			warningChecker.GetComponent<WarningChecker>().SwitchCameras();
		}
	}
	
	void OnTriggerExit (Collider other) {
		if (other.tag != "CouplerRange")  {
			return;
		}
		lastKnownCollider = null;
		inTriggerZone = false;
	}

	// lastKnownCollider must be assigned a value
	public void UncoupleTriggeredCars() {
		Debug.Log("Coupler::Trying to uncouple");
		if ((redZoneCalled)&&(inTriggerZone)) {
			//Debug.Log("Coupler::In uncoupling state");
			lastKnownCollider.GetComponent<CouplerRange>().UncoupleFromTrain();
		}
	}
	
	public void CoupleTiggeredCars () {
		Debug.Log("Coupler::Trying to couple");
		if ((redZoneCalled)&&(inTriggerZone)) {
			//Debug.Log("Coupler::In coupling state");
			locoScript.CompleteAirSystem();
		}
	}
	
	void FixedUpdate () {
		if (!isActive) {
			anim.SetFloat("Speed", 0);
			return;
		}

		h = Input.GetAxis("Horizontal");
		v = Input.GetAxis("Vertical");
		vAbs = Mathf.Abs(v);
		hAbs = Mathf.Abs(h);
		maxInput = Mathf.Max(vAbs, hAbs);

		anim.SetFloat("Speed", maxInput);
		if ((vAbs>0.1)||(hAbs>0.1f)) {
			dfs = Mathf.Atan2(-v, h);
			Vector3 lookAtTarget = Quaternion.AngleAxis(dfs*radsToDegs, Vector3.up)*Vector3.forward + transform.position;
			//Debug.Log("D is " + lookAtTarget);
			transform.LookAt(lookAtTarget);
			transform.Translate(Vector3.forward*Time.deltaTime*maxInput*runSpeed);

		}
		// Total hack.  This needs to be handled by some kind of event.
		//anim.SetBool("IsCoupling", false);
	}
}
