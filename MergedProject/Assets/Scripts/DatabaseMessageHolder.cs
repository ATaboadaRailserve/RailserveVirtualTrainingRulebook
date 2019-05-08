using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class DatabaseMessageHolder : MonoBehaviour {
	
	public enum Module {
		Intro,
		PP_PPE,
		PP_REAct,
		WK_PPE,
		LO_Refueling,
		WK_Refueling,
		TT_Take_Two,
		HZ_Handling_Hazardous_Materials,
		WK_Precautions,
		TM_Yard_Limits_Emergencies,
		EQ_Authorized_Employee,
		WK_Emergencies,
		IR_Incident_Reporting,
		CS_Radio_Communication,
		WK_Comms,
		GM_Comms_Emergencies,
		EQ_Equipment_On_Cars,
		WK_Identifying_Equipment,
		TM_Cars_Left_Standing,
		EQ_RidingCars,
		WK_Mounting_Brakes,
		GM_Car_Manipulation,
		TM_Checklist_Crossing,
		TM_Chocks,
		WK_Groundman_Spotting,
		LR_Loading_Racks,
		EQ_Dump_Box_Cars,
		WK_Cargo_Transfer,
		GM_Groundman_Duties,
		CP_Coupling_BrakeSystem,
		CP_Knuckle_OperatingLever,
		CS_Coupling_Callouts,
		WK_Coupling,
		EP_Bumpers_Derails,
		FD_Flags_Derails,
		WK_Bumpers_Derails,
		GM_Coupling_and_Derails,
		TM_Other_Crew_Eyes_Of_Move,
		TM_Movement_To_Spur_Track,
		WK_Eyes_of_the_Move,
		TM_Points_Look_Good,
		TM_Duties_Eyes_Of_Move,
		WK_Duties_of_Eyes_of_Move,
		GM_Eye_Duty,
		LO_Taking_Charge,
		LO_Restarting,
		WK_Startup_Procedure,
		EQ_Brake_Wheels,
		LB_Locomotive_Brake_Test,
		WK_Loco_Brakes,
		GM_Start_Brakes,
		LO_Leaf_Movement_Operations,
		WK_Movement,
		TM_Fouling_Track_Blockage,
		LO_Unusual_Track_Events,
		WK_Unusual_Track_Events,
		GM_Track_Awareness,
		RS_Restricted_Speed,
		TM_Road_Crossing,
		WK_Speed,
		CS_Whistle_Bell,
		LO_Lights_Noise,
		WK_Audio_Light,
		GM_Audio_Speed,
		LO_Unattended_Locomotive,
		LO_Shutting_Down,
		WK_Shutdown_Procedure,
		LO_Locomotive_Prep_Maintenance,
		LO_Locomotive_Inspection,
		WK_Loco_Inspection,
		GM_Shutdown_Inspection,
        GM_Comprehensive
	}
	
	[HideInInspector]
    private bool pushMessage = false;
    bool networkConnected = false;
    string startTime = "";
    string userName = "";
	[HideInInspector]
    public int procedureStatus = 0;
    [HideInInspector]
    public int gender = 3;
    [HideInInspector]
    public string gameFactors = "";
    [HideInInspector]
    public bool moduleFinished = false;

    string checkUserURL = "http://rsconnect.biz/UserData.php";
    string finishedURL = "http://rsconnect.biz/FinishedTutorial.php";

    [System.Serializable]
	public struct MessageHolder {
		public string userID;
		
		/* OLD
		public string moduleID;
		public char subModuleID;
		*/
		
		public int module;
		
        public string sessionID;
        public string messageTypeID;
		public string message;
	}
	
	//[HideInInspector]
	public List<MessageHolder> messages;
    public List<MessageHolder> messagesXML;
	/* OLD
	[Header("Index in the ID String")]
	public string moduleID;
	[Header("Letter to go in IDString[moduleID]")]
	public char subModuleID;
	*/
	
	public DataLoader dataLoader;
	private string userID;
	
    /* Message Type Reference
	
	Message Types:
	1 - Comment 	|		Non-critical information such as time started, time completed, etc.
	2 - Warning 	|		Any form of failure such as a rule break or section fail.
	3 - Credit		|		Any form of completion such as a section completion or individual tutorial task completion.
	4 - Score		|		Final score of games
	8 - CheckPoint  |       Checkpoint for the Uber game
	
	*/

    public IEnumerator ConnectToDB()
    {
        Debug.Log("Check Database Connection...");
        Debug.Log("Opening WWW...");
        WWW checkConnection = new WWW(checkUserURL);
        yield return checkConnection;
        if (checkConnection.error != null)
        {
			Debug.Log("Connection Not Established, Falling back to local mode");
            Debug.Log(checkConnection.error);
            networkConnected = false;
        }
        else
        {
            Debug.Log("Connection is Good!");
            networkConnected = true;
        }
        pushMessage = true;
    }

    void Awake () {
		if (!dataLoader) {
			if (!gameObject.GetComponent<DataLoader>()) {
				if (GameObject.FindGameObjectsWithTag("MessageHolder").Length == 1) {
					SceneManager.LoadScene("LogIn");
				} else {
					Destroy(gameObject);
				}
				return;
			} else
				dataLoader = gameObject.GetComponent<DataLoader>();
		}
		
		userName = dataLoader.CurrentUser.Username;
		procedureStatus = dataLoader.procedureStatus;
        gender = dataLoader.gender;
        gameFactors = dataLoader.gameFactors;
        Debug.Log(gameFactors);

        Debug.Log("dataLoader userName: " + userName + " | gender: " + gender + " | procedureStatus: " + procedureStatus);
		UpdateModules();

        SceneManager.sceneLoaded += OnSceneLoaded;
		
		messages = new List<MessageHolder>();
        messagesXML = new List<MessageHolder>();
    }
	
	void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		GameObject[] others = GameObject.FindGameObjectsWithTag("MessageHolder");
		for (int i = others.Length-1; i >= 0; i--) {
			if (others[i] == gameObject)
				continue;
			Destroy(others[i]);
		}
		
		if (userName == "") {
			userName = dataLoader.CurrentUser.Username;
			procedureStatus = dataLoader.procedureStatus;
            gender = dataLoader.gender;
            gameFactors = dataLoader.gameFactors;
			Debug.Log("dataLoader userName: " + userName + " | gender: " + gender + " | procedureStatus: " + procedureStatus + " | Game Factors: " + gameFactors);
		}
	}

    public void WriteMessage (string mess, int type = 1, bool pushImmediately = false) {
		// If this is a duplicate message, ignore it.
		Debug.Log("Checking for Dupes");
		if (type == 4) {
			for (int i = 0; i < messages.Count; i++) {
				if (messages[i].message == DateTime.Now.ToString() + " | " + mess) {
                    return;
				}
			}
		} else {
			for (int i = 0; i < messages.Count; i++) {
				if (messages[i].message == DateTime.Now.ToString() + " | " + mess) {
                    return;
				}
			}
		}
		
		MessageHolder temp = new MessageHolder();
		
		temp.userID = dataLoader.CurrentUser.userID;
		temp.sessionID = dataLoader.CurrentUser.sessionID;

        /* OLD
		temp.moduleID = moduleID;
		temp.subModuleID = subModuleID;
		*/
        if (GameObject.FindWithTag("WarningSystem"))
			temp.module = (int)GameObject.FindWithTag("WarningSystem").GetComponent<WarningSystem>().moduleName;
		else if (GameObject.FindWithTag("AnimationScrubber"))
			temp.module = (int)GameObject.FindWithTag("AnimationScrubber").GetComponent<AnimationScrubber>().moduleName;
		else {
			Debug.LogAssertion("No WarningSystem nor AnimationScrubber was found!");
			return;
		}
        Debug.Log(temp.module);

        
        
			
        temp.messageTypeID = type.ToString();
        if (type == 4)
        {
            UpdateGameFactors(int.Parse(mess), temp.module);
            temp.message = DateTime.Now.ToString() + " | " + mess;
        }
        else
            temp.message = DateTime.Now.ToString() + " | " + mess;
		
        messages.Add(temp);
        messagesXML.Add(temp);
		
		if (pushImmediately)
			PushingMessages();
	}
	
	public void PushMessageList () {
		/*
		if (messages.Count == 0) {
			Debug.LogWarning("Attempted to push empty message list");
			return;
		}
		*/

        if (networkConnected)
        {
			if (messages.Count == 0) {
				Debug.Log("Attempted to push no messages");
				GetComponent<PushToDB>().finishedPushing = true;
				return;
			}

            int count = 0;
            foreach (MessageHolder m in messages)
            {
                Debug.Log(count + ": " + m.message);
                count++;
            }

            //Pushing to XML
            GetComponent<PushToXML>().PushToXMLFile(messagesXML, userName, startTime);
            messagesXML.Clear();

            //Pushing to Database
            GetComponent<PushToDB>().PushToDatabase(messages, userName, startTime);

        }
        else
        {
            //Pushing to XML
            //foreach (DatabaseMessageHolder.MessageHolder m in messages)
            //{
            //    Debug.Log(m.message);
            //}
			
			if(messagesXML.Count == 0)
			{
				Debug.Log("Attempted to push no messages");
				GetComponent<PushToDB>().finishedPushing = true;
				return;
			}
			
            GetComponent<PushToXML>().PushToXMLFile(messagesXML, userName, startTime);
            messagesXML.Clear();
        }
    }
	
	// Call this method to push messages
	// Call this method to push messages
	// Call this method to push messages
    public void PushingMessages(string scene = "")
    {
        StartCoroutine(ConnectToDB());

        if (scene != "")
        {
            StartCoroutine(WaitForPush(scene));
        }
    }

    IEnumerator WaitForPush(string scene)
    {
        while (!GetComponent<PushToDB>().finishedPushing)
        {
            yield return null;
        }

        GetComponent<PushToDB>().finishedPushing = false;
        SceneManager.LoadScene(scene);
    }

	/* DEPRECATED
    public void FinishedTutorial()
    {
        StartCoroutine(FinishedTutorialFunction());
    }
	*/

    void Update () {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            StartCoroutine(ConnectToDB());
        }
