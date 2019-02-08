using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Rewired;

public class LocoScript : MonoBehaviour {

	public bool isActive;
	//public float currentSpeed;
	public float acceleration = 0.00000f;
	[HideInInspector]
	public float velocity = 0.0f;
	public float maxSpeed = 2.0f;
	private AudioSource audioSource;
	private SmartTankerScript smartTankerScript;
	public GameObject frontLight;
	public GameObject backLight;
	public bool airSystemComplete;
	public bool inRedZone;
	public WarningChecker wrongCar;
	
	// Kyle Addatives
	public int milesPerHour;
	public Text mphNumber;
	// Meters per second to miles per hour
	private float metersToMph = 2.23694f;
	private Vector3 lastPos;
	public Image speedWarning;
	public float friction = 0.0001f;
	[HideInInspector]
	public SoftWarning warner;

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
		isActive = false;
		audioSource = this.GetComponent<AudioSource>();
		smartTankerScript = this.GetComponent<SmartTankerScript>();
		airSystemComplete = true;
		inRedZone = false;
		lastPos = transform.position;
		frontLight.SetActive(false);
		backLight.SetActive(false);
		warner = GetComponent<SoftWarning>();
		speedWarning.enabled = false;
	}

	public void CompleteAirSystem() {
		airSystemComplete = true;
		if (smartTankerScript.childCarScript!=null) {
			smartTankerScript.childCarScript.FillAirBrakes();
		}
	}
	
	public void CheckAir () {
		if (smartTankerScript.childCarScript.CheckAir())
			airSystemComplete = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!isActive) {
			return;
		}

		if ((!airSystemComplete)||(inRedZone)) {
			if (Mathf.Abs(velocity) > acceleration) {
				velocity -= acceleration;
			}
			else {
				velocity = 0.0f;
			}
		}
		else {
			if (player.GetAxis("Horizontal") > 0 && velocity >= -maxSpeed/10.0f)
				velocity-=acceleration;
			else if (player.GetAxis("Horizontal") < 0 && velocity <= maxSpeed/10.0f)
					velocity+=acceleration;
			/*else if (velocity < -friction)
				velocity += friction;
			else if (velocity > friction)
				velocity -= friction;
			else
				velocity = 0;*/
		}
		
		if(velocity < 0) {
			frontLight.SetActive(true);
			backLight.SetActive(false);
		} else if (velocity > 0) {
			frontLight.SetActive(false);
			backLight.SetActive(true);
		}
		
		audioSource.pitch = 1+Mathf.Abs(velocity/50);
		smartTankerScript.Push(velocity);
		
		// Speed settings;
		Vector3 tempVec = lastPos - transform.position;
		milesPerHour = (int)Mathf.Round((tempVec.magnitude/Time.deltaTime)*metersToMph);
		if (milesPerHour > 8) {
			mphNumber.color = Color.red;
			speedWarning.enabled = true;
		} else {
			mphNumber.color = Color.white;
			speedWarning.enabled = false;
		}
		mphNumber.text = milesPerHour.ToString();
			
		lastPos = transform.position;
	}

	public int GetCarCount() {
		return smartTankerScript.GetCarCount();
	}
	
	public void CheckCars () {
		if (smartTankerScript.CheckPulls()) {
			smartTankerScript.AndAcrossTheLine();
			return;
		}
		wrongCar.SetReturnCharacter(transform);
		wrongCar.SwitchCameras();
	}
	
	public void Win () {
		acceleration = 0;
		velocity = 0;
		maxSpeed = 0;
	}
	
	public void CheckSpeed() {
		if (milesPerHour > 8) {
			warner.Warn(1);
		} else if (milesPerHour > 2) {
			warner.Warn(0);
		}
	}
}
