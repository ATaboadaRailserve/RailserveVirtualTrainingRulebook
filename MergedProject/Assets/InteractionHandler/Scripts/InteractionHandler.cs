using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Rewired;

/*

	This script is to be the over-arching handler for all interaction methods and character controllers
	It is to handle which character controller is active/used and how it interacts with the interaction
	methods defined in the other parts of this module (Other classes and prefabs).
	It is to have a universal interaction function with callbacks as well as custom Invoke-able variables
	
	Object Interaction:
	IHCursorEnterPass
	IHCursorStayPass
	IHCursorExitPass
	
	Rewired Axis Interaction:
	IHButtonDownPass
	IHButtonEnterPass
	IHButtonPass
	IHButtonExitPass
	IHButtonUpPass
	
*/


public class InteractionHandler : MonoBehaviour {
	
	// Internalized global struct for parameters passed from the InteractionHandler to the interacted objects
	public struct InteractionParameters {
		public string axis;
		public bool isFirst;
		public float distance;
		public Vector2 deltaLook;
		public Vector3 playerHeading;
	}
	
	[System.Serializable]
	public struct Interaction { // Hold UnityEvents for click interaction
		[Header("Cursor aim events")]
		public InvokableState onCursorEnter;
		public InvokableState onCursorStay;
		public InvokableState onCursorExit;
		public string verticalAxis;
		public string horizontalAxis;
		
		[Header("Rewired Axis events")]
		public InteractionMethod[] method;
	}
	
	[System.Serializable]
	public struct GameStates { // Hold UnityEvents for each default game state
		public InvokableState onWin;
		public InvokableState onLose;
		public InvokableState onReturnToMenu;
		public InvokableState onRestart;
		public CustomState[] customStates;
	}
	
	[System.Serializable]
	public struct CustomState {
		public string stateName;
		public InvokableState onState;
	}
	
	[System.Serializable]
	public class InvokableState { // Consolodated class holding the intitial and late events to call
		public UnityEvent initialActivation; // ALWAYS Invoke this BEFORE Invoking lateActivation.  It's similar to Update and LateUpdate
		public UnityEvent lateActivation;
		
		// Call this method if you wish to invoke an invokable state
		// rather than each activation phase individually
		public void Invoke () {
			initialActivation.Invoke();
			lateActivation.Invoke();
		}

        public void AddInitialListener(UnityAction call)
        {
            initialActivation.AddListener(call);
        }
		
		public void RemoveInitialListener(UnityAction call)
		{
			initialActivation.RemoveListener(call);
		}

        public void AddLateListener(UnityAction call)
        {
            lateActivation.AddListener(call);
        }
		
		public void RemoveLateListener(UnityAction call)
		{
			lateActivation.RemoveListener(call);
		}
	}
	
	[System.Serializable]
	public struct InteractionMethod {
		public string axis; // The Rewired Action name
		[HideInInspector]
		public bool isDown;
		public InvokableState onDown;
		public InvokableState onEnter;
		public InvokableState onStay;
		public InvokableState onExit;
		public InvokableState onUp;
	}
	
	// Interactions that are not directly player input related
	// This can be collisions, triggers, transform or game object states, etc.
	// Theses are called by other objects as interactionHandler.InvokeAction("nameOfAction")
	[System.Serializable]
	public struct CustomInteraction {
		public string name;
		public InvokableState action;
	}
	
	[Header("Setup")]
	public Camera interactionCamera;
	public LayerMask interactableLayers = ~0;
	public Text interactionText;
	public Interaction interactions;
	public GameStates gameStates;
	
	[Header("State Invokes")]
	public InvokableState onStart;
	public InvokableState onAwake;
	
	[Header("Callbacks from custom scripts")]
	public CustomInteraction[] customInteractions;
	
	[Header("Player Controller Controls")]
	public PlayerController playerController;
	public bool playerCanWalk = true;
	public bool playerCanLook = true;
	public bool playerCanCursorOver = true;
	public bool playerCanInteract = true;
	
	[Header("Debug")]
	public bool printHit;
	public bool printClick;
	
