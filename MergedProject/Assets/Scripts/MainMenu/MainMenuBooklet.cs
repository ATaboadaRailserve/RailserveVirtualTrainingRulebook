using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuBooklet : MonoBehaviour {

	public Color disabledColor, enabledColor, currentColor, nextColor;
	public Color disabledGraphicColor, enabledGraphicColor;
	public float breathTime = 1.0f;
	
	public Transform pageOne;
	public Transform pageTwo;
	public Transform pageThree;

	public List<GameObject> submenus;
	public List<GameObject> finmenus;
	
	/* PROCEDURE STATUS LIST
	index shift left 1 for buttons (ie buttons[0] == PPE)
	00 ?
	Intro
	  PPE
		01 PPE
		02 React
		03 Walkthrough
	  Refueling
		04 Refueling
		05 Walkthrough
	  Precautions
		06 Take Two
		07 Hazardous Materials
		08 Walkthrough
	Comms & Emer
	  Emerg
		09 Yard Limits
		10 Authorized Employee
		11 Walkthrough
	  Comms
		12 Incident Reporting
		13 Radio Communication
		14 Walkthrough
	  15 Game
	Car Manip
	  Identifying
		16 Equipment On Cars
		17 Walkthrough
	  Mounting
		18 Cars Left Standing
		19 Riding Cars
		20 Walkthrough
	  21 Game
	Groundman Duties
	  Groundman Spotting
		22 Checklist & Crossing
		23 Chocks
		24 Walkthrough
	  Cargo Transfer
		25 Loading Racks
		26 Dump & Box Cars
		27 Walkthrough
	  28 Game
	Coupling
	  Coupling
		29 Coupling & Brakes
		30 Knuckle & Operating
		31 Coupling Callouts
		32 Walkthrough
	  Bumpers
		33 Bumpers & Derails
		34 Flags & Derails
		35 Walkthrough
	  36 Game
	Eyes
	  Eyes of the Move
		37 Other Crew & Eyes
		38 Movement into Spur Tracks
		39 Walkthrough
	  Duties
		40 Points Look Good
		41 Duties of Eyes of the Move
		42 Walkthrough
	  43 Game
	Uber
	  44 Game
	*/
	
	public List<BasicInteractable> buttons;
	private int procedureStatus = 0;
	private int nextButtonPageOne = -1;
	private int nextButtonPageTwo = -1;
	private int nextButtonPageThree = -1;
	private int selectedButtonPageOne = -1;
	private int selectedButtonPageTwo = -1;
	private int lastSelectedButtonPageOne = -1;
	private int lastSelectedButtonPageTwo = -2;
	
	public InteractionHandler.InvokableState OnFemaleGender;
	public InteractionHandler.InvokableState OnMaleGender;
	private int gender;
	
	public int ProcedureStatus
	{
		get
		{
			return progressTranslation[procedureStatus];
		}
	}
	
//	private Vector3 p1Pos;
//	private Quaternion p1Rot;
//	private Vector3 p1Sca;
	
	private Vector3 p2Pos;
	private Quaternion p2Rot;
	private Vector3 p2Sca;

	bool isPageThreeOpen = false;
	
	int BUTTONS_COUNT = 64;
	
	// buttons enumerated
	const int NAV_INTRO = 0;
	const int NAV_PPE = 1;
	const int VID_PPE = 2;
	const int VID_REACT = 3;
	const int WK_PPE = 4;
	const int NAV_REFUELING = 5;
	const int VID_REFUELING = 6;
	const int WK_REFUELING = 7;
	const int NAV_PRECAUTIONS = 8;
	const int VID_TAKE_TWO = 9;
	const int VID_HAZARDOUS_MATERIALS = 10;
	const int WK_PRECAUTIONS = 11;
	const int NAV_COMMS_EMERGENCIES = 12;
	const int NAV_EMERGENCIES = 13;
	const int VID_YARD_LIMITS = 14;
	const int VID_AUTHORIZED_EMPLOYEE = 15;
	const int WK_EMERGENCIES = 16;
	const int NAV_COMMS = 17;
	const int VID_INCIDENT_REPORTING = 18;
	const int VID_RADIO_COMMUNICATION = 19;
	const int WK_COMMS = 20;
	const int GM_COMMS_EMERGENCIES = 21;
	const int NAV_CAR_MANIPULATION = 22;
	const int NAV_IDENTIFYING_EQUIPMENT = 23;
	const int VID_EQUIPMENT_ON_CARS = 24;
	const int WK_IDENTIFYING_EQUIPMENT = 25;
	const int NAV_MOUNTING = 26;
	const int VID_CARS_LEFT_STANDING = 27;
	const int VID_RIDING_CARS = 28;
	const int WK_MOUNTING = 29;
	const int GM_CAR_MANIPULATION = 30;
	const int NAV_GROUNDMAN_DUTIES = 31;
	const int NAV_GROUNDMAN_SPOTTING = 32;
	const int VID_CHECKLIST_CROSSING = 33;
	const int VID_CHOCKS = 34;
	const int WK_GROUNDMAN_SPOTTING = 35;
	const int NAV_CARGO_TRANSFER = 36;
	const int VID_LOADING_RACKS = 37;
	const int VID_DUMP_CARS = 38;
	const int WK_CARGO_TRANSFER = 39;
	const int GM_GROUNDMAN_DUTIES = 40;
	const int NAV_COUPLING_BUMPERS = 41;
	const int NAV_COUPLING = 42;
	const int VID_COUPLING_BRAKES = 43;
	const int VID_KNUCKLE_OPERATING = 44;
	const int VID_COUPLING_CALLOUTS = 45;
	const int WK_COUPLING = 46;
	const int NAV_BUMPERS_FLAGS = 47;
	const int VID_BUMPERS_DERAILS = 48;
	const int VID_FLAGS_DERAILS = 49;
	const int WK_BUMPERS_FLAGS = 50;
	const int GM_COUPLING_BUMPERS = 51;
	const int NAV_EYES_DUTIES = 52;
	const int NAV_EYES_OF_THE_MOVE = 53;
	const int VID_OTHER_CREW = 54;
	const int VID_MOVEMENT_INTO_SPUR_TRACKS = 55;
	const int WK_EYES_OF_THE_MOVE = 56;
	const int NAV_DUTIES = 57;
	const int VID_POINTS_LOOK_GOOD = 58;
	const int VID_DUTIES_OF_EYES = 59;
	const int WK_DUTIES = 60;
	const int GM_EYES_DUTIES = 61;
	const int NAV_UBER = 62;
	const int GM_UBER = 63;
	const int AllDone = 72;
	
	int [] navigationIndexes = {NAV_INTRO, NAV_PPE, NAV_REFUELING, NAV_PRECAUTIONS, NAV_COMMS_EMERGENCIES, NAV_EMERGENCIES, NAV_COMMS,
		NAV_CAR_MANIPULATION, NAV_IDENTIFYING_EQUIPMENT, NAV_MOUNTING, NAV_GROUNDMAN_DUTIES, NAV_GROUNDMAN_SPOTTING, NAV_CARGO_TRANSFER,
		NAV_COUPLING_BUMPERS, NAV_COUPLING, NAV_BUMPERS_FLAGS, NAV_EYES_DUTIES, NAV_EYES_OF_THE_MOVE, NAV_DUTIES, NAV_UBER};
	
	int [] videoIndexes = {VID_PPE, VID_REACT, VID_REFUELING, VID_TAKE_TWO, VID_HAZARDOUS_MATERIALS, VID_YARD_LIMITS, VID_AUTHORIZED_EMPLOYEE, VID_INCIDENT_REPORTING,
		VID_RADIO_COMMUNICATION, VID_EQUIPMENT_ON_CARS, VID_CARS_LEFT_STANDING, VID_RIDING_CARS, VID_CHECKLIST_CROSSING, VID_CHOCKS, VID_LOADING_RACKS,
		VID_DUMP_CARS, VID_COUPLING_BRAKES, VID_KNUCKLE_OPERATING, VID_COUPLING_CALLOUTS, VID_BUMPERS_DERAILS, VID_FLAGS_DERAILS, VID_OTHER_CREW,
		VID_MOVEMENT_INTO_SPUR_TRACKS, VID_POINTS_LOOK_GOOD, VID_DUTIES_OF_EYES};
		
	int [] walkthroughIndexes = {WK_PPE, WK_REFUELING, WK_PRECAUTIONS, WK_EMERGENCIES, WK_COMMS, WK_IDENTIFYING_EQUIPMENT, WK_MOUNTING, WK_GROUNDMAN_SPOTTING,
		WK_CARGO_TRANSFER, WK_COUPLING, WK_BUMPERS_FLAGS, WK_EYES_OF_THE_MOVE, WK_DUTIES};
	
	int [] gameIndexes = {GM_COMMS_EMERGENCIES, GM_CAR_MANIPULATION, GM_GROUNDMAN_DUTIES, GM_COUPLING_BUMPERS, GM_EYES_DUTIES, GM_UBER};
	
	int [] progressTranslation = { -1, 2, 4, 4, 6, 7, 9, 10, 11, 14, 15, 16, 18, 19, 20, 21, 24, 25, 27, 28, 29, 30, 33, 34, 35, 37, 38, 39, 40, 43, 44, 45, 46, 48, 49, 50, 51, 54, 55, 56, 58, 59, 60, 61, 63, 72};
	
	int [] pageOneNavIndexes = {NAV_INTRO, NAV_COMMS_EMERGENCIES, NAV_CAR_MANIPULATION, NAV_GROUNDMAN_DUTIES, NAV_COUPLING_BUMPERS, NAV_EYES_DUTIES, NAV_UBER};
	
	int [] pageTwoNavIndexes = {NAV_PPE, NAV_REFUELING, NAV_PRECAUTIONS, NAV_EMERGENCIES, NAV_COMMS, NAV_IDENTIFYING_EQUIPMENT, NAV_MOUNTING, NAV_GROUNDMAN_SPOTTING,
		NAV_CARGO_TRANSFER, NAV_COUPLING, NAV_BUMPERS_FLAGS, NAV_EYES_OF_THE_MOVE, NAV_DUTIES};
	
	public void Awake()
	{
//		p1Pos = pageOne.transform.localPosition;
//		p1Rot = pageOne.transform.localRotation;
//		p1Sca = pageOne.transform.localScale;
		
		p2Pos = pageTwo.transform.localPosition;
		p2Rot = pageTwo.transform.localRotation;
		p2Sca = pageTwo.transform.localScale;
		
//		pageThree.transform.position = p2Pos;
//		pageThree.transform.rotation = p2Rot;
//		pageThree.transform.localScale = p2Sca;
		
		pageThree.gameObject.SetActive(false);
		
		ImportDatabaseInfo();
		ResetButtons();
	}
	
	public void Update()
	{
		if (isPageThreeOpen) {
			pageTwo.transform.position = pageOne.transform.position;
			pageTwo.transform.rotation = pageOne.transform.rotation;
		}

		if(CheatcodeExecuter.CheatsEnabled)
		{
			if (Input.GetKeyDown(KeyCode.Minus))
			{
				procedureStatus--;
				if (procedureStatus < 0)
					procedureStatus = 0;
				ResetButtons();
				DisableFinmenus();
				DisableSubmenus();
				ViewPageOne ();
			}
			else if (Input.GetKeyDown(KeyCode.Equals))
			{
				procedureStatus++;
				if(procedureStatus > 45)
					procedureStatus = 45;
				ResetButtons();
				DisableFinmenus();
				DisableSubmenus();
				ViewPageOne ();
			}
			else if (Input.GetKeyDown(KeyCode.Asterisk))
			{
				procedureStatus = 0;
				ResetButtons();
				DisableFinmenus();
				DisableSubmenus();
				ViewPageOne ();
			}
		}
		
		Color nextLerpedColor = Color.Lerp(enabledColor, nextColor, Mathf.PingPong(Time.time, breathTime));
		if(nextButtonPageOne != -1 && selectedButtonPageOne != nextButtonPageOne)
			buttons[nextButtonPageOne].GetComponent<Image>().color = nextLerpedColor;
		if(nextButtonPageTwo != -1 && selectedButtonPageTwo != nextButtonPageTwo)
			buttons[nextButtonPageTwo].GetComponent<Image>().color = nextLerpedColor;
		if(nextButtonPageThree != -1)
			buttons[nextButtonPageThree].GetComponent<Image>().color = nextLerpedColor;
	}
	
	public void ViewPageOne()
	{
		pageOne.gameObject.SetActive(true);
		pageThree.gameObject.SetActive(false);
		
//		pageTwo.transform.position = p2Pos;
//		pageTwo.transform.rotation = p2Rot;
//		pageTwo.transform.localScale = p2Sca;

		isPageThreeOpen = false;

		pageTwo.transform.localPosition = p2Pos;
		pageTwo.transform.localRotation = p2Rot;
		pageTwo.transform.localScale = p2Sca;
	}
	
	public void ViewPageThree()
	{
		print ("PAGE 3 IS OPEN");
		pageOne.gameObject.SetActive(false);
		pageThree.gameObject.SetActive(true);
		
//		pageTwo.transform.position = p1Pos;
//		pageTwo.transform.rotation = p1Rot;
//		pageTwo.transform.localScale = p1Sca;
		isPageThreeOpen = true;
	}
	
	public void ImportDatabaseInfo()
	{
		procedureStatus = 0;
		DataLoader dataLoader = GameObject.FindGameObjectWithTag("MessageHolder").GetComponent<DataLoader>();
		if(dataLoader == null)
		{
			Debug.LogAssertion("Menu cannot find DataLoader");
			return;
		}
		else {
			procedureStatus = Mathf.Min(dataLoader.procedureStatus, 45);
			print (dataLoader.procedureStatus);
		}
		Debug.Log("Menu procedureStatus read as: " + procedureStatus);
		
		if(dataLoader.gender <= 1)
			OnFemaleGender.Invoke();
		else
			OnMaleGender.Invoke();
	}
	
	public void ResetButtons()
	{
		selectedButtonPageOne = -1;
		selectedButtonPageTwo = -1;
		nextButtonPageOne = -1;
		nextButtonPageTwo = -1;
		nextButtonPageThree  = -1;
		
		for(int i = 0; i < BUTTONS_COUNT && i < buttons.Count; i++)
		{
			ResetButton(i);
		}
		
		for (int i = 0; i < pageOneNavIndexes.Length; i++)
		{
			if (pageOneNavIndexes[i] <= progressTranslation[procedureStatus])
				nextButtonPageOne = pageOneNavIndexes[i];
		}
		
		for (int i = 0; i < pageTwoNavIndexes.Length; i++)
		{
			if (pageTwoNavIndexes[i] <= progressTranslation[procedureStatus])
				nextButtonPageTwo = pageTwoNavIndexes[i];
		}
		
		bool isGameDay = false;
		
		for(int i = 0; i < gameIndexes.Length; i++)
		{
			if(gameIndexes[i] <= progressTranslation[procedureStatus] && gameIndexes[i] > nextButtonPageTwo)
			{
				nextButtonPageTwo = gameIndexes[i];
				isGameDay = true;
			}
		}
		
		if (!isGameDay)
		{
			for (int i = 0; i < videoIndexes.Length; i++)
			{
				if(videoIndexes[i] <= progressTranslation[procedureStatus])
					nextButtonPageThree = videoIndexes[i];
			}
			
			for (int i = 0; i < walkthroughIndexes.Length; i++)
			{
				if(walkthroughIndexes[i] <= progressTranslation[procedureStatus] && walkthroughIndexes[i] > nextButtonPageThree)
					nextButtonPageThree = walkthroughIndexes[i];
			}
		}
	}
	
	void ResetButton(int buttonIndex)
	{
		if (buttonIndex <= progressTranslation[procedureStatus])
		{
			if (ArrayContains(gameIndexes, buttonIndex) || ArrayContains(walkthroughIndexes, buttonIndex))
				SetButtonEnabled(buttons[buttonIndex], true, enabledGraphicColor);
			else
				SetButtonEnabled(buttons[buttonIndex], true, enabledColor);
		}
		else
		{
			if (ArrayContains(gameIndexes, buttonIndex) || ArrayContains(walkthroughIndexes, buttonIndex))
				SetButtonEnabled(buttons[buttonIndex], false, disabledGraphicColor);
			else
				SetButtonEnabled(buttons[buttonIndex], false, disabledColor);
		}
	}
	
	void SetButtonEnabled(BasicInteractable button, bool isEnabled, Color color)
	{
		Image image = button.GetComponent<Image>();
		button.isInteractiable = isEnabled;
		image.color = color;
	}
	
	bool ArrayContains(int [] source, int value)
	{
		for (int i = 0; i < source.Length; i++)
		{
			if (source[i] == value)
				return true;
		}
		return false;
	}
	
	public void SetCurrentButton(int index)
	{
		if (index < 0 || index >= BUTTONS_COUNT)
			return;
		
		if (selectedButtonPageOne >= 0)
			ResetButton(selectedButtonPageOne);
		
		selectedButtonPageOne = -1;
		for (int i = 0; i < pageOneNavIndexes.Length; i++)
		{
			if(pageOneNavIndexes[i] <= index)
				selectedButtonPageOne = pageOneNavIndexes[i];
		}
		
		if (selectedButtonPageOne >= 0)
			buttons[selectedButtonPageOne].GetComponent<Image>().color = currentColor;
		
		if (selectedButtonPageTwo >= 0)
			ResetButton(selectedButtonPageTwo);
		
		selectedButtonPageTwo = -1;
		for(int i = 0; i < pageTwoNavIndexes.Length; i++)
		{
			if(pageTwoNavIndexes[i] <= index)
				selectedButtonPageTwo = pageTwoNavIndexes[i];
		}
		
		if (selectedButtonPageTwo >= 0)
			buttons[selectedButtonPageTwo].GetComponent<Image>().color = currentColor;
		
		if (ArrayContains(pageTwoNavIndexes, index))
			ViewPageThree();
	}
	
	public void EnableSubmenu(int index)
	{
		if(index < 0 || index >= submenus.Count)
			return;

		DisableSubmenus();
		
		submenus[index].SetActive(true);
	}
	
	public void DisableSubmenus()
	{
		DisableFinmenus();
		
		foreach(GameObject s in submenus)
			s.SetActive(false);
	}
	
	public void EnableFinmenu(int index)
	{
		if(index < 0 || index >= finmenus.Count)
			return;

		DisableFinmenus();
		
		finmenus[index].SetActive(true);
	}
	
	public void DisableFinmenus()
	{
		foreach(GameObject f in finmenus)
			f.SetActive(false);
	}
}
