using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GrabandRotate : MonoBehaviour
{
	//------------------------------Public-----------------------------
	[Range(0,360)]
	public int switchScoreNumber;
	public float maxRotation; //maximum rotation of switch
	public bool start_Left_Side;
	public enum SwitchType{BackSaver, Tutorial, Other};
	public SwitchType type;
	public List<StuckObject> objectlist = new List<StuckObject> ();
	public bool RightSwitch;
	public bool canBeLocked;//Can this be locked
	public float angleOffset;
	public float maxRailAngle;
	public Text warningText;
	public int random_factor;
	public bool switchScoring;
	public GameObject CarPrefab;
	public BezierSetup currentBezierHandler;
	public SplineInterpolator currentSpline;
	public AudioClip derailSound;

	//------------------------------Public Hidden----------------------
	[HideInInspector]
	public bool inSwitchRange = false;
	[HideInInspector]
	public bool locked;
	[HideInInspector]
	public bool lined;
	[HideInInspector]
	public float minRotation = 0f;
	[HideInInspector]
	public int playerLocation; //0=nottouching 1=left 2=right
	[HideInInspector]
	public int nearState=-1, farState=-1;
	[HideInInspector]
	public bool onFarSide;
	[HideInInspector]
	public int switchNumber;

	//------------------------------Private----------------------------
	private bool switchIsGrabed = false; //Is Switch Grabed?
	private bool currentlyWarning = false;
	private bool currentlyWarningFar = false;
	private bool interactedwith = false;
	private bool scored;
	private bool ingame;
	private float midRotation; //midpoint of rotation
	private float acceleration = 0;
	private Transform switchGameObject;
	private Transform switchFlag;
	private Transform switchTrack;
	private GameObject farItem, nearItem;
	private Camera cam;
	private WarningSystem warningSystem;
	private Vector3 lastRotation;
	//static int NOTTOUCHING = 0;
	private static int LEFT = 1;
	private static int RIGHT = 2;
	private static int ONTRACK = 3;
	private int delay;
	private SwitchTutorial tutorialscript;
	private GameObject checkTrack;
	private RockScript removeRock;
	private Switch_GAME_CONTROL gameControl;
	private Wheels wheelF;
	private Wheels wheelB;
	private GameObject localcar;
	private Camera topDownCamera;
	private ScreenFader screenFader;
	private AudioSource locomotiveIdle;
	private AudioSource ring;
	private bool tutorialderail = false;
	private DatabaseMessageHolder messageHolder;
	private int tutorialScore = 100;
	private GameObject restartTutorial;
	private UnityStandardAssets.Characters.FirstPerson.FirstPersonController fpsController;


	void Start ()
	{
		cam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>();
		switchGameObject = transform.parent.transform;
		switchFlag = switchGameObject.parent.FindChild ("Flag_1").transform;
		switchTrack = switchGameObject.parent.FindChild ("Rails").transform;
		farItem = switchGameObject.parent.FindChild ("ITEM_F").gameObject;
		nearItem = switchGameObject.parent.FindChild ("ITEM_N").gameObject;
		removeRock = nearItem.GetComponent<RockScript> ();
		screenFader = GameObject.FindGameObjectWithTag ("ScreenFader").GetComponent<ScreenFader> ();
		messageHolder = GameObject.FindWithTag ("MessageHolder").GetComponent<DatabaseMessageHolder> ();
		fpsController = cam.transform.parent.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ();

		if (GameObject.FindGameObjectWithTag ("TopDownCamera") != null) {
			topDownCamera = GameObject.FindGameObjectWithTag ("TopDownCamera").GetComponent<Camera>();
			topDownCamera.gameObject.SetActive (false);
		}
		if (GameObject.FindGameObjectWithTag ("SwitchGameControl") != null) {
			gameControl = GameObject.FindGameObjectWithTag ("SwitchGameControl").GetComponent<Switch_GAME_CONTROL> ();
			ingame = true;
		}

		locked = canBeLocked;
		midRotation = maxRotation / 2f;
		SetRandomItem (true);
		SetRandomItem (false);
		if (start_Left_Side) 
		{
			switchGameObject.transform.localEulerAngles = new Vector3 (0, 0, maxRotation); //sets angle of switch
			switchFlag.transform.localEulerAngles = new Vector3 (0, maxRotation * (90f / maxRotation), 0);//sets angle of flag
			if (!RightSwitch)
			{
				switchTrack.transform.localEulerAngles = new Vector3 (0, -maxRotation * (maxRailAngle / maxRotation), 0); //sets angle of track
			} 
		} 
		else 
		{
			if (RightSwitch) 
			{
				switchTrack.transform.localEulerAngles = new Vector3 (0, -maxRotation * (maxRailAngle / maxRotation), 0); //sets angle of track
			}
		}
		if(nearState>-1 && start_Left_Side)
		{
			//switchTrack.transform.localEulerAngles = new Vector3(0,-(maxRotation - 35f)*(.7f/maxRotation),0); //sets angle of track
			if (!RightSwitch) {
				switchTrack.transform.localEulerAngles = new Vector3 (0, -(minRotation + objectlist[nearState].size) * (maxRailAngle / maxRotation), 0); //sets angle of track
			} else {
				switchTrack.transform.localEulerAngles = new Vector3 (0, -(maxRotation - objectlist[nearState].size) * (maxRailAngle / maxRotation), 0); //sets angle of track
			}
		}
		if(farState>-1 && start_Left_Side)
		{
			//switchTrack.transform.localEulerAngles = new Vector3(0,-(maxRotation - 35f)*(.7f/maxRotation),0); //sets angle of track
			if (!RightSwitch) {
				switchTrack.transform.localEulerAngles = new Vector3 (0, -(minRotation + objectlist[farState].size) * (maxRailAngle / maxRotation), 0); //sets angle of track
			} else {
				switchTrack.transform.localEulerAngles = new Vector3 (0, -(maxRotation - objectlist[farState].size) * (maxRailAngle / maxRotation), 0); //sets angle of track
			}
		}

		if (type == SwitchType.Tutorial) {
			tutorialscript = GameObject.FindGameObjectWithTag ("TutorialControl").GetComponent<SwitchTutorial> ();
			checkTrack = switchGameObject.parent.FindChild ("CheckTrack").gameObject;
			restartTutorial = GameObject.FindGameObjectWithTag ("RestartTutorialUI").gameObject;
			restartTutorial.SetActive(false);
			removeRock.hideArrow = false;
		}
		if (switchScoring) {
			warningSystem = GameObject.FindWithTag ("WarningSystem").GetComponent<WarningSystem> ();
		}
		//warningText = GameObject.FindWithTag ("WarningSystem").GetComponent<Text> ();
		//StartCoroutine(Warn ("Starting..."));
		//print(transform.parent.parent.name +" N:" + nearState + "  F:" + farState);
	}

	void CheckLined()
	{
		//----------------Sets if track is lined(ie. if the train will derail or not)----------------------
		if (nearState < 0 && farState < 0) {
			lined = true;
		}
		if (onFarSide) {
			if (nearState == -1) {
				lined = true;
			} else {
				lined = false;
			}
		}
		else if (!onFarSide) {
			if (farState == -1) {
				lined = true;
			} else {
				lined = false;
			}
		}
	}
	void SetCarUp()
	{
		localcar = Instantiate (CarPrefab);
		wheelF = localcar.transform.FindChild ("FrontWheels").GetComponent<Wheels> ();
		wheelB = localcar.transform.FindChild ("RearWheels").GetComponent<Wheels> ();

		wheelF.currentBezierHandler = currentBezierHandler;
		wheelB.currentBezierHandler = currentBezierHandler;
		wheelF.currentSpline = currentSpline;
		wheelB.currentSpline = currentSpline;

		locomotiveIdle = localcar.transform.FindChild ("Leaf").GetComponent<AudioSource> ();
		locomotiveIdle.Stop ();
		ring = wheelF.gameObject.GetComponent<AudioSource> ();
		ring.Stop ();
	}
	void SendCar()
	{
		ring.volume = .5f;
		locomotiveIdle.volume = .5f;
		ring.Play ();
		locomotiveIdle.Play ();
		wheelF.accelerating = true;
		wheelB.accelerating = true;
	}

	void Update ()
	{
		if (!farItem.activeSelf) {farState = -1;}
		if (!nearItem.activeSelf) {nearState = -1;}

		CheckLined ();
		if (!scored && !inSwitchRange && interactedwith && locked && lined && switchScoring && !ingame) 
		{
			warningSystem.Pass (2);
			scored = true;
		}

		if (type == SwitchType.Tutorial && nearState < 0 ) {
			if (tutorialscript.current == 0) {
				tutorialscript.NextStep (0);
			}
		}

		if ((!locked && !inSwitchRange && !currentlyWarningFar&& switchScoring) || (!lined && interactedwith && !inSwitchRange&& !currentlyWarningFar&& switchScoring)) {
			warningSystem.Warn (1);//Left Switch In Unsafe Condition
			if(type == SwitchType.Tutorial)
			{
				restartTutorial.SetActive (true);
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				fpsController.enabled = false;
				tutorialScore = 0;
				messageHolder.WriteMessage("0",4);
				messageHolder.PushingMessages();
				print ("1");
			}
			//fpsController.enabled = false;
			//Cursor.visible = true;
			//messageHolder.WriteMessage("Left Switch In Unsafe Condition",1);
			if (ingame) {
				print ("fail");
				StartCoroutine (gameControl.WaitForFail (gameControl.waitTime));
			}
			currentlyWarningFar = true;
		}
		if (inSwitchRange) {
			currentlyWarningFar = false;
		}

		switchGameObject.localEulerAngles = new Vector3 (0, 0, Mathf.Clamp (switchGameObject.localEulerAngles.z, minRotation, maxRotation));
		if(type == SwitchType.Tutorial)
		{
		RaycastHit hit;
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast (ray, out hit, 2f);
		if (hit.collider == checkTrack.GetComponent<Collider> ()) {
			if (tutorialscript.current == 8) {
					SetCarUp ();
					tutorialscript.NextStep (8);
					cam.transform.parent.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().enabled = false;
					cam.gameObject.SetActive (false);
					topDownCamera.gameObject.SetActive (true);
					locked = false;
					switchGameObject.transform.localEulerAngles = new Vector3 (0, 0, maxRotation-15); //sets angle of switch
	
					if(tutorialScore == 100)
					{
						messageHolder.moduleFinished = true;
					}
					messageHolder.WriteMessage(tutorialScore.ToString(),4);
					messageHolder.PushingMessages();
					StartCoroutine (WaitToSendCar (7));
			}
		}
			if (tutorialscript.current == 9 && wheelF.t == 1 && tutorialderail == false) {
				tutorialderail = true;
				ring.Stop ();
				ring.loop = false;
				ring.clip = derailSound;
				ring.Play ();
				StartCoroutine( WaitToEndScene (2));
			}
		}
		if (switchIsGrabed) 
		{
			acceleration = 0;
			RaycastHit hit;
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			Physics.Raycast (ray, out hit, 1.5f);
			if (hit.collider == gameObject.GetComponent<Collider> ()) {//if raycast on mouse hits hitbox

				if (type == SwitchType.Tutorial) {
					if (tutorialscript.current == 2 && nearState < 0) {
						tutorialscript.NextStep (2);
					}
				}
				if (type == SwitchType.Tutorial) {
					if (tutorialscript.current == 5) {
						tutorialscript.NextStep (5);
					}
				}

				interactedwith = true;

				Vector3 diff = hit.point - transform.position; //gets difference between raycast to hitbox and switch position
				//diff = Quaternion.AngleAxis (-transform.parent.parent.localEulerAngles.y, Vector3.up) * diff; 
				diff = Quaternion.AngleAxis (-transform.parent.parent.eulerAngles.y, Vector3.up) * diff; 
				float tempPI = (Mathf.Atan2 (diff.y, diff.x));
				if (tempPI < -Mathf.PI / 2) {
					tempPI += 2 * Mathf.PI;
				}
				float angle = ((tempPI * 180.0f / Mathf.PI) + angleOffset); //gets angle from difference
				angle = Mathf.Clamp (angle, minRotation, maxRotation); //clamps the switch to minimum rotation or maximum rotation
				if (switchGameObject.transform.localEulerAngles.z <= minRotation + 3f && lastRotation.z > switchGameObject.localEulerAngles.z && canBeLocked) { //If you move the switch into the latch on rightside
					locked = true; //Lock Latch
					onFarSide = false; //Set side of lock
				} else if (switchGameObject.transform.localEulerAngles.z >= maxRotation - 3f && lastRotation.z < switchGameObject.localEulerAngles.z && canBeLocked) { //If you move the switch into the latch on leftside
					locked = true; //Lock Latch
					onFarSide = true; //Set side of Lock
				}
				if (!locked || !canBeLocked) {
					if (nearState > -1 && objectlist [nearState].size > 50) {
						angle = Mathf.Clamp (angle, minRotation, maxRotation - 20);
					} else if (farState > -1 && objectlist [farState].size > 50) {
						angle = Mathf.Clamp (angle, minRotation + 20, maxRotation);
					}

					switchGameObject.transform.localEulerAngles = new Vector3 (0, 0, angle); //sets angle of switch
					switchFlag.transform.localEulerAngles = new Vector3 (0, angle * (90f / maxRotation), 0);//sets angle of flag

					if (nearState > -1) {
						angle = Mathf.Clamp (angle, minRotation, maxRotation - objectlist [nearState].size);
						lined = false;
					}
					if (farState > -1) {
						angle = Mathf.Clamp (angle, minRotation + objectlist [farState].size, maxRotation);
						lined = false;
					}
					if (RightSwitch) {
						angle = maxRotation - angle;
					}
					switchTrack.transform.localEulerAngles = new Vector3 (0, -angle * (maxRailAngle / maxRotation), 0); //sets angle of track
				} else if (locked && !onFarSide && canBeLocked) {
					angle = Mathf.Clamp (angle, minRotation, minRotation + 2.9f);
					if (farState > -1 && objectlist [farState].size <= 50) {
						angle = Mathf.Clamp (angle, minRotation + 2, maxRotation);
					}
					switchGameObject.transform.localEulerAngles = new Vector3 (0, 0, angle); //sets angle of switch
					switchFlag.transform.localEulerAngles = new Vector3 (0, angle * (90f / maxRotation), 0); //sets angle of flag
					if (farState > -1 && objectlist [farState].size <= 50) {
						angle = Mathf.Clamp (angle, minRotation + 35, maxRotation);
					}
					if (RightSwitch) {
						angle = maxRotation - angle;
					}
					switchTrack.transform.localEulerAngles = new Vector3 (0, -angle * (maxRailAngle / maxRotation), 0); //sets angle of track
				} else if (locked && onFarSide && canBeLocked) {
					angle = Mathf.Clamp (angle, maxRotation - 2.9f, maxRotation);
					if (type == SwitchType.Tutorial) {
						if (tutorialscript.current == 7 && angle < maxRotation - 2.7f) {
							tutorialscript.NextStep (7);
						}
					}
					if (type == SwitchType.Tutorial) {
						if (tutorialscript.current == 6 && angle > maxRotation - 1f) {
							tutorialscript.NextStep (6);
						}
					}
					if (nearState > -1 && objectlist [nearState].size <= 50) {
						angle = Mathf.Clamp (angle, minRotation, maxRotation - 2);
					}
					switchGameObject.transform.localEulerAngles = new Vector3 (0, 0, angle); //sets angle of switch
					switchFlag.transform.localEulerAngles = new Vector3 (0, angle * (90f / maxRotation), 0); //sets angle of flag
					if (nearState > -1 && objectlist [nearState].size <= 50) {
						angle = Mathf.Clamp (angle, minRotation, maxRotation - 35);
					}
					if (RightSwitch) {
						angle = maxRotation - angle;
					}
					switchTrack.transform.localEulerAngles = new Vector3 (0, -angle * (maxRailAngle / maxRotation), 0); //sets angle of track
				}
				if (playerLocation == ONTRACK && !currentlyWarning && switchScoring) {
					print ("Failure: Do Not Operate Switch While On Track");
					warningSystem.Warn (0);
					if(type == SwitchType.Tutorial)
					{
						restartTutorial.SetActive (true);
						Cursor.lockState = CursorLockMode.None;
						Cursor.visible = true;
						fpsController.enabled = false;
						tutorialScore = 0;
						messageHolder.WriteMessage("0",4);
						messageHolder.PushingMessages();
						print ("3");
					}
					//fpsController.enabled = false;
					//Cursor.visible = true;
					//messageHolder.WriteMessage("Incorrect Switch Operational Position",1);
					currentlyWarning = true;
					if (ingame)
						StartCoroutine (gameControl.WaitForFail (gameControl.waitTime));
					//StartCoroutine(Warn ("Failure: Do Not Operate Switch While On Track"));
				} else if (switchGameObject.transform.localEulerAngles.z > midRotation + 10 && playerLocation != LEFT && !currentlyWarning&& switchScoring) { //if player is on right side and the switch is rotated to left
					print ("Failure: Move To Left Side!");
					warningSystem.Warn (0);
					if(type == SwitchType.Tutorial)
					{
						restartTutorial.SetActive (true);
						Cursor.lockState = CursorLockMode.None;
						Cursor.visible = true;
						fpsController.enabled = false;
						tutorialScore = 0;
						messageHolder.WriteMessage("0",4);
						messageHolder.PushingMessages();
						print ("4");
					}
					//fpsController.enabled = false;
					//Cursor.visible = true;
					//messageHolder.WriteMessage("Incorrect Switch Operational Position",1);
					currentlyWarning = true;
					if (ingame)
						StartCoroutine (gameControl.WaitForFail (gameControl.waitTime));
					//StartCoroutine(Warn ("Failure: Move To Left Side!"));
				} else if (switchGameObject.transform.localEulerAngles.z < midRotation - 10 && playerLocation != RIGHT && !currentlyWarning&& switchScoring) { //if player is on left side and the switch is rotated to right
					print ("Failure: Move To Right Side!");
					warningSystem.Warn (0);
					if(type == SwitchType.Tutorial)
					{
						restartTutorial.SetActive (true);
						fpsController.enabled = false;
						Cursor.lockState = CursorLockMode.None;
						Cursor.visible = true;
						tutorialScore = 0;
						messageHolder.WriteMessage("0",4);
						messageHolder.PushingMessages();
						print ("5");
					}
					//fpsController.enabled = false;
					//Cursor.visible = true;
					//messageHolder.WriteMessage("Incorrect Switch Operational Position",1);
					currentlyWarning = true;
					if (ingame)
						StartCoroutine (gameControl.WaitForFail (gameControl.waitTime));
					//StartCoroutine(Warn ("Failure: Move To Right Side!"));
				}
			} 
		}
		else 
		{
			currentlyWarning = false;

			if (type == SwitchType.Tutorial) {
				if (tutorialscript.current == 3 && switchGameObject.transform.localEulerAngles.z > midRotation - 10 && switchGameObject.transform.localEulerAngles.z < midRotation + 10) {
					tutorialscript.NextStep (3);
				}
			}
			if (type == SwitchType.Tutorial) {
				if (tutorialscript.current == 4 && playerLocation == LEFT) {
					tutorialscript.NextStep (4);
				}
			}
		}

	/*	if (type == SwitchType.CannonBall && switchGameObject.transform.localEulerAngles.z > minRotation && switchGameObject.transform.localEulerAngles.z < maxRotation) //If it is a cannonball switch and its not at min or max rotation
		{
			RaycastHit hit;
			Ray ray = cam.ScreenPointToRay (Input.mousePosition);
			Physics.Raycast (ray, out hit, 1.5f);
			if ((hit.collider != gameObject.GetComponent<Collider> () && switchIsGrabed) || !switchIsGrabed) //if the mouse is not down or it is down but not on the switch
			{
				Vector3 tempVec;
				if (switchGameObject.transform.localEulerAngles.z > midRotation + 15) //if switch is on left side
				{
					tempVec = new Vector3 (0, 0, Mathf.Clamp((switchGameObject.transform.localEulerAngles.z + acceleration),minRotation,maxRotation)); //rotate
					float angle = tempVec.z;
					if(RrockState.state == Rock.BIG) 
					{
						tempVec.z = Mathf.Clamp (tempVec.z, minRotation, maxRotation - 20);
					}
					else if(RrockState.state == Rock.SMALL)
					{
						tempVec.z = Mathf.Clamp (tempVec.z, minRotation, maxRotation - 2);
					}
					switchGameObject.transform.localEulerAngles = tempVec;
					switchFlag.transform.localEulerAngles = new Vector3(0,switchGameObject.transform.localEulerAngles.z*(90f/maxRotation),0);//sets angle of flag
					if(RrockState.state == Rock.BIG) 
					{
						angle = Mathf.Clamp (angle, minRotation, maxRotation - 80);
					}
					if(RrockState.state == Rock.SMALL) 
					{
						angle = Mathf.Clamp (angle, minRotation, maxRotation - 50);
					}
					//if(RightSwitch){angle = maxRotation - angle;}
					switchTrack.transform.localEulerAngles = new Vector3(0,-angle*(maxRailAngle/maxRotation),0); //sets angle of track
					//switchTrack.transform.localEulerAngles = new Vector3(0,-switchGameObject.transform.localEulerAngles.z*(.6f/maxRotation),0); //sets angle of track
				} 
				else if (switchGameObject.transform.localEulerAngles.z < midRotation -15) //if switch is on right side
				{
					tempVec = new Vector3 (0, 0, Mathf.Clamp((switchGameObject.transform.localEulerAngles.z - acceleration),minRotation,maxRotation)); //rotate
					float angle = tempVec.z;
					if (LrockState.state == Rock.BIG) 
					{
						tempVec.z = Mathf.Clamp (tempVec.z, minRotation + 20, maxRotation);
					}
					else if(LrockState.state == Rock.SMALL)
					{
						tempVec.z = Mathf.Clamp (tempVec.z, minRotation + 2, maxRotation);
					}	
					switchGameObject.transform.localEulerAngles = tempVec;
					switchFlag.transform.localEulerAngles = new Vector3(0,switchGameObject.transform.localEulerAngles.z*(90f/maxRotation),0);//sets angle of flag
					if (LrockState.state == Rock.BIG) 
					{
						angle = Mathf.Clamp (angle, minRotation + 80, maxRotation);
					}
					if(LrockState.state == Rock.SMALL)
					{
						angle = Mathf.Clamp (angle, minRotation + 50, maxRotation);
					}
					//if(RightSwitch){angle = maxRotation - angle;}
					switchTrack.transform.localEulerAngles = new Vector3(0,-angle*(maxRailAngle/maxRotation),0); //sets angle of track
					//switchTrack.transform.localEulerAngles = new Vector3(0,-switchGameObject.transform.localEulerAngles.z*(.6f/maxRotation),0); //sets angle of track
				}
				if(switchGameObject.transform.localEulerAngles.z <= minRotation+3f && lastRotation.z > switchGameObject.localEulerAngles.z && canBeLocked) //If you move the switch into the latch on rightside
				{
					locked = true; //Lock Latch
					leftSide = false; //Set side of lock
				}
				else if(switchGameObject.transform.localEulerAngles.z >= maxRotation-3f && lastRotation.z < switchGameObject.localEulerAngles.z && canBeLocked) //If you move the switch into the latch on leftside
				{
					locked = true; //Lock Latch
					leftSide = true; //Set side of Lock
				}
				acceleration += .1f;
			}
		} 
		else 
		{
			acceleration = 0;
		}*/

		if (delay == 3) 
		{
			lastRotation = switchGameObject.localEulerAngles; //Sets last rotation
			delay = 0;
		}
		delay++;
	}
		
	public void OnMouseDown ()
	{
		switchIsGrabed = true; //Switch is Grabed
	}
	public void OnMouseUp()
	{
		switchIsGrabed = false; //Switch is not Grabed
	}

	/*void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Transform tempLeft = transform.parent.parent.FindChild ("LeftHit").transform; //Get Left Hitbox
		Transform tempRight = transform.parent.parent.FindChild ("RightHit").transform; //Get Right Hitbox
		Transform tempTrack = transform.parent.parent.FindChild ("TrackHit").transform; //Get Right Hitbox
		Gizmos.DrawWireCube (tempLeft.GetComponent<Collider> ().bounds.center, tempLeft.GetComponent<Collider> ().bounds.extents*2); 
		Gizmos.DrawWireCube (tempRight.GetComponent<Collider> ().bounds.center, tempRight.GetComponent<Collider> ().bounds.extents*2);
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube (tempTrack.GetComponent<Collider> ().bounds.center, tempTrack.GetComponent<Collider> ().bounds.extents*2);
	}*/

	// Kyle wuz hear
	/*IEnumerator Warn (string warning) {
		warningText.transform.parent.gameObject.SetActive (true);
		warningText.text = warning;
		yield return new WaitForSeconds (2f);
		warningText.transform.parent.gameObject.SetActive (false);

	}*/
	public IEnumerator WaitToSendCar(float i)
	{
		yield return new WaitForSeconds (i);
		SendCar ();
	}
	public IEnumerator WaitToEndScene(float i)
	{
		yield return new WaitForSeconds (i);
		screenFader.EndScene ("Switch(Game)");
		messageHolder.WriteMessage ("Completed Tutorial", 3);
		print ("5");
	}
	void SetRandomItem(bool near)
	{
		//-------------This sets if there is an object obstructing the track-----------------
		//------------- (-1 = nothing, any number above -1 is an object) --------------------
		if (random_factor > 0) {
			int random_number = Random.Range (0, random_factor);
			//random_number = 1;
			if (type == SwitchType.Tutorial) {
				if (near == true) {
					random_number = 0;
				} else {
					random_number = objectlist.Count + 2;
				}
			}
			if (near && random_number < objectlist.Count && !start_Left_Side) 
			{
				if (type == SwitchType.Tutorial) 
				{
					removeRock.hideArrow = true;
				}
				nearItem.SetActive (true);
				lined = false;
				Transform s = Instantiate (objectlist [random_number].item.transform);
				Transform t = nearItem.transform.FindChild ("Empty").transform;
				s.SetParent (nearItem.transform);
				s.localPosition = new Vector3 (0, 0, 0);
				s.localEulerAngles = new Vector3 (0, 90, 0);
				nearState = random_number;
			} 
			else if (!near && random_number < objectlist.Count && start_Left_Side) 
			{
				farItem.SetActive (true);
				lined = false;
				Transform s = Instantiate (objectlist [random_number].item.transform);
				Transform t = farItem.transform.FindChild ("Empty").transform;
				s.SetParent (farItem.transform);
				s.localPosition = new Vector3 (0, 0, 0);
				s.localEulerAngles = new Vector3 (0, 90, 0);
				farState = random_number;
			}
		}
	}
}


[System.Serializable]
public class StuckObject
{
	public int size;
	public GameObject item;
	public StuckObject(int state_i, GameObject item_g)
	{
	}
}
