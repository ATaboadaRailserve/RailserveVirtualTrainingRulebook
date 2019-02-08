﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableLever : Interactable {
	
	[System.Serializable]
	public struct State {
		public bool disabled;
		public Vector2 primaryAxisRange;
		public Vector2 secondaryAxisRange;
		//public bool radialRange; // Needs equation for Ellipse and normalization of lever heading to work properly
		public InteractionHandler.InvokableState onAction;
		public InteractionHandler.InvokableState offAction;
		public bool invokeEveryFrame;
		
		[HideInInspector]
		public bool invoked;
	}
	
	[System.Serializable]
	public struct MovementAxis {
		public Vector2 angleLimits;
		public Axis leverAxis;
		public bool negateAxis;
		public bool dontNegateSwappedAxis;
		public float sensitivity;
		public bool ignorePlayerPosition;
		public bool randomizeAngle;
		public float randomizeWeight;
		
		[HideInInspector]
		public Vector3 _axis;
	}
	
	[Header("Movement")]
	public Transform targetTransform;
	public MovementAxis xLookAxis;
	public MovementAxis yLookAxis;
	public bool playerRelativeAxisSwap; // Swap axis based on whether the player is to the side of the object or not
	
	[Header("States")]
	public State[] states;
	
	private Vector3 rotation;
	private Vector3 playerNormalized;
	private float workerFloat;
	private Transform m_targetTransform;
	
	void Start () {
		
		if (targetTransform) {
			m_targetTransform = targetTransform;
		} else {
			m_targetTransform = transform;
		}
		
		lockToThisWhileInteracting = true;
		
		switch ((int)xLookAxis.leverAxis) {
			case 0: // X
				xLookAxis._axis = m_targetTransform.right;
				break;
			case 1: // Y
				xLookAxis._axis = m_targetTransform.up;
				break;
			case 2: // Z
				xLookAxis._axis = m_targetTransform.forward;
				break;
		}
		
		switch ((int)yLookAxis.leverAxis) {
			case 0: // X
				yLookAxis._axis = m_targetTransform.right;
				break;
			case 1: // Y
				yLookAxis._axis = m_targetTransform.up;
				break;
			case 2: // Z
				yLookAxis._axis = m_targetTransform.forward;
				break;
		}
		
		StartCoroutine(Initial());
	}
	
	void OnEnabled () {
		if (m_targetTransform)
			return;
		
		if (targetTransform) {
			m_targetTransform = targetTransform;
		} else {
			m_targetTransform = transform;
		}
		
		lockToThisWhileInteracting = true;
		
		switch ((int)xLookAxis.leverAxis) {
			case 0: // X
				xLookAxis._axis = m_targetTransform.right;
				break;
			case 1: // Y
				xLookAxis._axis = m_targetTransform.up;
				break;
			case 2: // Z
				xLookAxis._axis = m_targetTransform.forward;
				break;
		}
		
		switch ((int)yLookAxis.leverAxis) {
			case 0: // X
				yLookAxis._axis = m_targetTransform.right;
				break;
			case 1: // Y
				yLookAxis._axis = m_targetTransform.up;
				break;
			case 2: // Z
				yLookAxis._axis = m_targetTransform.forward;
				break;
		}
	}
	
	IEnumerator Initial () {
		yield return null;
		
		SetAngle(new Vector2((xLookAxis.randomizeAngle ? (Random.value*2f-1f)*xLookAxis.randomizeWeight : 0), (yLookAxis.randomizeAngle ? (Random.value*2f-1f)*yLookAxis.randomizeWeight : 0)));
	}
	
	// Called on the axis initial press/activation
	public override void IHButtonDown (InteractionHandler.InteractionParameters iParam) {
		SetAngle(iParam.deltaLook);
	}
	
	// Called when holding an axis while entering this object
	public override void IHButtonEnter (InteractionHandler.InteractionParameters iParam) {
		SetAngle(iParam.deltaLook);
	}
	
	// Called when holding an axis while entering this object
	public override void IHButton (InteractionHandler.InteractionParameters iParam) {
		SetAngle(iParam.deltaLook);
	}
	
	// Called on the axis release
	public override void IHButtonUp (InteractionHandler.InteractionParameters iParam) {
		SetAngle(iParam.deltaLook);
	}
	
	public void SetAngle (Vector2 deltaLook) {
		Cursor.lockState = CursorLockMode.Locked;
		
		// Add the deltas to the localEulerAngles about their proper axis
		rotation = m_targetTransform.localEulerAngles;
		if (xLookAxis.angleLimits.x < 0 && rotation[(int)xLookAxis.leverAxis] > 180f) {
			rotation[(int)xLookAxis.leverAxis] -= 360f;
		}
		if (yLookAxis.angleLimits.x < 0 && rotation[(int)yLookAxis.leverAxis] > 180f) {
			rotation[(int)yLookAxis.leverAxis] -= 360f;
		}
		
		playerNormalized = interactionHandler.playerController.transform.position - transform.position;
		
		deltaLook.x *= xLookAxis.sensitivity;
		deltaLook.y *= yLookAxis.sensitivity;
		
		if (playerRelativeAxisSwap && Mathf.Abs(Vector3.Dot(playerNormalized, xLookAxis._axis)) < 0.5f) {
			workerFloat = deltaLook.x;
			deltaLook.x = deltaLook.y;
			deltaLook.y = workerFloat;
			deltaLook.x *= (xLookAxis.dontNegateSwappedAxis ? -1 : 1) * (xLookAxis.ignorePlayerPosition ? 1 : Mathf.Sign(Vector3.Dot(playerNormalized, yLookAxis._axis)));
			deltaLook.y *= (yLookAxis.dontNegateSwappedAxis ? -1 : 1) * (yLookAxis.ignorePlayerPosition ? 1 : Mathf.Sign(Vector3.Dot(playerNormalized, yLookAxis._axis)));
		} else {
			deltaLook.x *= (xLookAxis.negateAxis ? -1 : 1) * (xLookAxis.ignorePlayerPosition ? 1 : Mathf.Sign(Vector3.Dot(playerNormalized, xLookAxis._axis)));
			deltaLook.y *= (yLookAxis.negateAxis ? -1 : 1) * (yLookAxis.ignorePlayerPosition ? 1 : Mathf.Sign(Vector3.Dot(playerNormalized, xLookAxis._axis)));
		}
		
		if (xLookAxis.angleLimits.x != xLookAxis.angleLimits.y) {
			rotation[(int)xLookAxis.leverAxis] += deltaLook.x;
			rotation[(int)xLookAxis.leverAxis] = Mathf.Clamp(rotation[(int)xLookAxis.leverAxis], xLookAxis.angleLimits.x, xLookAxis.angleLimits.y);
		}
		if (yLookAxis.angleLimits.x != yLookAxis.angleLimits.y) {
			rotation[(int)yLookAxis.leverAxis] += deltaLook.y;
			rotation[(int)yLookAxis.leverAxis] = Mathf.Clamp(rotation[(int)yLookAxis.leverAxis], yLookAxis.angleLimits.x, yLookAxis.angleLimits.y);
		}
		
		m_targetTransform.localEulerAngles = rotation;
		
		for (int i = 0; i < states.Length; i++) {
			if (states[i].disabled)
				continue;
			if (   rotation[(int)xLookAxis.leverAxis] >= states[i].primaryAxisRange.x
				&& rotation[(int)xLookAxis.leverAxis] <= states[i].primaryAxisRange.y
				&& rotation[(int)yLookAxis.leverAxis] >= states[i].secondaryAxisRange.x
				&& rotation[(int)yLookAxis.leverAxis] <= states[i].secondaryAxisRange.y)
			{
				if ((!states[i].invokeEveryFrame && !states[i].invoked) || states[i].invokeEveryFrame) {
					states[i].onAction.Invoke();
					states[i].invoked = true;
				}
			} else if (states[i].invoked) {
				states[i].invoked = false;
				states[i].offAction.Invoke();
			}
		}
	}
	
	public bool IsInteractiable {
		get { return isInteractiable; }
		set { isInteractiable = value; }
	}
	
	public void EnableState (int state) {
		states[state].disabled = false;
	}
	
	public void DisableState (int state) {
		states[state].disabled = true;
	}
	
	public void ToggleState (int state) {
		states[state].disabled = !states[state].disabled;
	}
	
	public float XAxisPercent {
		get { return (m_targetTransform.localEulerAngles[(int)xLookAxis.leverAxis] - xLookAxis.angleLimits.x)/(xLookAxis.angleLimits.y - xLookAxis.angleLimits.x); }
	}
	
	public float YAxisPercent {
		get { return (m_targetTransform.localEulerAngles[(int)yLookAxis.leverAxis] - yLookAxis.angleLimits.x)/(yLookAxis.angleLimits.y - yLookAxis.angleLimits.x); }
	}
}