#endif
		/* DEPRECATED
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            StartCoroutine(FinishedTutorialFunction());
        }
		*/
        if (pushMessage)
        {
			Debug.Log("Pushing Messages");
			/*
			if (messages.Count == 0) {
				pushMessage = false;
				
				return;
			}
			*/
            pushMessage = false;
            //moduleFinished = true;
            //WriteMessage("100", 4);
            PushMessageList();
        }
	}
	
	public void UpdateModules () {
		
		// *************************** //
		//                             //
		//     Needs to be updated     //
		// to match Andrew's new menu! //
		//                             //
		// *************************** //
		
		Debug.Log("Updating Modules: " + procedureStatus);
		GameObject[] modules = GameObject.FindGameObjectsWithTag("Module");
		for (int i = 0; i < modules.Length; i++) {
			print("Module: " + modules[i].name);
			modules[i].SendMessage("UpdateCheck", SendMessageOptions.DontRequireReceiver);
		}
	}
	
	public void UpdateStatus (int status) {
		print("Checking if " + procedureStatus + " is greater than " + status);
		if (procedureStatus > status) {
			print("It is!");
			return;
		} else
			procedureStatus = status+1;
		
		print("New Status: " + procedureStatus);
		
		dataLoader.procedureStatus = procedureStatus;
		dataLoader.CurrentUser.trainingPorcedure = procedureStatus;
		
		UpdateModules();
	}
	
	public static int[] IntToIntArray(int num)
	{
		if (num == 0)
			return new int[1] { 0 };

		List<int> digits = new List<int>();

		for (; num != 0; num /= 10)
			digits.Add(num % 10);

		int[] array = digits.ToArray();
		System.Array.Reverse(array);

		return array;
	}

    private void UpdateGameFactors(int finalgrade, int moduleID)
    {
        //15-0, 0; 21-0; 28-0, 0; 36-0; 43-0, 0
        string[] gameFactorElement = gameFactors.Split(';');

        string[] gameFactor_Element_15 = gameFactorElement[0].Split('-');
        string[] gameFactor_Element_21 = gameFactorElement[1].Split('-');
        string[] gameFactor_Element_28 = gameFactorElement[2].Split('-');
        string[] gameFactor_Element_36 = gameFactorElement[3].Split('-');
        string[] gameFactor_Element_43 = gameFactorElement[4].Split('-');

        string[] gameFactor_Attempt_15 = gameFactor_Element_15[1].Split(',');
        string[] gameFactor_Attempt_21 = gameFactor_Element_15[1].Split(',');
        string[] gameFactor_Attempt_28 = gameFactor_Element_28[1].Split(',');
        string[] gameFactor_Attempt_36 = gameFactor_Element_15[1].Split(',');
        string[] gameFactor_Attempt_43 = gameFactor_Element_43[1].Split(',');

        float gameFactor_15 = float.Parse(gameFactor_Attempt_15[0]);
        int gameAttempt_15 = int.Parse(gameFactor_Attempt_15[1]);
        float gameFactor_21 = float.Parse(gameFactor_Attempt_21[0]);
        int gameAttempt_21 = int.Parse(gameFactor_Attempt_21[1]);
        float gameFactor_28 = float.Parse(gameFactor_Attempt_28[0]);
        int gameAttempt_28 = int.Parse(gameFactor_Attempt_28[1]);
        float gameFactor_36 = float.Parse(gameFactor_Attempt_36[0]);
        int gameAttempt_36 = int.Parse(gameFactor_Attempt_36[1]);
        float gameFactor_43 = float.Parse(gameFactor_Attempt_43[0]);
        int gameAttempt_43 = int.Parse(gameFactor_Attempt_43[1]);

        if (moduleID == 15)
        {
            gameFactor_15 = (gameFactor_15 * gameAttempt_15 + finalgrade) / (gameAttempt_15 + 1);
            gameAttempt_15++;
        }
        else if (moduleID == 21)
        {
            gameFactor_21 = (gameFactor_21 * gameAttempt_21 + finalgrade) / (gameAttempt_21 + 1);
            gameAttempt_21++;
        }
        else if (moduleID == 28)
        {
            gameFactor_28 = (gameFactor_28 * gameAttempt_28 + finalgrade) / (gameAttempt_28 + 1);
            gameAttempt_28++;
        }
        else if (moduleID == 36)
        {
            gameFactor_36 = (gameFactor_36 * gameAttempt_36 + finalgrade) / (gameAttempt_36 + 1);
            gameAttempt_36++;
        }
        else if (moduleID == 43)
        {
            gameFactor_43 = (gameFactor_43 * gameAttempt_43 + finalgrade) / (gameAttempt_43 + 1);
            gameAttempt_43++;
        }

        gameFactors = "15-" + gameFactor_15 + ", " + gameAttempt_15 + "; 21-" + gameFactor_21 + ", " + gameAttempt_21 + "; 28-" + gameFactor_28 + ", " + gameAttempt_28 + "; 36-" + gameFactor_36 + ", " + gameAttempt_36 + "; 43-" + gameFactor_43 + ", " + gameAttempt_43;
    }
	
	public void CompleteModule (bool pushImmediately = true) {
		WriteMessage("100", 4);
		if (pushImmediately)
			PushingMessages();
		if (GameObject.FindWithTag("WarningSystem")) {
			print((int)GameObject.FindWithTag("WarningSystem").GetComponent<WarningSystem>().moduleName + ": " + GameObject.FindWithTag("WarningSystem").GetComponent<WarningSystem>().moduleName);
			UpdateStatus((int)GameObject.FindWithTag("WarningSystem").GetComponent<WarningSystem>().moduleName);
		} else if (GameObject.FindWithTag("AnimationScrubber")) {
			print((int)GameObject.FindWithTag("AnimationScrubber").GetComponent<AnimationScrubber>().moduleName + ": " + GameObject.FindWithTag("AnimationScrubber").GetComponent<AnimationScrubber>().moduleName);
			UpdateStatus((int)GameObject.FindWithTag("AnimationScrubber").GetComponent<AnimationScrubber>().moduleName);
		} else {
			Debug.LogAssertion("No WarningSystem nor AnimationScrubber was found!");
			return;
		}
	}
	
	public void FailedModule (bool pushImmediately = true) {
		WriteMessage("0", 4);
		if (pushImmediately)
			PushingMessages();
	}
}