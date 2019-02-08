using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class Switch_GAME_CONTROL : MonoBehaviour {
	//------------------------------Public-----------------------------
	//public int timeDelay;
	//public Text timeText;
	public Text alignText;
	public Text completedText;
	public List<SwitchMasterControl> switchList = new List<SwitchMasterControl>();
	public List<Text> textList = new List<Text>();
	public GameObject CarPrefab;
	public BezierSetup currentBezierHandler;
	public SplineInterpolator currentSpline;
	public AudioClip derailSound;

	//------------------------------Public Hidden----------------------
	[HideInInspector]
	public int selectedTrackNum;
	[HideInInspector]
	public float waitTime;
	//------------------------------Private----------------------------
	private int currentTrackNum;
	//private int time;
	//private bool outOfTime = true;
	private WarningSystem warningSystem;
	private ScoreBoardTemp score;
	private bool fail = false;
	private bool derailf = false;
	private GameObject localcar;
	private Wheels wheelF;
	private Wheels wheelB;
	private int completedlevels = 0;
	private AudioSource ring;
	private bool gameGo = false;
	private Camera cam;
	private UnityStandardAssets.Characters.FirstPerson.FirstPersonController fpsController;
	private DatabaseMessageHolder messageHolder;
	private GameObject continueScreen;
	private ScreenFader screenFader;
	private bool waitForCar = false;
    

	// Use this for initialization
	void Start () {
		cam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>();
		fpsController = cam.transform.parent.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ();
		fpsController.enabled = false;
		warningSystem = GameObject.FindWithTag ("WarningSystem").GetComponent<WarningSystem> ();
		score = GameObject.FindWithTag ("ScoreBoard").GetComponent<ScoreBoardTemp> ();
		messageHolder = GameObject.FindWithTag ("MessageHolder").GetComponent<DatabaseMessageHolder> ();
		continueScreen = GameObject.FindGameObjectWithTag("ContinueMenu");
		screenFader = GameObject.FindGameObjectWithTag ("ScreenFader").GetComponent<ScreenFader> ();
		continueScreen.SetActive (false);
		waitTime = warningSystem.warningTime;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		//NewSwitch ();
	}
	
	// Update is called once per frame
	void Update () {
		if (gameGo && wheelF.gameend && !derailf) {
			ring.Stop ();
			ring.loop = false;
			ring.clip = derailSound;
			//ring.loop = false;
			ring.Play ();

			FinishCarThough ();
			derailf = true;
		}
		if (Input.GetKey (KeyCode.Q) && !waitForCar) {
			SendCarThrough ();
			waitForCar = true;
		}
	}
	public void StartGame()
	{
		//Cursor.visible = false;
		fpsController.enabled = true;
		NewSwitch ();
		gameGo = true;
	}
	public void NewSwitch()
	{
		waitForCar = false;
		/*time = timeDelay;
		if (outOfTime) {
			outOfTime = false;
			//StartCoroutine (OneSecond());
		}*/
		int random_number = Random.Range (1, 7);
		if(selectedTrackNum == random_number)
		{
			int nr = Random.Range(1,6);
			random_number += nr;
			if(random_number > 7)
			{
				random_number -= 7;
			}
		}
		
		selectedTrackNum = random_number;
		foreach (Text t in textList) {
			t.color = new Color (.9f, 0, 0);
		}
		textList [selectedTrackNum - 1].color = new Color (0, 1, .5f);
		alignText.text = "Align To Track " + selectedTrackNum;
		//timeText.text = "TIME LEFT: " + time.ToString ();

		Destroy (localcar);
		localcar = Instantiate (CarPrefab);
		wheelF = localcar.transform.FindChild ("FrontWheels").GetComponent<Wheels> ();
		wheelB = localcar.transform.FindChild ("RearWheels").GetComponent<Wheels> ();
		ring = wheelF.gameObject.GetComponent<AudioSource> ();

		wheelF.currentBezierHandler = currentBezierHandler;
		wheelB.currentBezierHandler = currentBezierHandler;
		wheelF.currentSpline = currentSpline;
		wheelB.currentSpline = currentSpline;
		//wheelB.maxSpeed = 5f;
		//wheelF.maxSpeed = 5f;
	}
	public void Fail ()
	{
		score.GetScoreSwitchGame ();
		if(completedlevels >= 2)
		{
			messageHolder.moduleFinished = true;
		}
		messageHolder.WriteMessage (completedlevels.ToString(), 4);
		messageHolder.PushingMessages();
		print ("6");
	}
	public void ReturnToMenu ()
	{
		score.GetScoreSwitchGame ();
		if(completedlevels >= 2)
		{
			messageHolder.moduleFinished = true;
		}
		messageHolder.WriteMessage (completedlevels.ToString(), 4);
		messageHolder.PushingMessages();
		print ("7");
	}
	public void GoOn()
	{
		continueScreen.SetActive (false);
		//Cursor.visible = false;
		fpsController.enabled = true;
		NewSwitch ();
		gameGo = true;
	}
	public void FinishCarThough()
	{
		int value = GetTrack ();
		if (value > 0) 
		{
			if(value == selectedTrackNum)
			{
				print ("Success: " + value);
				warningSystem.Pass (0);
				//messageHolder.WriteMessage ("Sent Car Down Correct Track", 2);
				completedlevels += 1;
				completedText.text = "Completed " + completedlevels.ToString () + "/2";
				if(completedlevels == 2)
				{
					continueScreen.SetActive (true);
					fpsController.enabled = false;
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
					gameGo = false;
				}
				else
				{
					NewSwitch ();
				}
			}
			else
			{
				print ("Wrong Track: " + value);
				warningSystem.Warn (2);
				//messageHolder.WriteMessage ("Sent Car Down Incorrect Track", 1);
				StartCoroutine (WaitForFail (waitTime));
				//Fail ();
			}
		} 
		else
		{
			print ("Derail on Switch: " + value);
			warningSystem.Warn (3);
			//messageHolder.WriteMessage ("Derailed Car", 1);
			StartCoroutine (WaitForFail (waitTime));
		}
	}
	public void SendCarThrough()
	{
		wheelF.accelerating = true;
		wheelB.accelerating = true;
		ring.Play ();
		print ("Send Car");
	}

	public int GetTrack()
	{
		int value = 0;
		if (switchList [0].locked == true && switchList [0].lined == true) 
		{
			if (switchList [0].onFarSide == true) //move to 2
			{
				if (switchList [2].locked == true && switchList [2].lined == true) 
				{
					if (switchList [2].onFarSide == true) //Track 3
					{
						//print ("Track: 3");
						value = 3;
						//end
					} 
					else //move to 4
					{
						if (switchList [4].locked == true && switchList [4].lined == true) 
						{
							if (switchList [4].onFarSide == true) //Track 1
							{
								//print ("Track: 1");
								value = 1;
								//end
							} 
							else //Track 2
							{
								//print ("Track: 2");
								value = 2;
								//end
							}
						}
						else //derail 4
						{
							//print ("Derail: 4");
							value = -4;
						}
					}
				}
				else //derail 2
				{
					//print ("Derail: 2");
					value = -2;
				}
			} 
			else //move to 1
			{
				if (switchList [1].locked == true && switchList [1].lined == true) 
				{
					if (switchList [1].onFarSide == true) //move to 3
					{
						if (switchList [3].locked == true && switchList [3].lined == true) 
						{
							if (switchList [3].onFarSide == true) //Track 7
							{
								//print ("Track: 7");
								value = 7;
								//end
							} 
							else //Track 6
							{
								//print ("Track: 6");
								value = 6;
								//end
							}
						}
						else //derail 3
						{
							//print ("Derail: 3");
							value = -3;
						}
					} 
					else //move to 5
					{
						if (switchList [5].locked == true && switchList [5].lined == true) 
						{
							if (switchList [5].onFarSide == true) //Track 5
							{
								//print ("Track: 5");
								value = 5;
								//end
							} 
							else //Track 4
							{
								//print ("Track: 4");
								value = 4;
								//end
							}
						}
						else //derail 5
						{
							//print ("Derail: 5");
							value = -5;
						}
					}
				}
				else //derail 1
				{
					//print ("Derail: 1");
					value = -1;
				}
			}
		}
		else //derail 0
		{
			//print ("Derail: 0");
		}
		return value;
	}
	/*IEnumerator OneSecond()
	{
		yield return new WaitForSeconds (1);
		time -= 1;
//		print (timeDelay);
		timeText.text = "TIME LEFT: " + time.ToString ();
		if (time > 0 && !fail) {
			StartCoroutine (OneSecond ());
		} 
		else if (!fail) {
			outOfTime = true;
			SendCarThrough ();
		}
	}*/
	public IEnumerator WaitForFail(float i)
	{
		fail = true;
		yield return new WaitForSeconds (i);
		Fail ();
	}
	public void Restart()
	{
		screenFader.EndScene ("Switch(Game)");
	}
}

