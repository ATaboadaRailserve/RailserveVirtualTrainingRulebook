using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : BaseLogic {
	
	public enum GateType { AND, OR }
	
	[Header("Gate Parameters")]
	public GateType gateType;
	public BaseLogic[] inputs;
	
	[Header("True Events")]
	public InteractionHandler.InvokableState onBecomeTrue;
	public InteractionHandler.InvokableState onTrueCheck;
	
	[Header("False Events")]
	public InteractionHandler.InvokableState onBecomeFalse;
	public InteractionHandler.InvokableState onFalseCheck;
	
	[Header("Debug")]
	public bool outputStates;
	
	private bool localIsTrue;
	private bool lastState;
	
	void Start () {
		if (gateType == GateType.AND) {
			localIsTrue = true;
			for (int i = 0; i < inputs.Length; i++) {
				if (!inputs[i].IsTrue) {
					localIsTrue = false;
					break;
				}
			}
		} else {
			localIsTrue = false;
			for (int i = 0; i < inputs.Length; i++) {
				if (inputs[i].IsTrue) {
					localIsTrue = true;
					break;
				}
			}
		}
		lastState = isTrue;
	}
	
	public void UpdateLogic () {
		if (gateType == GateType.AND) {
			localIsTrue = true;
			for (int i = 0; i < inputs.Length; i++) {
				if (!inputs[i].IsTrue) {
					localIsTrue = false;
					break;
				}
			}
		} else {
			localIsTrue = false;
			for (int i = 0; i < inputs.Length; i++) {
				if (inputs[i].IsTrue) {
					localIsTrue = true;
					break;
				}
			}
		}
		IsTrue = localIsTrue;
		if (outputStates) {
			string states = "";
			for (int i = 0; i < inputs.Length; i++) {
				states += inputs[i].gameObject.name + " = " + inputs[i].IsTrue + " | ";
			}
			states += gameObject.name + " = " + localIsTrue;
			print(states);
		}
		if (localIsTrue) {
			onTrueCheck.Invoke();
			if (lastState != localIsTrue)
				onBecomeTrue.Invoke();
		} else {
			onFalseCheck.Invoke();
			if (lastState != localIsTrue)
				onBecomeFalse.Invoke();
		}
		lastState = isTrue;
	}
}