	private bool lockInteraction;
	
	private Ray ray;
	private RaycastHit cursorHit;
	private GameObject cursordObject;
	private GameObject initialInteractedObject;
	private InteractionParameters iParam;
	
	private int cursorLocks;
	private int interactLocks;
	private int moveLocks;
	private int lookLocks;
	private int solidInteractLocks;
	private int notKinematicLocks;
	
	#region Rewired Stuff
	private Player player;
	void Awake() {
		gameObject.tag = "InteractionHandler";
		player = ReInput.players.GetPlayer(0); // get the Rewired Player
		
		if (interactionText == null && GameObject.FindWithTag("InteractionText")) {
			interactionText = GameObject.FindWithTag("InteractionText").GetComponent<Text>();
		}
		onAwake.Invoke();
	}
	#endregion
	
	void Start () {
		onStart.Invoke();
	}
	
	void Update () {
		iParam.deltaLook = new Vector2(player.GetAxis(interactions.horizontalAxis), player.GetAxis(interactions.verticalAxis));
		
		if (playerCanCursorOver)
			CursorOver();
		if (playerCanInteract)
			CheckAxes();
		if (playerCanWalk != playerController.CanMove)
			playerController.CanMove = playerCanWalk;
		if (playerCanLook != playerController.CanLook)
			playerController.CanLook = playerCanLook;
	}
	
