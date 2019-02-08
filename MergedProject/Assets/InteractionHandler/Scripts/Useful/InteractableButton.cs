using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableButton : Interactable {
	
	[Header("Movement")]
	public Vector3 depressTo;
	public float depressTime = 0.25f;
	[Range(0,1)]
	public float activationPercent = 0.75f;
	
	[Header("Behavior")]
	public string activationAxis;
	public bool isToggle;
	public InteractionHandler.InvokableState onAction;
	public InteractionHandler.InvokableState offAction;
	
	private Vector3 origin;
	private bool depressing;
	private bool depressed;
	private Coroutine depression;
	private float timer;
	private bool invokedOn;
	private bool invokedOff;
	
	void Start () {
		origin = transform.localPosition;
	}
	
	
	public override void IHCursorEnter () {
		onCursorEnter.Invoke();
	}
	
	public override void IHCursorStay () {
		onCursorStay.Invoke();
	}
	
	public override void IHCursorExit () {
		onCursorExit.Invoke();
	}
	
	// Called on the axis initial press/activation
	public override void IHButtonDown (InteractionHandler.InteractionParameters iParam) {
		if (iParam.axis != activationAxis)
			return;
		if (depressing) {
			StopCoroutine(depression);
			depressing = false;
		}
		if (isToggle)
			depressed = !depressed;
		else
			depressed = true;
		depression = StartCoroutine(Depress());
	}
	
	// Called when holding an axis while entering this object
	public override void IHButtonEnter (InteractionHandler.InteractionParameters iParam) {
		if (iParam.axis != activationAxis)
			return;
		if (depressing) {
			StopCoroutine(depression);
			depressing = false;
		}
		if (isToggle)
			depressed = !depressed;
		else
			depressed = true;
		depression = StartCoroutine(Depress());
	}
	
	// Called when holding an axis while exiting this object
	public override void IHButtonExit (InteractionHandler.InteractionParameters iParam) {
		if (iParam.axis != activationAxis || isToggle)
			return;
		if (depressing) {
			StopCoroutine(depression);
			depressing = false;
		}
		depressed = false;
		depression = StartCoroutine(Depress());
	}
	
	// Called when the axis is released
	public override void IHButtonUp (InteractionHandler.InteractionParameters iParam) {
		if (iParam.axis != activationAxis || isToggle)
			return;
		if (depressing) {
			StopCoroutine(depression);
			depressing = false;
		}
		depressed = false;
		depression = StartCoroutine(Depress());
	}
	
	
	public override void IHOnTriggerEnter () {
		onCursorEnter.Invoke();
	}
	
	public override void IHOnTriggerStay () {
		onCursorStay.Invoke();
	}
	
	public override void IHOnTriggerExit () {
		onCursorExit.Invoke();
	}
	
	
	public void SetDepressionLevel (float point) {
		timer = Mathf.Clamp(point, 0f, 1f);
		transform.localPosition = Vector3.Lerp(origin, origin+depressTo, timer);
		
		if (!invokedOn && timer >= activationPercent) {
			onAction.Invoke();
			invokedOn = true;
		} else if (invokedOn && timer < activationPercent) {
			invokedOn = false;
		}
		if (invokedOff && timer >= activationPercent) {
			onAction.Invoke();
			invokedOff = false;
		} else if (!invokedOff && timer < activationPercent) {
			invokedOff = true;
		}
	}
	
	IEnumerator Depress () {
		depressing = true;
		while ((depressed && timer < 1f-Time.deltaTime/depressTime) || (!depressed && timer > Time.deltaTime/depressTime)) {
			timer += Time.deltaTime/depressTime * (depressed ? 1f : -1f);
			transform.localPosition = Vector3.Lerp(origin, origin+depressTo, timer);
			
			if (!invokedOn && timer >= activationPercent) {
				onAction.Invoke();
				invokedOn = true;
			} else if (invokedOn && timer < activationPercent) {
				invokedOn = false;
			}
			
			if (!invokedOff && timer <= activationPercent) {
				offAction.Invoke();
				invokedOff = true;
			} else if (invokedOff && timer > activationPercent) {
				invokedOff = false;
			}
			
			yield return null;
		}
		
		SetDepressionLevel(depressed ? 1f : 0);
		depressing = false;
	}
	
	void OnDrawGizmos () {
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, transform.position+depressTo);
		Gizmos.DrawWireCube(transform.position+depressTo, new Vector3(0.1f,0.1f,0.1f));
	}
	
	public bool IsInteractiable {
		get { return isInteractiable; }
		set { isInteractiable = value; }
	}
}