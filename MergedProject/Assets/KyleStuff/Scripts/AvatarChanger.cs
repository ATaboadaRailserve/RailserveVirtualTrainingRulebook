using UnityEngine;
using System.Collections;
using Rewired;

public class AvatarChanger : MonoBehaviour {

	public Transform coupler;
	public Transform switcher;
	public Transform locomotive;
	public float transitionSpeed = 50.0f;
	public float lerpSpeed = 3.0f;
	public float heightOffset = 0.5f;
	public Light light;
	public ParticleSystem system;
	public Canvas couplerCanvas;
	public Canvas switcherCanvas;
	public Canvas leafCanvas;
	public OrthoFollowScript ortho;
	public Vector3  SwitcherViewOffset, CouplerViewOffset;
	public CameraWaypoints waypoints;
	public Transform[] icons;
	public float iconOffset = -50f;
	
	private Vector3[] iconsStart;

	//private Vector3 heightVector;
	private bool timeToLerp;
	//private float startTime;
	//private float lerpDistance;
	//private Vector3 lerpStart;
	private Vector3 lerpEnd;
	//float tempTime;
	
	private bool controllable = true;
	
	private int playerId = 0;
	private Player _player; // the Rewired player
	
	private Player player {
		get {
			// Get the Rewired Player object for this player. Refresh it as needed so it will get the new reference after a script recompile in the editor.
			if(_player == null) _player = ReInput.players.GetPlayer(playerId);
			return _player;
		}
	}
	
	// Use this for initialization
	void Start () {
		Screen.lockCursor = false;
		Cursor.visible = true;
		iconsStart = new Vector3[icons.Length];
		for (int i = 0; i < icons.Length; i++){
			iconsStart[i] = icons[i].position;
			if (i != 1)
				icons[i].position = iconsStart[i] + new Vector3(0, iconOffset, 0);
			else 
				icons[i].position = iconsStart[i];
		}
		timeToLerp = false;
		coupler.GetComponent<Coupler>().isActive = false;
		couplerCanvas.enabled = false;
		switcher.GetComponent<Switcher>().isActive = true;
		switcherCanvas.enabled = true;
		leafCanvas.enabled = false;
		this.transform.position = switcher.transform.position;
		//tempTime = 0.0f;
		//heightVector = new Vector3(0, heightOffset, 0);
		system.Stop();
		light.enabled = false;
		ortho.SetOffset(20, 15, 0);

	}

	void setUpLerp() {
		//startTime = Time.time;
		//lerpDistance = Vector3.Distance(coupler.position, switcher.position);
		if (coupler.GetComponent<Coupler>().isActive) {
			lerpEnd = coupler.transform.position;
		}
		else if (switcher.GetComponent<Switcher>().isActive) {
			lerpEnd = switcher.transform.position;
		}
		else if (locomotive.GetComponent<LocoScript>().isActive) {
			lerpEnd = locomotive.transform.position;
		}
		//tempTime = 0.0f;
	}

	public void UpdateLocoView() {
		int carCount = locomotive.GetComponent<LocoScript>().GetCarCount();
		float distance = carCount*0.5f + 30;
		ortho.SetOffset(distance/2, 15, carCount);
		ortho.SetLookAt(locomotive);
	}