	/*
		Callbacks for the object that the cursor is aimed at
		
		IHCursorEnter - Called on the first frame that the cursor is aimed at an object
		IHCursorStay - Called on EVERY frame that the cursor is aimed at an object
		IHCursorExit - Called on the frame that the cursor is no longer aimed at the object (Calling IHCursorEnter on the new object)
		IHButtonExit - Called when the cursor leaves an object while an axis is held down
		IHButtonEnter - Called when the cursor enters an object while an axis is held down
	*/
	void CursorOver () {
		SetInteractionText();
		// Get what the cursor is currently aimed at
		ray = interactionCamera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out cursorHit, 1000, interactableLayers)) {
			if (printHit)
				print(cursorHit.collider.gameObject.name);
			iParam.distance = cursorHit.distance;
			iParam.playerHeading = playerController.transform.localEulerAngles;
			// If it's aimed at something new and the player is not locked to an interaction
			if (!lockInteraction) {
				if (cursordObject && cursordObject != cursorHit.collider.gameObject) {
					// Send the cursor exit callback to the old object
					cursordObject.SendMessage("IHCursorExitPass", SendMessageOptions.DontRequireReceiver);
					interactions.onCursorExit.Invoke();
					
					// If a button/axis is active while the cursor is leaving an object
					// Call the button exit callback on the object being left
					foreach (InteractionMethod i in interactions.method) {
						if (player.GetButton(i.axis)) {
							iParam.axis = i.axis;
							iParam.isFirst = (cursordObject == initialInteractedObject);
							cursordObject.SendMessage("IHButtonExitPass", iParam, SendMessageOptions.DontRequireReceiver);
							i.onExit.Invoke();
						}
					}
					
					// Set the new object then send the cursor enter callback to it
					cursordObject = cursorHit.collider.gameObject;
					cursordObject.SendMessage("IHCursorEnterPass", SendMessageOptions.DontRequireReceiver);
					interactions.onCursorEnter.Invoke();
					
					// If a button/axis is active while the cursor is entering an object
					// Call the button exit callback on the object being entered
					foreach (InteractionMethod i in interactions.method) {
						if (player.GetButton(i.axis)) {
							iParam.axis = i.axis;
							iParam.isFirst = (cursordObject == initialInteractedObject);
							cursordObject.SendMessage("IHButtonEnterPass", iParam, SendMessageOptions.DontRequireReceiver);
							i.onEnter.Invoke();
						}
					}
				} else {
					// Set the new object then send the cursor enter callback to it
					cursordObject = cursorHit.collider.gameObject;
					cursordObject.SendMessage("IHCursorEnterPass", SendMessageOptions.DontRequireReceiver);
					interactions.onCursorEnter.Invoke();
				}
			}
			// Each frame that the cursor is aimed at something, send the cursor stay callback
			cursordObject.SendMessage("IHCursorStayPass", SendMessageOptions.DontRequireReceiver);
			interactions.onCursorStay.Invoke();
		} else if (!lockInteraction && cursordObject) {
			// If it's aimed at nothing and was previously aimed at something
			// Send the cursor exit callback to the previous something then set the current target to null
			cursordObject.SendMessage("IHCursorExitPass", SendMessageOptions.DontRequireReceiver);
			interactions.onCursorExit.Invoke();
			
			// If a button/axis is active while the cursor is leaving an object
			// Call the button exit callback on the object being left
			foreach (InteractionMethod i in interactions.method) {
				if (player.GetButton(i.axis)) {
					iParam.axis = i.axis;
					iParam.isFirst = (cursordObject == initialInteractedObject);
					cursordObject.SendMessage("IHButtonExitPass", iParam, SendMessageOptions.DontRequireReceiver);
					i.onExit.Invoke();
				}
			}
			cursordObject = null;
		}
	}
	
	/*
		Callbacks for the object that the cursor is aimed at
		
		IHButtonDown - Called on the first frame that the button is pressed
		IHButton - Called on EVERY frame that the button is held
		IHButtonUp - Called on the frame that the button is released
	*/
	void CheckAxes () {
		for (int i = 0; i < interactions.method.Length; i++) {
			if (cursordObject) {
				iParam.axis = interactions.method[i].axis;
				if (!interactions.method[i].isDown && player.GetButtonDown(interactions.method[i].axis)) {
					interactions.method[i].isDown = true;
					iParam.isFirst = true;
					cursordObject.SendMessage("IHButtonDownPass", iParam, SendMessageOptions.DontRequireReceiver);
					interactions.method[i].onDown.Invoke();
					initialInteractedObject = cursordObject;
				}
				if (player.GetButton(interactions.method[i].axis)) {
					iParam.isFirst = (cursordObject == initialInteractedObject);
					cursordObject.SendMessage("IHButtonPass", iParam, SendMessageOptions.DontRequireReceiver);
					if (initialInteractedObject)
						initialInteractedObject.SendMessage("IHButtonOriginalPass", iParam, SendMessageOptions.DontRequireReceiver);
					interactions.method[i].onStay.Invoke();
				}
				if (interactions.method[i].isDown && player.GetButtonUp(interactions.method[i].axis)) {
					iParam.isFirst = (cursordObject == initialInteractedObject);
					cursordObject.SendMessage("IHButtonUpPass", iParam, SendMessageOptions.DontRequireReceiver);
					if (initialInteractedObject)
						initialInteractedObject.SendMessage("IHButtonUpOriginalPass", iParam, SendMessageOptions.DontRequireReceiver);
					interactions.method[i].onUp.Invoke();
					initialInteractedObject = null;
					interactions.method[i].isDown = false;
				}
			} else if (interactions.method[i].isDown && !player.GetButton(interactions.method[i].axis)) {
				interactions.method[i].isDown = false;
			}
		}
	}
	
	public void Win () {
		gameStates.onWin.Invoke();
		GameObject.FindWithTag("MessageHolder").GetComponent<DatabaseMessageHolder>().CompleteModule();
	}
	
	public void Lose () {
		gameStates.onLose.Invoke();
		GameObject.FindWithTag("MessageHolder").GetComponent<DatabaseMessageHolder>().FailedModule();
	}
	
	public void ReturnToMenu () {
		gameStates.onReturnToMenu.Invoke();
	}
	
	public void Restart () {
		gameStates.onRestart.Invoke();
	}
	
	public void InvokeAction (string actionName) {
		foreach (CustomInteraction i in customInteractions) {
			if (i.name == actionName)
				i.action.Invoke();
		}
	}
	
	public void ForcePlayerCanWalk()
	{
		moveLocks = 0;
		playerCanWalk = true;
	}
	
	public void ForcePlayerCanLook()
	{
		lookLocks = 0;
		playerCanLook = true;
	}
	
	public void ForcePlayerCanCursor()
	{
		cursorLocks = 0;
		playerCanCursorOver = true;
	}
	
	public void ForcePlayerCanInteract()
	{
		interactLocks = 0;
		playerCanInteract = true;
	}
	
	public bool PlayerCanWalk {
		get { return playerCanWalk; }
		set {
			
			/* Gets the method of the script that calls to set
			// Still not quite what's needed as it needs the specific instance of the script that calls it
			// So I would need to find the memory id of the caller object to store that this object has locked (or remove if unlocked) in the list of lock calls
			StackTrace stackTrace = new StackTrace();           // get call stack
			StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)
			print(stackFrames[1].GetMethod().Name);
			*/
			
			if (!value) {
				moveLocks++;
				playerCanWalk = false;
			} else {
				moveLocks--;
				if (moveLocks <= 0) {
					moveLocks = 0;
					playerCanWalk = true;
				}
			}
		}
	}
	
	public bool PlayerCanLook {
		get { return playerCanLook; }
		set {
			if (!value) {
				lookLocks++;
				playerCanLook = false;
			} else {
				lookLocks--;
				if (lookLocks <= 0) {
					lookLocks = 0;
					playerCanLook = true;
				}
			}
		}
	}
	
	public bool PlayerCanCursorOver {
		get { return playerCanCursorOver; }
		set {
			if (!value) {
				cursorLocks++;
				playerCanCursorOver = false;
			} else {
				cursorLocks--;
				if (cursorLocks <= 0) {
					cursorLocks = 0;
					playerCanCursorOver = true;
				}
			}
		}
	}
	
	public bool PlayerCanInteract {
		get { return playerCanInteract; }
		set {
			if (!value) {
				interactLocks++;
				playerCanInteract = false;
			} else {
				interactLocks--;
				if (interactLocks <= 0) {
					interactLocks = 0;
					playerCanInteract = true;
				}
			}
		}
	}
	
	public bool LockInteraction {
		get { return lockInteraction; }
		set {
			if (value) {
				solidInteractLocks++;
				PlayerCanWalk = false;
				PlayerCanLook = false;
				PlayerCanCursorOver = false;
				lockInteraction = true;
			} else {
				solidInteractLocks--;
				if (solidInteractLocks <= 0) {
					solidInteractLocks = 0;
					PlayerCanWalk = true;
					PlayerCanLook = true;
					PlayerCanCursorOver = true;
					lockInteraction = false;
				}
			}
		}
	}
	
	public bool PlayerIsKinematic {
		get { return playerController.GetRigidbody().isKinematic; }
		set {
			if (!value) {
				notKinematicLocks++;
				playerController.GetRigidbody().isKinematic = false;
			} else {
				notKinematicLocks--;
				if (notKinematicLocks <= 0) {
					notKinematicLocks = 0;
					playerController.GetRigidbody().isKinematic = true;
				}
			}
		}
	}
	
	public int CursorLock {
		get {
			switch(Cursor.lockState) {
				case CursorLockMode.None:
					return 0;
					break;
				case CursorLockMode.Locked:
					return 1;
					break;
				case CursorLockMode.Confined:
					return 2;
					break;
			}
			return 3;
		}
		set {
			switch(value) {
				case 0:
					Cursor.lockState = CursorLockMode.None;
					break;
				case 1:
					Cursor.lockState = CursorLockMode.Locked;
					break;
				case 2:
					Cursor.lockState = CursorLockMode.Confined;
					break;
			}
		}
	}
	
	public void SetInteractionText (string text = "") {
		if (interactionText == null)
			return;
		interactionText.text = text;
		if (text != "")
			interactionText.transform.parent.gameObject.SetActive(true);
		else
			interactionText.transform.parent.gameObject.SetActive(false);
	}
	
	public void CustomGameState (string state) {
		foreach (CustomState s in gameStates.customStates) {
			if (s.stateName == state) {
				s.onState.Invoke();
			}
		}
	}
}