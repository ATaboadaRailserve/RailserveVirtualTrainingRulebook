using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour {
	
	[System.Serializable]
	public class Movement {
		public string verticalAxis = "Vertical";
		public string horizontalAxis = "Horizontal";
		
		[Header("Forward")]
		public float accelerationF = 10;
		public float maxSpeedF = 5;
		
		[Header("Strafe")]
		public float accelerationS = 6;
		public float maxSpeedS = 3;
		
		[Header("Backward")]
		public float accelerationB = 4;
		public float maxSpeedB = 2;
	}
	
	[System.Serializable]
	public class Look {
		public string verticalAxis = "LookVertical";
		public string horizontalAxis = "LookHorizontal";
		
		public float maxDeltaLook = 5;
		
		public float sensitivityVertical = 2;
		public float sensitivityHorizontal = 2;
		
		public bool limitVertical = true;
		public Vector2 verticalLimits = new Vector2(1,179);
		public bool limitHorizontal;
		public Vector2 horizontalLimits;
	}
	
	public InteractionHandler interactionHandler;
	public Transform head;
	public Transform cameraTransform;
	public Movement movement;
	public Look look;
	
	private Vector3 accel;
	private Rigidbody rigidbody;
	private float directionAngle;
	private bool canMove;
	private bool canLook;
	
	private bool lookCoRunning;
	private Coroutine lookCo;
	private float lookCoTimer;
	private Vector3 initialLook;
	private Vector3 bodyAngles;
	private Vector3 headAngles;
	private bool followTarget;
	private Transform targetTransform;
	
	#region Rewired Stuff
	private Player player;
	void Awake() {
		player = ReInput.players.GetPlayer(0); // get the Rewired Player
	}
	#endregion
	
	void Start () {
		rigidbody = GetComponent<Rigidbody>();
		GetSensitivityOnStart();
	}
	
	void FixedUpdate () {
		if (followTarget) {
			LookAt(targetTransform);
		}
		
		if (canLook && !GameObject.FindWithTag("EscapeCursor").GetComponent<EscapeCursor>().IsCurrentlyEscaped) {
			if (canMove) {
				UpdateMovement();
			}
			/*
			if (Cursor.lockState != CursorLockMode.Locked)
				Cursor.lockState = CursorLockMode.Locked;
			*/
			UpdateLook();
		} else {
			/*
			if (Cursor.lockState != CursorLockMode.None)
				Cursor.lockState = CursorLockMode.None;
			*/
		}
	}
	
	void UpdateMovement() {
		accel.z = player.GetAxis(movement.verticalAxis);
		if (Application.isEditor) {
			if (Input.GetKey(KeyCode.LeftShift))
				accel.z *= 10f;
		}
		if (accel.z > 0)
			accel.z *= movement.accelerationF;
		else if (accel.z < 0)
			accel.z *= movement.accelerationB;
		accel.x = player.GetAxis(movement.horizontalAxis)*movement.accelerationS;
		rigidbody.AddForce(transform.TransformDirection(accel));
		
		directionAngle = Vector3.Dot(rigidbody.velocity, transform.forward);
		if (directionAngle < 0) {
			directionAngle = Mathf.Lerp(movement.maxSpeedS, movement.maxSpeedB, -directionAngle);
		} else {
			directionAngle = Mathf.Lerp(movement.maxSpeedS, movement.maxSpeedF, directionAngle);
		}
		
		if (rigidbody.velocity.sqrMagnitude > directionAngle*directionAngle)
			rigidbody.velocity = rigidbody.velocity.normalized * directionAngle;
	}
	
	void UpdateLook () {
		
		// Look vertically with just the head
		headAngles = head.localEulerAngles;
		headAngles.z += Mathf.Clamp(player.GetAxis(look.verticalAxis) * look.sensitivityVertical, -look.maxDeltaLook, look.maxDeltaLook);
		if (look.limitVertical)
			headAngles.z = Mathf.Clamp(headAngles.z, look.verticalLimits.x, look.verticalLimits.y);
		head.localEulerAngles = headAngles;
		
		// Look horizontally with just the body
		bodyAngles = transform.localEulerAngles;
		bodyAngles.y += Mathf.Clamp(player.GetAxis(look.horizontalAxis) * look.sensitivityHorizontal, -look.maxDeltaLook, look.maxDeltaLook);
		if (look.limitHorizontal)
			bodyAngles.y = Mathf.Clamp(bodyAngles.y, look.horizontalLimits.x, look.horizontalLimits.y);
		transform.localEulerAngles = bodyAngles;
	}
	
	void LookAt (Vector3 target) {
		transform.LookAt(target);
		bodyAngles = transform.localEulerAngles;
		bodyAngles.x = 0;
		transform.localEulerAngles = bodyAngles;
		
		head.LookAt(target);
		headAngles = head.localEulerAngles;
		headAngles.z = 90 - headAngles.x;
		headAngles.y = -90;
		headAngles.x = 0;
		head.localEulerAngles = headAngles;
	}
	
	void LookAt (Transform target) {
		transform.LookAt(target);
		bodyAngles = transform.localEulerAngles;
		bodyAngles.x = 0;
		transform.localEulerAngles = bodyAngles;
		
		head.LookAt(target);
		headAngles = head.localEulerAngles;
		headAngles.z = 90 - headAngles.x;
		headAngles.y = -90;
		headAngles.x = 0;
		head.localEulerAngles = headAngles;
	}
	
	// Force Camera based on a static point
	public void ForceCamera (Vector3 target, float lookTime = 0, bool stopMovement = true, bool stayOnObject = false) {
		if (lookCoRunning)
			StopCoroutine(lookCo);
		lookCo = StartCoroutine(LookCoroutine(target, lookTime, new AnimationCurve(new Keyframe(0,0), new Keyframe(1,1)), stopMovement, stayOnObject));
	}
	
	public void ForceCamera (Vector3 target, AnimationCurve lookCurve, float lookTime = 0, bool stopMovement = true, bool stayOnObject = false) {
		if (lookCoRunning)
			StopCoroutine(lookCo);
		lookCo = StartCoroutine(LookCoroutine(target, lookTime, lookCurve, stopMovement, stayOnObject));
	}
	
	IEnumerator LookCoroutine (Vector3 target, float lookTime, AnimationCurve lookCurve, bool stopMovement, bool stayOnObject) {
		followTarget = false;
		lookCoTimer = 0;
		initialLook = cameraTransform.position + cameraTransform.forward*10f;
		interactionHandler.playerCanWalk = !stopMovement;
		do {
			transform.LookAt(Vector3.Lerp(initialLook, target, lookCurve.Evaluate(lookCoTimer/lookTime)));
			bodyAngles = transform.localEulerAngles;
			bodyAngles.x = 0;
			transform.localEulerAngles = bodyAngles;
			
			head.LookAt(Vector3.Lerp(initialLook, target, lookCurve.Evaluate(lookCoTimer/lookTime)));
			headAngles = head.localEulerAngles;
			headAngles.z = 90 - headAngles.x;
			headAngles.y = -90;
			headAngles.x = 0;
			head.localEulerAngles = headAngles;
			
			lookCoTimer += Time.deltaTime;
			
			yield return null;
		} while (lookCoTimer < lookTime);
		interactionHandler.playerCanWalk = !stayOnObject;
	}
	
	// Force Camera based on a transform
	public void ForceCamera (Transform target, float lookTime = 0, bool stopMovement = true, bool stayOnObject = false) {
		if (lookCoRunning)
			StopCoroutine(lookCo);
		lookCo = StartCoroutine(LookCoroutine(target, lookTime, new AnimationCurve(new Keyframe(0,0), new Keyframe(1,1)), stopMovement, stayOnObject));
	}
	
	public void ForceCamera (Transform target, AnimationCurve lookCurve, float lookTime = 0, bool stopMovement = true, bool stayOnObject = false) {
		if (lookCoRunning)
			StopCoroutine(lookCo);
		lookCo = StartCoroutine(LookCoroutine(target, lookTime, lookCurve, stopMovement, stayOnObject));
	}
	
	IEnumerator LookCoroutine (Transform target, float lookTime, AnimationCurve lookCurve, bool stopMovement, bool stayOnObject) {
		followTarget = false;
		lookCoTimer = 0;
		initialLook = cameraTransform.position + cameraTransform.forward*10f;
		interactionHandler.playerCanWalk = !stopMovement;
		while (lookCoTimer < lookTime) {
			transform.LookAt(Vector3.Lerp(initialLook, target.position, lookCurve.Evaluate(lookCoTimer/lookTime)));
			bodyAngles = transform.localEulerAngles;
			bodyAngles.x = 0;
			transform.localEulerAngles = bodyAngles;
			
			head.LookAt(Vector3.Lerp(initialLook, target.position, lookCurve.Evaluate(lookCoTimer/lookTime)));
			headAngles = head.localEulerAngles;
			headAngles.z = 90 - headAngles.x;
			headAngles.y = -90;
			headAngles.x = 0;
			head.localEulerAngles = headAngles;
			
			lookCoTimer += Time.deltaTime;
			
			yield return null;
		}
		
		transform.LookAt(target.position);
		bodyAngles = transform.localEulerAngles;
		bodyAngles.x = 0;
		transform.localEulerAngles = bodyAngles;
		
		head.LookAt(target.position);
		headAngles = head.localEulerAngles;
		headAngles.z = 90 - headAngles.x;
		headAngles.y = -90;
		headAngles.x = 0;
		head.localEulerAngles = headAngles;
		
		interactionHandler.playerCanWalk = !stayOnObject;
		followTarget = stayOnObject;
		targetTransform = target;
	}
	
	public void StopFollowing () {
		followTarget = false;
	}
	
	public bool CanMove {
		get { return canMove; }
		set { canMove = value; }
	}

    public void ChangeVerticalLimits()
    {
        look.verticalLimits = new Vector2(70, 110);
    }

    public void ChangeHorizontalLimits()
    {
        look.limitHorizontal = true;
        look.horizontalLimits = new Vector2(30, 150);
    }

    public void ForceLookAtMenu()
    {
        head.transform.rotation = new Quaternion(0, -90, 90, 0);
        cameraTransform.transform.rotation = new Quaternion(90, 0, -90, 0);
    }

    public bool CanLook {
		get { return canLook; }
		set { canLook = value; }
	}
	
	public Rigidbody GetRigidbody () {
		return rigidbody;
	}
	
	public void SetSensitivity (float sensitivity) {
		PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
		
		look.sensitivityHorizontal = sensitivity;
		look.sensitivityVertical = sensitivity;
	}
	
	void GetSensitivityOnStart()
	{
		look.sensitivityHorizontal = PlayerPrefs.GetFloat("MouseSensitivity",30);
		look.sensitivityVertical = PlayerPrefs.GetFloat("MouseSensitivity",30);
	}
	
}