	// Update is called once per frame
	void Update () {
		if (controllable) {
			if (player.GetButtonDown("Job1")) {			// Change control to Switcher
				icons[0].position = iconsStart[0] + new Vector3(0, iconOffset, 0);
				icons[1].position = iconsStart[1];
				icons[2].position = iconsStart[2] + new Vector3(0, iconOffset, 0);
				
				setUpLerp();
				if(waypoints.isActive)
					waypoints.Toggle();
				// Stop the coupler from running
				coupler.GetComponent<Coupler>().isActive = false;
				coupler.GetComponent<Animator>().SetFloat("Speed", 0.0f);

				// Switch to the switcher
				switcher.GetComponent<Switcher>().isActive = true;

				// Stop the loco
				locomotive.GetComponent<LocoScript>().isActive = false;
				locomotive.GetComponent<LocoScript>().velocity = 0.0f;
				timeToLerp = true;
				lerpEnd = switcher.transform.position;
				system.Play();
				light.enabled = true;
				couplerCanvas.enabled = false;
				switcherCanvas.enabled = true;
				leafCanvas.enabled = false;
				ortho.SetOffset(SwitcherViewOffset);
				ortho.SetLookAt(switcher);
				ortho.carNumCamera.depth = -1;
			}
			else if (player.GetButtonDown("Job2")) {	// Change control to Coupler
				icons[0].position = iconsStart[0];
				icons[1].position = iconsStart[1] + new Vector3(0, iconOffset, 0);
				icons[2].position = iconsStart[2] + new Vector3(0, iconOffset, 0);
				
				setUpLerp();
				if(waypoints.isActive)
					waypoints.Toggle();
				// Start the coupler
				coupler.GetComponent<Coupler>().isActive = true;

				// Stop the switcher from running
				switcher.GetComponent<Animator>().SetFloat("Speed", 0.0f);
				switcher.GetComponent<Switcher>().isActive = false;

				// Stop the loco
				locomotive.GetComponent<LocoScript>().isActive = false;
				locomotive.GetComponent<LocoScript>().velocity = 0.0f;

				timeToLerp = true;
				lerpEnd = coupler.transform.position;
				system.Play();
				light.enabled = true;
				couplerCanvas.enabled = true;
				switcherCanvas.enabled = false;
				leafCanvas.enabled = false;
				ortho.SetOffset(CouplerViewOffset);
				ortho.SetLookAt(coupler);
				ortho.carNumCamera.depth = 1;
			}
			else if (player.GetButtonDown("Job3")) {	// Change control to Locomotive
				icons[0].position = iconsStart[0] + new Vector3(0, iconOffset, 0);
				icons[1].position = iconsStart[1] + new Vector3(0, iconOffset, 0);
				icons[2].position = iconsStart[2];
				if(!waypoints.isActive)
					waypoints.Toggle();
				setUpLerp();
							// Start loco control
				locomotive.GetComponent<LocoScript>().isActive = true;

				coupler.GetComponent<Coupler>().isActive = false;
				coupler.GetComponent<Animator>().SetFloat("Speed", 0.0f);

				switcher.GetComponent<Switcher>().isActive = false;
				switcher.GetComponent<Animator>().SetFloat("Speed", 0.0f);
				timeToLerp = true;
				lerpEnd = locomotive.transform.position;
				system.Play();
				light.enabled = true;
				couplerCanvas.enabled = false;
				switcherCanvas.enabled = false;
				leafCanvas.enabled = true;
				ortho.carNumCamera.depth = -1;
				UpdateLocoView();
			}

			// ======  EMPTY AND  MOVEMENT ==============
			if (timeToLerp) {
				setUpLerp();
				// Lerp the empty and the  with it
				transform.position = Vector3.Lerp(transform.position, lerpEnd, Time.deltaTime*lerpSpeed);
				// If we've reached our target, stop lerping
				if (Vector3.Distance(this.transform.position, lerpEnd)< 1.0f) {
					timeToLerp = false;
					system.Stop();
					light.enabled = false;
				}
			}
			else {
				// Update the position of the empty
				if (coupler.GetComponent<Coupler>().isActive) {
					this.transform.position = coupler.transform.position;
				}
				else if (switcher.GetComponent<Switcher>().isActive) {
					this.transform.position = switcher.transform.position;
				}
				else if (locomotive.GetComponent<LocoScript>().isActive) {
					this.transform.position = locomotive.transform.position;
				}
			}
		}
	}
	
	public void Win () {
		controllable = false;
	}
}
