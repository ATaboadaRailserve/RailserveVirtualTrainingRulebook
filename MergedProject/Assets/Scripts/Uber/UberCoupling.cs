using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UberCoupling : MonoBehaviour {
	
	[System.Serializable]
	public class CoupleScenario
	{
		public string scenarioName;
		public UberCouplingCar LocomotiveAttachedCar;
		public UberCouplingCar NewCarToBeAttached;
		public InteractionHandler.InvokableState OnScenarioStart;
		public InteractionHandler.InvokableState OnScenarioEnd;
		public InteractionHandler.InvokableState OnScenarioStopped;
		public InteractionHandler.InvokableState OnScenarioReset;
		
		public void StartScenario()
		{
			LocomotiveAttachedCar.StartScenario(true);
			NewCarToBeAttached.StartScenario(false);
			OnScenarioStart.Invoke();
		}
		
		public void EndScenario()
		{
			LocomotiveAttachedCar.EndScenario();
			NewCarToBeAttached.EndScenario();
			OnScenarioEnd.Invoke();
		}
		
		public void EngageKnuckle()
		{
			LocomotiveAttachedCar.TryEngageRearCouple();
			NewCarToBeAttached.TryEngageFrontCouple();
		}
		
		public void StretchSlack()
		{
			LocomotiveAttachedCar.TryStretchRearSlack();
			NewCarToBeAttached.TryStretchFrontSlack();
		}
		
		public void AttachAirHoses()
		{
			LocomotiveAttachedCar.TryAttachRearAirHose();
			NewCarToBeAttached.TryAttachFrontAirHose();
		}
		
		public void OpenAngleCocks()
		{
			LocomotiveAttachedCar.TryOpenRearAngleCock();
			NewCarToBeAttached.TryOpenFrontAngleCock();
		}
		
		public void Reset()
		{
			OnScenarioReset.Invoke();
		}
		
		public void Initialize(UberCoupling uc, UberCouplingModule ucm)
		{
			LocomotiveAttachedCar.Initialize(uc, ucm);
			NewCarToBeAttached.Initialize(uc, ucm);
		}
		
		public bool IsSecureCouple()
		{
			return LocomotiveAttachedCar.RearAngleCockOpened && NewCarToBeAttached.FrontAngleCockOpened;
		}
		
		public void DisableInteraction()
		{
			LocomotiveAttachedCar.DisableRearInteraction();
			NewCarToBeAttached.DisableFrontInteraction();
		}
	}
	
	// Constants
	const float CarLength = 11.66238f;			// Meters
	const float FlexibleHalfWidthLarge = 6.0f;	// Feet
	const float FlexibleHalfWidthMedium = 3.0f;	// Feet
	const float FlexibleHalfWidthSmall = 1.5f;	// Feet
	const float MetersToFeet = 3.28084f;
	
	[Header("Module Reference")]
	public UberCouplingModule module;
	
	// Distances
	public float DistanceInMeters {get; private set;}
	public float DistanceInFeet {get{return DistanceInMeters * MetersToFeet;}}
	public float DistanceInCars {get{return DistanceInMeters / CarLength;}}
	
	// Scenario Management
	[Header("Scenario Management")]
	public CoupleScenario[] scenarios;
	Coroutine activeScenarioCoroutine;
	CoupleScenario activeScenario;
	int activeScenarioIndex;
	bool didHalfWayStop = false;
	float lastCallDistance = Mathf.Infinity;
	
	// Radio Calls
	float[] carLengthCalls  = {1,2,3,4,5,6,7,8,9,10};
	float[] feetLengthCalls = {2,5,10,15,26};
	
	// UI
	[Header("UI")]
	public Text UIDistance;
	public Image UIDistanceBackground;
	public Color yellow;
	public Color red;
	public float flashRate = 0.5f;
	
	// Debug
	[Header("Debug")]
	public bool debugMode;
	public bool automaticDistanceCalling;
	public GameObject debugWindow;
	public GameObject debugLLeverOpen;
	public GameObject debugNLeverOpen;
	public GameObject debugLCoupleOpen;
	public GameObject debugNCoupleOpen;
	public GameObject debugHalfwayStop;
	public GameObject debugLKnuckleEngaged;
	public GameObject debugNKnuckleEngaged;
	public GameObject debugLSlackStretched;
	public GameObject debugNSlackStretched;
	public GameObject debugLAirHoseAttached;
	public GameObject debugNAirHoseAttached;
	public GameObject debugLAngleCockOpened;
	public GameObject debugNAngleCockOpened;
	
	// Events
	[Header("Any Scenario Events")]
	public InteractionHandler.InvokableState OnAnyScenarioStart;
	public InteractionHandler.InvokableState OnAnyScenarioEnd;
	public InteractionHandler.InvokableState OnAnyScenarioStopped;
	
	[Header("Failure Events")]
	public InteractionHandler.InvokableState OnMissedCall;
	public InteractionHandler.InvokableState OnMissedHalfwayStop;
	
	// Unity Callbacks
	private void Start()
	{
		foreach(CoupleScenario s in scenarios)
			s.Initialize(this, module);
		debugWindow.SetActive(false);
	}
	
	private void Update()
	{
		if(!debugMode)
			return;
		debugWindow.SetActive(activeScenario != null);
		if(debugWindow.activeSelf)
		{
			debugLLeverOpen.SetActive(activeScenario.LocomotiveAttachedCar.RearCoupleLeverOpened);
			debugNLeverOpen.SetActive(activeScenario.NewCarToBeAttached.FrontCoupleLeverOpened);
			debugLCoupleOpen.SetActive(activeScenario.LocomotiveAttachedCar.RearCoupleOpened);
			debugNCoupleOpen.SetActive(activeScenario.NewCarToBeAttached.FrontCoupleOpened);
			debugHalfwayStop.SetActive(didHalfWayStop);
			debugLKnuckleEngaged.SetActive(activeScenario.LocomotiveAttachedCar.RearKnuckleEngaged);
			debugNKnuckleEngaged.SetActive(activeScenario.NewCarToBeAttached.FrontKnuckleEngaged);
			debugLSlackStretched.SetActive(activeScenario.LocomotiveAttachedCar.RearSlackStretched);
			debugNSlackStretched.SetActive(activeScenario.NewCarToBeAttached.FrontSlackStretched);
			debugLAirHoseAttached.SetActive(activeScenario.LocomotiveAttachedCar.RearAirHoseAttached);
			debugNAirHoseAttached.SetActive(activeScenario.NewCarToBeAttached.FrontAirHoseAttached);
			debugLAngleCockOpened.SetActive(activeScenario.LocomotiveAttachedCar.RearAngleCockOpened);
			debugNAngleCockOpened.SetActive(activeScenario.NewCarToBeAttached.FrontAngleCockOpened);
		}
	}
	
	// Helper functions
	private float Average(float a, float b) {return (a+b) / 2f;}
	
	// Scenario Functions
	public void ResetScenario(int i)
	{
		if(i < 0 || i > scenarios.Length - 1)
		{
			Debug.LogError("Attempted to reset scenario out of index");
			return;
		}
		scenarios[i].Reset();
	}
	
	public void StartScenario(string scenarioName)
	{
		for(int i = 0; i < scenarios.Length; i++)
		{
			if(scenarioName == scenarios[i].scenarioName)
			{
				StartScenario(i);
				return;
			}
		}
		Debug.LogError("Scenario \'" + scenarioName + "\' not found");
	}
	
	public void StartScenario(int index)
	{
		if(index < 0 || index >= scenarios.Length)
		{
			Debug.LogError("Tried to start invalid scenario index \'" + index + "\'");
			return;
		}
		
		StopScenario();
		activeScenarioCoroutine = StartCoroutine(DoScenario(scenarios[index]));
	}
	
	public void StopScenario()
	{
		if(activeScenarioCoroutine != null)
		{
			StopCoroutine(activeScenarioCoroutine);
			activeScenario.OnScenarioStopped.Invoke();
			OnAnyScenarioStopped.Invoke();
		}
	}
	
	private IEnumerator DoScenario(CoupleScenario sc)
	{
		print("Scenario Started");
		// Assign Active Scenario
		activeScenario = sc;
		
		// Do Events
		OnAnyScenarioStart.Invoke();
		sc.StartScenario();
		
		// Show UI
		if(UIDistance != null && UIDistanceBackground != null)
		{
			UIDistance.gameObject.SetActive(true);
			UIDistanceBackground.gameObject.SetActive(true);
		}
		
		// Reset Distance Calling
		bool isCoupled = false;
		bool usingFeetCalls = false;
		bool needsCall = false;
		bool wasCalled = false;
		didHalfWayStop = false;
		int activeCallIndex = carLengthCalls.Length - 1;
		lastCallDistance = Mathf.Infinity;
		Color currentColor = yellow;
		float flash_t = 0.0f;
		DistanceInMeters = Vector3.Distance(sc.LocomotiveAttachedCar.rearCouplePoint.position, sc.NewCarToBeAttached.frontCouplePoint.position);
		
		if(DistanceInCars < 1)
		{
			usingFeetCalls = true;
			for(int i = 0; i < feetLengthCalls.Length; i++)
			{
				if(feetLengthCalls[i] < DistanceInFeet)
					activeCallIndex = i;
			}
		}
		else
		{
			for(int i = 0; i < carLengthCalls.Length; i++)
			{
				if(carLengthCalls[i] * CarLength < DistanceInMeters)
				{
					activeCallIndex = i;
				}
			}
		}
		
		// Main Loop
		while(!isCoupled)
		{
			// Update Values
			needsCall = false;
			wasCalled = false;
			DistanceInMeters = Vector3.Distance(sc.LocomotiveAttachedCar.rearCouplePoint.position, sc.NewCarToBeAttached.frontCouplePoint.position);
			flash_t += Time.deltaTime;
			if(UIDistance)
				UIDistance.text = "Meters: " + DistanceInMeters.ToString("0.00") + "m | Cars: " + DistanceInCars.ToString("0.00")  + " | Feet: " + DistanceInFeet.ToString("0.00")  + "ft | Last Call: " + lastCallDistance.ToString("0.00") + "ft";
			
			//if(Application.isEditor && automaticDistanceCalling)
				lastCallDistance = DistanceInFeet;
			
			if(usingFeetCalls)
			{	
				// Prepare target value and flex
				float targetInFeet = feetLengthCalls[activeCallIndex];
				float currentFlex = FlexibleHalfWidthMedium;
				if (DistanceInFeet < 10)
					currentFlex = FlexibleHalfWidthSmall;
				
				// Is a call currently necessary
				if(DistanceInFeet < targetInFeet + currentFlex)
					needsCall = true;
				
				// Has a call already been made
				if(lastCallDistance < targetInFeet + currentFlex)
					wasCalled = true;
				
				// Check call success / failure
				if(DistanceInFeet < targetInFeet - currentFlex)
				{
					// Missed call
					if(lastCallDistance > targetInFeet + currentFlex)
					{
						OnMissedCall.Invoke();
					}
					else // Call is accepted
					{
						// Move calling distance index
						activeCallIndex--;
						
						// check if coupled
						if(activeCallIndex < 0)
						{
							activeCallIndex = 0;
							isCoupled = true;
							Debug.Log("Coupled");
						}
						else
						{
							Debug.Log("Next Distance: " + feetLengthCalls[activeCallIndex]);
						}
							
					}
				}
			}
			else
			{
				// Prepare target value
				float targetInFeet = carLengthCalls[activeCallIndex] * CarLength * MetersToFeet;
				
				// Is a call currently necessary
				if(DistanceInFeet < targetInFeet + FlexibleHalfWidthLarge)
					needsCall = true;
				
				// Has a call already been made
				if(lastCallDistance < targetInFeet + FlexibleHalfWidthLarge)
					wasCalled = true;
				
				// Check call success / failure
				if(DistanceInFeet < targetInFeet - FlexibleHalfWidthLarge)
				{
					// Missed call
					if(lastCallDistance > targetInFeet + FlexibleHalfWidthLarge)
					{
						OnMissedCall.Invoke();
					}
					else // Call is accepted
					{
						activeCallIndex--;
						if(activeCallIndex < 0) // Transition to feet calls
						{
							usingFeetCalls = true;
							activeCallIndex = feetLengthCalls.Length-1;
							Debug.Log("Next Distance: " + feetLengthCalls[activeCallIndex]);
						}
						else
						{
							Debug.Log("Next Distance: " + carLengthCalls[activeCallIndex] * CarLength * MetersToFeet);
						}
					}
				}
			}
			
			/* Old method
			// Check for Halfway Stop
			if(DistanceInCars > 1)
				didHalfWayStop = false;
			else if(DistanceInFeet < 26 && !didHalfWayStop)
				OnMissedHalfwayStop.Invoke();
			*/
			
			// Update Color
			if(needsCall && !wasCalled)
				currentColor = red;
			else if(currentColor == red)
			{
				currentColor = yellow;
				flash_t = 0.0f;
			}
			
			if(UIDistanceBackground)
				UIDistanceBackground.color = Color.Lerp(yellow, currentColor, Mathf.PingPong(flash_t, flashRate) / flashRate);
			
			yield return null;
		}
		
		// Hide UI
		if(UIDistance != null && UIDistanceBackground != null)
		{
			UIDistance.gameObject.SetActive(false);
			UIDistanceBackground.gameObject.SetActive(false);
		}
		
		// Wait for Angle Cocks to be opened
		while(!sc.IsSecureCouple())
			yield return null;
		
		sc.DisableInteraction();
		
		sc.EndScenario();
		OnAnyScenarioEnd.Invoke();
		
		activeScenario = null;
		activeScenarioCoroutine = null;
		print("Scenario Ended");
	}

	// Functions called by scenario cars
	public void Scenario_EngageKnuckle()
	{
		if(activeScenario != null)
		{
			activeScenario.EngageKnuckle();
		}
	}
	
	public void Scenario_AttachAirHoses()
	{
		if(activeScenario != null)
		{
			activeScenario.AttachAirHoses();
		}
	}
	
	public void Scenario_OpenAngleCock()
	{
		if(activeScenario != null)
		{
			activeScenario.OpenAngleCocks();
		}
	}
	
	public void Scenario_CheckHalfwayStop()
	{
		if(activeScenario != null)
		{
			if(!didHalfWayStop)
				OnMissedHalfwayStop.Invoke();
		}
	}
	
	// Functions called by radio wheel
	public void Scenario_TryHalfwayStop()
	{
		if(activeScenario != null)
		{
			if(DistanceInCars < 1)
				didHalfWayStop = true;
		}
	}
	
	// Functions called by radio wheel
	public void Scenario_SetHalfwayStop()
	{
		didHalfWayStop = true;
	}
	
	public void Scenario_CallDistance()
	{
		if(activeScenario != null)
		{
			lastCallDistance = DistanceInFeet;
		}
	}
	
	public void Scenario_StretchSlack()
	{
		if(activeScenario != null)
		{
			activeScenario.StretchSlack();
		}
	}
}
