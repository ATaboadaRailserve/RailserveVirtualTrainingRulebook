using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour {
	
	public enum Axis { X, Y, Z };
	
	[System.Serializable]
	public struct CustomInteraction {
		public string interactionName;
		public InteractionHandler.InvokableState onTrigger;
	}
	
	// Reference to the all mighty handler
	public InteractionHandler interactionHandler;
	
	[Header("Display")]
	public string label;
	
	[Header("Basic Interactions")]
	public bool isInteractiable = true;
	public bool remainLocked;
	public InteractionHandler.InvokableState onCursorEnter;
	public InteractionHandler.InvokableState onCursorStay;
	public InteractionHandler.InvokableState onCursorExit;
	
	[Header("Trigger Interactions")]
	public InteractionHandler.InvokableState onTriggerEnter;
	public InteractionHandler.InvokableState onTriggerStay;
	public InteractionHandler.InvokableState onTriggerExit;
	public string[] tagWhiteList;
	public string[] tagBlackList;
	
	[Header("State Interactions")]
	public InteractionHandler.InvokableState onStart;
	public InteractionHandler.InvokableState onAwake;
	public InteractionHandler.InvokableState onEnable;
	public InteractionHandler.InvokableState onDisable;
	
	[Header("Custom Interactions")]
	public CustomInteraction[] customInteractions;
	
	[Header("Properties")]
	public bool lockToThisWhileInteracting;
	
	private float labelTimer;
	private bool prevInteractable;
	private bool initialized;
	
	private bool overrideInteraction;
	
	void Start () {
		onStart.Invoke();
	}
	
	void Awake () {
		StartCoroutine(Initialize());
		onAwake.Invoke();
	}
	
	void OnEnable () {
		if (!initialized)
			StartCoroutine(Initialize());
		onEnable.Invoke();
	}
	
	void OnDisable () {
		onDisable.Invoke();
	}
	
	IEnumerator Initialize () {
		yield return null;
		if (interactionHandler == null)
		{
			GameObject iHandlerGameObject = GameObject.FindWithTag("InteractionHandler");
			if(!iHandlerGameObject)
			{
				Debug.LogWarning("No interactionHandler found");
			}
			else
			{
				interactionHandler = iHandlerGameObject.GetComponent<InteractionHandler>();
			}
		}
		initialized = true;
	}
	
	void LateUpdate () {
		if (!initialized)
			return;
		if (interactionHandler && prevInteractable != isInteractiable && !isInteractiable && !remainLocked) {
			interactionHandler.LockInteraction = false;
		}
		prevInteractable = isInteractiable;
	}
	
	#region Cursor Callbacks
	
	public void IHCursorEnterPass () {	// Called when the mouse first enters this object
		if (overrideInteraction)
			return;
		IHCursorEnterEvents();
	}
	
	public void IHCursorStayPass () {	// Called when the mouse stays over this object
		if (overrideInteraction)
			return;
		if (!initialized)
			return;
		if (isInteractiable)
			interactionHandler.SetInteractionText(label);
		IHCursorStayEvents();
	}
	
	public void IHCursorExitPass () {	// Called when the mouse first leaves this object
		if (overrideInteraction)
			return;
		IHCursorExitEvents();
	}
	
	// Event Callbacks | Override this if you wish to employ conditional event blocking
	public virtual void IHCursorEnterEvents () {
		onCursorEnter.Invoke();
		IHCursorEnter();
	}
	public virtual void IHCursorStayEvents () {
		onCursorStay.Invoke();
		IHCursorStay();
	}
	public virtual void IHCursorExitEvents () {
		onCursorExit.Invoke();
		IHCursorExit();
	}
	
	// General Use Callback | Override this if you wish to employ extra logic after the events have been invoked
	public virtual void IHCursorEnter () {}
	public virtual void IHCursorStay () {}
	public virtual void IHCursorExit () {}
	
	#endregion
	
	
	#region Button (Axis) Callbacks
	
	public void IHButtonDownPass (InteractionHandler.InteractionParameters iParam) {
		if (overrideInteraction)
			return;
		if (!initialized)
			return;
		if (!isInteractiable) {
			return;
		}
		if (lockToThisWhileInteracting) {
			interactionHandler.LockInteraction = true;
		}
		IHButtonDown(iParam);
	}
	
	public void IHButtonEnterPass (InteractionHandler.InteractionParameters iParam) {
		if (overrideInteraction)
			return;
		if (!initialized)
			return;
		if (!isInteractiable) {
			return;
		}
		if (lockToThisWhileInteracting) {
			interactionHandler.LockInteraction = true;
		}
		IHButtonEnter(iParam);
	}
	
	public void IHButtonPass (InteractionHandler.InteractionParameters iParam) {
		if (overrideInteraction)
			return;
		if (!initialized)
			return;
		if (!isInteractiable) {
			if (prevInteractable != isInteractiable) {
				if (!remainLocked)
					interactionHandler.LockInteraction = false;
			}
			return;
		}
		IHButton(iParam);
	}
	
	public void IHButtonOriginalPass (InteractionHandler.InteractionParameters iParam) {
		if (overrideInteraction)
			return;
		if (!initialized)
			return;
		if (!isInteractiable) {
			if (prevInteractable != isInteractiable) {
				if (!remainLocked)
					interactionHandler.LockInteraction = false;
			}
			return;
		}
		IHButtonOriginal(iParam);
	}
	
	public void IHButtonExitPass (InteractionHandler.InteractionParameters iParam) {
		if (overrideInteraction)
			return;
		if (!initialized)
			return;
		if (!isInteractiable) {
			if (!remainLocked)
				interactionHandler.LockInteraction = false;
			return;
		}
		IHButtonExit(iParam);
	}
	
	public void IHButtonUpPass (InteractionHandler.InteractionParameters iParam) {
		if (overrideInteraction)
			return;
		if (!initialized)
			return;
		if (!isInteractiable) {
			return;
		}
		if (lockToThisWhileInteracting) {
			interactionHandler.LockInteraction = false;
		}
		IHButtonUp(iParam);
	}
	
	public void IHButtonUpOriginalPass (InteractionHandler.InteractionParameters iParam) {
		if (overrideInteraction)
			return;
		if (!initialized)
			return;
		if (!isInteractiable) {
			return;
		}
		if (lockToThisWhileInteracting) {
			interactionHandler.LockInteraction = false;
		}
		IHButtonUpOriginal(iParam);
	}
	
	// Called during axis activation events
	// InteractionHandler.InteractionParameters iParam
	// 		string axis; name of Rewired Axis
	// 		bool isFirst; whether or not this is the first object activated on this axis activation
	// isFirst is used for if you need to click on one object and drag over to another, leaving the first.
	public virtual void IHButtonDown (InteractionHandler.InteractionParameters iParam) {}			// Called on the axis initial press/activation
	public virtual void IHButtonEnter (InteractionHandler.InteractionParameters iParam) {}			// Called when holding an axis while entering this object
	public virtual void IHButton (InteractionHandler.InteractionParameters iParam) {}				// Called every frame the axis is held
	public virtual void IHButtonOriginal (InteractionHandler.InteractionParameters iParam) {}		// Called every frame the axis is held to the original object
	public virtual void IHButtonExit (InteractionHandler.InteractionParameters iParam) {}			// Called when holding an axis while exiting this object
	public virtual void IHButtonUp (InteractionHandler.InteractionParameters iParam) {}				// Called when the axis is released
	public virtual void IHButtonUpOriginal (InteractionHandler.InteractionParameters iParam) {}		// Called when the axis is released to the original object
	
	#endregion
	
	
	#region Trigger Volume Callbacks
	
	void OnTriggerEnter (Collider col) {
		if (tagWhiteList.Length != 0) {
			foreach (string t in tagWhiteList) {
				if (col.gameObject.tag == t) {
					IHOnTriggerEnterPass(col);
				}
			}
		} else {
			foreach (string t in tagBlackList) {
				if (col.gameObject.tag == t)
					return;
			}
			IHOnTriggerEnterPass(col);
		}
	}
	void OnTriggerStay (Collider col) {
		if (tagWhiteList.Length != 0) {
			foreach (string t in tagWhiteList) {
				if (col.gameObject.tag == t) {
					IHOnTriggerStayPass(col);
				}
			}
		} else {
			foreach (string t in tagBlackList) {
				if (col.gameObject.tag == t)
					return;
			}
			IHOnTriggerStayPass(col);
		}
	}
	void OnTriggerExit (Collider col) {
		if (tagWhiteList.Length != 0) {
			foreach (string t in tagWhiteList) {
				if (col.gameObject.tag == t) {
					IHOnTriggerExitPass(col);
				}
			}
		} else {
			foreach (string t in tagBlackList) {
				if (col.gameObject.tag == t)
					return;
			}
			IHOnTriggerExitPass(col);
		}
	}
	
	// This step is for any future blanket use logic that will feed down to all Interactable type scripts
	void IHOnTriggerEnterPass (Collider col) {
		IHOnTriggerEnterEvents(col);
	}
	
	void IHOnTriggerStayPass (Collider col) {
		IHOnTriggerStayEvents(col);
	}
	
	void IHOnTriggerExitPass (Collider col) {
		IHOnTriggerExitEvents(col);
	}
	
	// Collider Overload
	// Event Callback | Override this if you wish to employ conditional event blocking
	public virtual void IHOnTriggerEnterEvents (Collider col) {
		onTriggerEnter.Invoke();
		IHOnTriggerEnter(col);
		IHOnTriggerEnter();
	}
	public virtual void IHOnTriggerStayEvents (Collider col) {
		onTriggerStay.Invoke();
		IHOnTriggerStay(col);
		IHOnTriggerStay();
	}
	public virtual void IHOnTriggerExitEvents (Collider col) {
		onTriggerExit.Invoke();
		IHOnTriggerExit(col);
		IHOnTriggerExit();
	}
	
	// General Use Callback | Override this if you wish to employ extra logic after the events have been invoked
	public virtual void IHOnTriggerEnter (Collider col) {}
	public virtual void IHOnTriggerStay (Collider col) {}
	public virtual void IHOnTriggerExit (Collider col) {}
	public virtual void IHOnTriggerEnter () {}
	public virtual void IHOnTriggerStay () {}
	public virtual void IHOnTriggerExit () {}
	
	#endregion
	
	public void CallCustomInteraction (string name) {
		foreach (CustomInteraction c in customInteractions) {
			if (c.interactionName == name) {
				c.onTrigger.Invoke();
			}
		}
	}
	
	#region Transform Functions
	
	public void SetXAngle (float angle) {
		transform.localEulerAngles = new Vector3(angle, transform.localEulerAngles.y, transform.localEulerAngles.z);
	}
	
	public void SetYAngle (float angle) {
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, angle, transform.localEulerAngles.z);
	}
	
	public void SetZAngle (float angle) {
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, angle);
	}
	
	public void SetXPosition (float pos) {
		transform.localPosition = new Vector3(pos, transform.localPosition.y, transform.localPosition.z);
	}
	
	public void SetYPosition (float pos) {
		transform.localPosition = new Vector3(transform.localPosition.x, pos, transform.localPosition.z);
	}
	
	public void SetZPosition (float pos) {
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, pos);
	}
	
	#endregion
	
	#region GetterSetters
	
	public bool OverrideInteraction {
		get { return overrideInteraction; }
		set { overrideInteraction = value; }
	}
	
	#endregion
}