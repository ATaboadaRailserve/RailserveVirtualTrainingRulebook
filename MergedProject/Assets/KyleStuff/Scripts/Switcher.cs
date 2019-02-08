using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// This is the script for the fellow switching the track

public class Switcher : MonoBehaviour {

	Animator anim;
	public float runSpeed = 2.0f;
	public bool isActive = false;
	//private bool running = false;
	public Transform warningChecker;
	public Transform warningZone;
	public Transform activeZonePrefab;
	private Transform activeZone;
	public Button switchTrackButton;
	private Vector3 offset;
	private bool inActiveZone;


	private Collider lastKnownCollider;

	Vector3 up, down, left, right;
	static float radsToDegs;
	float dfs = 0;
	float h, v, vAbs, hAbs, maxInput;


	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		anim.SetFloat("Speed", 0.0f);
		anim.SetFloat("Direction", 0.0f);
		//anim.speed = 1.0f;
		radsToDegs = 180.0f/(float)Mathf.PI;
		isActive = true;
		offset = new Vector3(0, -1.6f, 0);
		inActiveZone = false;
		Application.targetFrameRate = 60;
	}

	public void PlaySwitchingAnim() {

		if (inActiveZone) {
			// Face the switch
			// Look at not working.  Do it the old-fashioned way
			//transform.LookAt(other.transform.position+offset, this.up);
			Vector3 delta = lastKnownCollider.transform.position+offset - this.transform.position;
			float rotation = Mathf.Atan2(delta.z, -delta.x);
			this.transform.Rotate(0, rotation*radsToDegs, 0);
			lastKnownCollider.gameObject.GetComponent<TrackSwitcher>().SwitchTrack();
			StartCoroutine(RunFixedCouplingAnim());

		}
	}
	
	public IEnumerator RunFixedCouplingAnim() {
		anim.SetBool("IsSwitching", true);
		yield return new WaitForSeconds(1f); 
		anim.SetBool("IsSwitching", false); 
	}


	public void CreateWarningZone() {

		// Create a new warning zone and start it blinkin
		Transform o = Instantiate(warningZone, lastKnownCollider.transform.position, lastKnownCollider.transform.rotation) as Transform;
		o.transform.Rotate(0, 90, 0);
		o.GetComponent<BlinkScript>().StartDasBlinkins();
	}

	public void CreateActiveZone() {
		// Create a new active zone 
		activeZone = Instantiate(activeZonePrefab, lastKnownCollider.transform.position+offset, lastKnownCollider.transform.rotation) as Transform;
	}

	void OnTriggerExit(Collider other) {

		if (other.tag == "SplitTrack") {
			inActiveZone = false;
			switchTrackButton.GetComponent<Image>().color = Color.yellow;
			Destroy(activeZone.gameObject);
		}
	}

	void OnTriggerEnter(Collider other) {


		// Set the thing he collided with as the last known collision (for creating the warning zone)

		if (other.tag == "SplitTrack") {
			lastKnownCollider = other;

			inActiveZone = true;

			switchTrackButton.GetComponent<Image>().color = Color.green;
			CreateActiveZone();
		}
		else if (other.tag == "Locomotive") {
			lastKnownCollider = other;

			return;
		}
		/*else {
			// Disable this guy
			isActive = false;
			// Set this guy as the control return from meeting the master (Rick)
			warningChecker.GetComponent<WarningChecker>().SetReturnCharacter(this.gameObject.transform);
			// Switch cameras to go to Rick
			warningChecker.GetComponent<WarningChecker>().SwitchCameras();
		}*/



	}

	// Update is called once per frame
	void Update () {
		if (!isActive) {
			anim.SetFloat("Speed", 0);
			return;
		}
		if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle2Crouch_Neutral2Crouch2Idle")) {
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

	}
}
