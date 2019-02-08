using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicInteractable : Interactable {
	
	public bool mustBeInitialObject;
	public string activationAxis;
	
	[Header("Axis Interactions")]
	public InteractionHandler.InvokableState onButtonDown;
	public InteractionHandler.InvokableState onButtonEnter;
	public InteractionHandler.InvokableState onButton;
	public InteractionHandler.InvokableState onButtonExit;
	public InteractionHandler.InvokableState onButtonUp;
	
	public override void IHButtonDown (InteractionHandler.InteractionParameters iParam) {
		if (iParam.axis == activationAxis && (!mustBeInitialObject || iParam.isFirst == gameObject)) {
			if (!remainLocked)
				interactionHandler.LockInteraction = false;
			onButtonDown.Invoke();
		}
	}
	
	public override void IHButtonEnter (InteractionHandler.InteractionParameters iParam) {
		if (iParam.axis == activationAxis && (!mustBeInitialObject || iParam.isFirst == gameObject)) {
			if (!remainLocked)
				interactionHandler.LockInteraction = false;
			onButtonEnter.Invoke();
		}
	}
	
	public override void IHButton (InteractionHandler.InteractionParameters iParam) {
		if (iParam.axis == activationAxis && (!mustBeInitialObject || iParam.isFirst == gameObject)) {
			if (!remainLocked)
				interactionHandler.LockInteraction = false;
			onButton.Invoke();
		}
	}
	
	public override void IHButtonExit (InteractionHandler.InteractionParameters iParam) {
		if (iParam.axis == activationAxis && (!mustBeInitialObject || iParam.isFirst == gameObject)) {
			if (!remainLocked)
				interactionHandler.LockInteraction = false;
			onButtonExit.Invoke();
		}
	}
	
	public override void IHButtonUp (InteractionHandler.InteractionParameters iParam) {
		if (iParam.axis == activationAxis && (!mustBeInitialObject || iParam.isFirst == gameObject)) {
			if (!remainLocked)
				interactionHandler.LockInteraction = false;
			onButtonUp.Invoke();
		}
	}
	
	public bool IsInteractiable {
		get { return isInteractiable; }
		set { isInteractiable = value; }
	}
}