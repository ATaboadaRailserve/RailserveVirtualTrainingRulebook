using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UberCouplingCar : MonoBehaviour {
	
	/*
		This script is specifically to simplify the enforcement
		of the step-by-set operations required during coupling.
		
		This script removes the need for logic gate scripts, and
		hopefully generalizes some of the coupling functionality.
		
		The following steps occur during a couple:
		
		1) Apprach the locomotive within 0.5 - 0.999 car lengths
		2) Open any operating levers
		3) Open any knuckles
		4) Engage the knuckle/couple (back the locomotive into it)
		5) Stretch slack
		6) Lace the air
		7) Open the angle cocks
		
		In the terminology below "Front" refers to the side of a
		standalone car that a locomotive is approaching
		
		"Rear" refers to the side that will remain open/uncoupled,
		after the front knuckle has been coupled to the locomotive line
		
		"Rear" usually only matters after a car has already been coupled
		at the front.
	*/
	
	// Inspector Properties
	[Header("Debug")]
	public bool debugMode;
	
	[Header("Basic Properties")]
	public Transform frontCouplePoint;
	public Transform rearCouplePoint;
	
	[Header("Additional References")]
	public GameObject AirHoseInteractable;
	
	[Header("Initial State")]
	public bool coupledAtFront;
	public bool coupledAtRear;
	
	[Header("Inspector Only")]
	public bool insFrontCoupleLeverOpened;
	public bool insRearCoupleLeverOpened;
	public bool insFrontCoupleOpened;
	public bool insRearCoupleOpened;
	public bool insFrontAirHoseAttached;
	public bool insRearAirHoseAttached;
	public bool insFrontKnuckleEngaged;
	public bool insRearKnuckleEngaged;
	public bool insFrontSlackStretched;
	public bool insRearSlackStretched;
	public bool insFrontAngleCockOpened;
	public bool insRearAngleCockOpened;
	
	
	[Header("Visual & Audio Events")]
	public UnityEvent OnFrontLeverOpened;
	public UnityEvent OnFrontLeverClosed;
	public UnityEvent OnRearLeverOpened;
	public UnityEvent OnRearLeverClosed;
	public UnityEvent OnFrontKnuckleOpened;
	public UnityEvent OnFrontKnuckleClosed;
	public UnityEvent OnRearKnuckleOpened;
	public UnityEvent OnRearKnuckleClosed;
	public UnityEvent OnFrontAirHoseAttached;
	public UnityEvent OnRearAirHoseAttached;
	public UnityEvent OnFrontKnuckleEngaged;
	public UnityEvent OnRearKnuckleEngaged;
	public UnityEvent OnFrontSlackStretched;
	public UnityEvent OnRearSlackStretched;
	public UnityEvent OnFrontAngleCockOpened;
	public UnityEvent OnFrontAngleCockClosed;
	public UnityEvent OnRearAngleCockOpened;
	public UnityEvent OnRearAngleCockClosed;
	
	// UberCoupling Properties
	public bool FrontCoupleLeverOpened {get; private set;}
	public bool RearCoupleLeverOpened {get; private set;}
	public bool FrontCoupleOpened {get; private set;}
	public bool RearCoupleOpened {get; private set;}
	public bool FrontAirHoseAttached {get; private set;}
	public bool RearAirHoseAttached {get; private set;}
	
	// Additional Car Properties
	public bool FrontKnuckleEngaged {get; private set;}
	public bool RearKnuckleEngaged {get; private set;}
	public bool FrontSlackStretched {get; private set;}
	public bool RearSlackStretched {get; private set;}
	public bool FrontAngleCockOpened {get; private set;}
	public bool RearAngleCockOpened {get; private set;}
	
	// Interaction
	public bool isFrontInteractionEnabled {get; private set;}
	public bool isRearInteractionEnabled {get; private set;}
	
	// Script References
	private UberCoupling uberCoupling;
	private UberCouplingModule uberCouplingModule;
	
	// Hidden Properties
	private bool isActiveInScenario;
	private bool isFrontCar;
	private bool isRearCar;
	
	
	// Helper Functions
	public void Initialize(UberCoupling uc, UberCouplingModule ucm)
	{
		uberCoupling = uc;
		uberCouplingModule = ucm;
		
		isFrontInteractionEnabled = true;
		isRearInteractionEnabled = true;
		
		if(coupledAtFront)
		{
			OnFrontLeverOpened.Invoke();
			OnFrontKnuckleOpened.Invoke();
			OnFrontAirHoseAttached.Invoke();
			OnFrontKnuckleEngaged.Invoke();
			OnFrontSlackStretched.Invoke();
			OnFrontAngleCockOpened.Invoke();
			DisableFrontInteraction();
		}
		if(coupledAtRear)
		{
			OnRearLeverOpened.Invoke();
			OnRearKnuckleOpened.Invoke();
			OnRearAirHoseAttached.Invoke();
			OnRearKnuckleEngaged.Invoke();
			OnRearSlackStretched.Invoke();
			OnRearAngleCockOpened.Invoke();
			DisableRearInteraction();
		}
	}
	
	public void ResetScenarioValues()
	{
		isFrontInteractionEnabled = true;
		isRearInteractionEnabled = true;
		
		coupledAtFront = false;
		coupledAtRear = false;
		
		FrontCoupleLeverOpened = false;
		RearCoupleLeverOpened = false;
		FrontCoupleOpened = false;
		RearCoupleOpened = false;
		FrontAirHoseAttached = false;
		RearAirHoseAttached = false;
		FrontKnuckleEngaged = false;
		RearKnuckleEngaged = false;
		FrontSlackStretched = false;
		RearSlackStretched = false;
		FrontAngleCockOpened = false;
		RearAngleCockOpened = false;
		
		insFrontCoupleLeverOpened = false;
		insRearCoupleLeverOpened = false;
		insFrontCoupleOpened = false;
		insRearCoupleOpened = false;
		insFrontAirHoseAttached = false;
		insRearAirHoseAttached = false;
		insFrontKnuckleEngaged = false;
		insRearKnuckleEngaged = false;
		insFrontSlackStretched = false;
		insRearSlackStretched = false;
		insFrontAngleCockOpened = false;
		insRearAngleCockOpened = false;
	}
	
	public void StartScenario(bool isFrontCar)
	{
		this.isActiveInScenario = true;
		this.isFrontCar = isFrontCar;
		this.isRearCar = !isFrontCar;
		
	}
	
	public void EndScenario()
	{
		this.isActiveInScenario = false;
		this.isFrontCar = false;
		this.isRearCar = false;
	}
	
	public void Fail(string message)
	{
		if(uberCouplingModule != null)
		{
			uberCouplingModule.PushServerMessage(message);
			uberCouplingModule.FailModule();
		}
		else
		{
			Debug.LogError("Missing UberCoupling reference");
		}
	}
	
	public void DisableFrontInteraction()
	{
		AirHoseInteractable.SetActive(false);
		isFrontInteractionEnabled = false;
	}
	
	public void DisableRearInteraction()
	{
		isRearInteractionEnabled = false;
	}
	
	// Forwards
	public void ForwardCoupleEngaged()
	{
		if(!isActiveInScenario) // prevent random cars from progressing scenario
			return;
		if(uberCoupling != null)
		{
			uberCoupling.Scenario_EngageKnuckle();
		}
		else
		{
			Debug.LogError("Missing UberCoupling reference");
		}
	}
	
	public void ForwardAirHosesAttached()
	{
		if(!isActiveInScenario) // prevent random cars from progressing scenario
			return;
		if(uberCoupling != null)
		{
			uberCoupling.Scenario_AttachAirHoses();
		}
		else
		{
			Debug.LogError("Missing UberCoupling reference");
		}
	}
	
	public void ForwardFrontAngleCockOpened()
	{
		// Rear car has angle cock at FRONT
		if(!isRearCar) // prevent random cars from progressing scenario
			return;
		if(uberCoupling != null)
		{
			uberCoupling.Scenario_OpenAngleCock();
		}
		else
		{
			Debug.LogError("Missing UberCoupling reference");
		}
	}
	
	public void ForwardRearAngleCockOpened()
	{
		// Front car has angle cock at REAR
		if(!isFrontCar)
			return;
		if(uberCoupling != null)
		{
			uberCoupling.Scenario_OpenAngleCock();
		}
		else
		{
			Debug.LogError("Missing UberCoupling reference");
		}
	}
	
	public void ForwardCheckHalfwayStop()
	{
		if(!isRearCar)
		{
			Fail("Approaching wrong car!!!");
		}
		else
		{
			uberCoupling.Scenario_CheckHalfwayStop();
		}
	}
	
	// Try Functions (Direct Player Interaction)
	// Levers
	public void TryToggleFrontLever()
	{
		if(!isFrontInteractionEnabled || (FrontKnuckleEngaged && !debugMode))
			return;
		else if(FrontCoupleLeverOpened) {
			//TryCloseFrontLever();
		} else
			TryOpenFrontLever();
	}
	
	private void TryOpenFrontLever()
	{
		FrontCoupleLeverOpened = true;
		insFrontCoupleLeverOpened = true;
		OnFrontLeverOpened.Invoke();
	}
	
	private void TryCloseFrontLever()
	{
		if(FrontCoupleOpened && !debugMode) // Fail condition
			Fail("Cannot close knuckle lever with knuckle opened");
		else
		{
			FrontCoupleLeverOpened = false;
			insFrontCoupleLeverOpened = false;
			OnFrontLeverClosed.Invoke();
		}
	}
	
	public void TryToggleRearLever()
	{
		if(!isRearInteractionEnabled || (RearKnuckleEngaged && !debugMode))
			return;
		else if(RearCoupleLeverOpened) {
			//TryCloseRearLever();
		} else
			TryOpenRearLever();
	}
	
	private void TryOpenRearLever()
	{
		RearCoupleLeverOpened = true;
		insRearCoupleLeverOpened = true;
		OnRearLeverOpened.Invoke();
	}
	
	private void TryCloseRearLever()
	{
		if(RearCoupleOpened && !debugMode) // Fail condition
			Fail("Cannot close knuckle lever with knuckle opened");
		else
		{
			RearCoupleLeverOpened = false;
			insRearCoupleLeverOpened = false;
			OnRearLeverClosed.Invoke();
		}
	}
	
	// Knuckles
	public void TryToggleFrontCouple()
	{
		if(!isFrontInteractionEnabled || (FrontKnuckleEngaged && !debugMode))
		{
			return;
		}
		else if (FrontCoupleOpened)
		{
			//TryCloseFrontCouple();
		}
		else{
			TryOpenFrontCouple();
		}
	}
	
	private void TryOpenFrontCouple()
	{
		Debug.Log("Trying Open Couple");
		if(!FrontCoupleLeverOpened && !debugMode) // Fail condition
		{
			Fail("Cannot open knuckle with knuckle lever closed");
		}
		else
		{
			Debug.Log("Doing Open Couple");
			FrontCoupleOpened = true;
			insFrontCoupleOpened = true;
			OnFrontKnuckleOpened.Invoke();
		}
	}
	
	private void TryCloseFrontCouple()
	{
		print("5");
		Debug.Log("Doing Close Couple");
		FrontCoupleOpened = false;
		insFrontCoupleOpened = false;
		OnFrontKnuckleClosed.Invoke();
	}
	
	public void TryToggleRearCouple()
	{
		if(!isRearInteractionEnabled || (RearKnuckleEngaged && !debugMode))
			return;
		else if(RearCoupleOpened) {
			//TryCloseRearCouple();
		} else
			TryOpenRearCouple();
	}
	
	private void TryOpenRearCouple()
	{
		if(!RearCoupleLeverOpened && !debugMode) // Fail condition
			Fail("Cannot open knuckle with knuckle lever closed");
		else
		{
			RearCoupleOpened = true;
			insRearCoupleOpened = true;
			OnRearKnuckleOpened.Invoke();
		}
	}
	
	private void TryCloseRearCouple()
	{
		RearCoupleOpened = false;
		insRearCoupleOpened = false;
		OnRearKnuckleClosed.Invoke();
	}
	
	// Air Hoses
	public void TryAttachFrontAirHose()
	{
		if(!isFrontInteractionEnabled || FrontAirHoseAttached)
			return;
		if(!FrontSlackStretched && !debugMode) // Fail condition
			Fail("Cannot attach air hoses before stretching slack");
		else
		{
			FrontAirHoseAttached = true;
			insFrontAirHoseAttached = true;
			OnFrontAirHoseAttached.Invoke();
		}
	}
	
	public void TryAttachRearAirHose()
	{
		if(!isRearInteractionEnabled || RearAirHoseAttached)
			return;
		if(!RearSlackStretched && !debugMode) // Fail condition
			Fail("Cannot attach air hoses before stretching slack");
		else
		{
			RearAirHoseAttached = true;
			insRearAirHoseAttached = true;
			OnRearAirHoseAttached.Invoke();
		}
	}
	
	// Angle Cock
	public void TryOpenFrontAngleCock()
	{
		if(!isFrontInteractionEnabled || FrontAngleCockOpened)
			return;
		if(!FrontAirHoseAttached && !debugMode) // Fail condition
			Fail("Cannot open angle cock before air hoses are attached");
		else
		{
			FrontAngleCockOpened = true;
			insFrontAngleCockOpened = true;
			OnFrontAngleCockOpened.Invoke();
		}
	}
	
	public void TryOpenRearAngleCock()
	{
		if(!isRearInteractionEnabled || RearAngleCockOpened)
			return;
		if(!RearAirHoseAttached && !debugMode) // Fail condition
			Fail("Cannot open angle cock before air hoses are attached");
		else
		{
			RearAngleCockOpened = true;
			insRearAngleCockOpened = true;
			OnRearAngleCockOpened.Invoke();
		}
	}
	
	// Try Functions (Messaged / Explicit Call / Indirect Player Interaction)
	public void TryEngageFrontCouple()
	{
		if(!isFrontInteractionEnabled || FrontKnuckleEngaged)
			return;
		if(!FrontCoupleOpened && !debugMode)
			Fail("Cannot couple with knuckle closed");
		else
		{
			FrontKnuckleEngaged = true;
			insFrontKnuckleEngaged = true;
			OnFrontKnuckleEngaged.Invoke();
		}
	}
	
	public void TryEngageRearCouple()
	{
		if(!isRearInteractionEnabled || RearKnuckleEngaged)
			return;
		if(!RearCoupleOpened && !debugMode)
			Fail("Cannot couple with knuckle closed");
		else
		{
			RearKnuckleEngaged = true;
			insRearKnuckleEngaged = true;
			OnRearKnuckleEngaged.Invoke();
		}
	}
	
	public void TryStretchFrontSlack()
	{
		if(!isFrontInteractionEnabled || FrontSlackStretched)
			return;
		if(!FrontKnuckleEngaged && !debugMode)
			Fail("Cannot stretch slack before the knuckle has been engaged");
		else
		{
			FrontSlackStretched = true;
			insFrontSlackStretched = true;
			AirHoseInteractable.SetActive(true);
			OnFrontSlackStretched.Invoke();
		}
	}
	
	public void TryStretchRearSlack()
	{
		if(!isRearInteractionEnabled || RearSlackStretched)
			return;
		if(!RearKnuckleEngaged && !debugMode)
			Fail("Cannot stretch slack before the knuckle has been engaged");
		else
		{
			RearSlackStretched = true;
			insRearSlackStretched = true;
			OnRearSlackStretched.Invoke();
		}
	}
}
