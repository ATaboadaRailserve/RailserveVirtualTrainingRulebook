using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewableObject : Interactable {
	
	[Header("Viewable Object")]
	public AnimationCurve movementCurve;
	public float moveTime;
	public float distanceFromFace;
	public bool restAtOriginalRotation;
	
	[Header("View Events")]
	public InteractionHandler.InvokableState onViewBegin;
	public InteractionHandler.InvokableState onViewStay;
	public InteractionHandler.InvokableState onViewEnd;
	
	[Header("Button Events")]
	public InteractionHandler.InvokableState onButtonDown;
	public InteractionHandler.InvokableState onButtonEnter;
	public InteractionHandler.InvokableState onButton;
	public InteractionHandler.InvokableState onButtonExit;
	public InteractionHandler.InvokableState onButtonUp;
	
	[Header("Behavior")]
	public Transform targetTransform;
	public string activationAxis;
	public float xSensitivity;
	public float ySensitivity;
	[Range(0,1)]
	public float friction = 0.25f;
	public Collider activationCollider;
	public bool alternateActivationMethod;
	
	[Header("Nested Viewables")]
	public ViewableObject parentViewable;
	public ViewableObject[] childViewables;
	
	private Vector3 originPos;
	private Quaternion originRot;
	
	private bool interacting;
	private bool active;
	private float moveTimer;
	private Vector3 prevRot;
	private Vector3 inertia;
	private Transform m_targetTransform;
	private List<int> layeredObjects;
	
	private int layerIndex;
	private Vector3 workerVec;
	private Quaternion workerQuat;
	private Quaternion currentRot;
	
	private Coroutine co;
	
	void Start () {
		// Set up the hungarianized transform
		if (targetTransform) {
			m_targetTransform = targetTransform;
		} else {
			m_targetTransform = transform;
		}
		
		// Grab the layers of all children to turn them UI on begin view
		if (!parentViewable) {
			active = true;
			
			layeredObjects = new List<int>();
			InitiateLayers(m_targetTransform);
			layeredObjects.Add(m_targetTransform.gameObject.layer);
			
			// Call to turn off all children
			StartCoroutine(InitializeChildren());
		}
		
		// Grab the local position and rotation of the object as the rest anchor
		originPos = m_targetTransform.localPosition;
		originRot = m_targetTransform.localRotation;
	}
	
	IEnumerator InitializeChildren () {
		yield return null; // Wait a frame to ensure all children hit their Start functions
		RecurseChildrenEnabled();
	}
	
	// Recurse through all children and nested children to deactivate their viewable objects (Top parent only should be on at the start)
	public void RecurseChildrenEnabled (bool activate = false) {
		foreach (ViewableObject v in childViewables) {
			v.RecurseChildrenEnabled(activate);
			v.Active = activate;
			v.activationCollider.enabled = activate;
		}
	}
	
	// Recurse through all children to store their layers
	void InitiateLayers (Transform t) {
		for (int i = 0; i < t.childCount; i++) {
			InitiateLayers(t.GetChild(i));
		}
		layeredObjects.Add(t.gameObject.layer);
	}
	
	// Recurse through all children to set their layers.  If layer is -1, set to the stored layer (Assumed to be returning to rest)
	void SetLayers (Transform t, int layer = -1) {
		for (int i = 0; i < t.childCount; i++) {
			SetLayers(t.GetChild(i), layer);
		}
		t.gameObject.layer = (layer != -1 ? layer : layeredObjects[layerIndex]);
		layerIndex++;
	}
	
	void Update () {
		if (!active)
			return;
		// Do inertia spins
		if (inertia.sqrMagnitude != 0) {
			m_targetTransform.localEulerAngles += inertia;
			inertia -= inertia-inertia*friction;
		}
	}
	
	#region Movement
	
	// Move to either rest or in front of the player
	IEnumerator ViewMove () {
		// Disallow the parent to be interacted with before moving
		if (parentViewable) {
			parentViewable.Active = false;
			parentViewable.activationCollider.enabled = false;
		}
		
		moveTimer = 0;
		
		// Grab the position and rotation of the player camera
		workerVec = interactionHandler.playerController.cameraTransform.position + interactionHandler.playerController.cameraTransform.forward * distanceFromFace;
		workerQuat = Quaternion.Euler (interactionHandler.playerController.cameraTransform.eulerAngles - new Vector3(180,180,180));
		// And the current global rotation of the viewable
		currentRot = m_targetTransform.rotation;
		
		// Do the move
		while (moveTimer <= 1-Time.deltaTime) {
			moveTimer += Time.deltaTime / moveTime;
			
			if (parentViewable || transform.parent) {
				m_targetTransform.position = Vector3.Lerp(transform.parent.TransformPoint(originPos), workerVec, moveTimer);
			} else {
				m_targetTransform.position = Vector3.Lerp(originPos, workerVec, moveTimer);
			}
			m_targetTransform.rotation = Quaternion.Lerp(currentRot, workerQuat, moveTimer);
			
			yield return null;
		}
		
		moveTimer = 1;
		if (parentViewable) {
			m_targetTransform.position = workerVec;
		} else {
			m_targetTransform.position = workerVec;
			if (!interacting)
				active = true;
		}
		m_targetTransform.rotation = workerQuat;
		
		foreach (ViewableObject v in childViewables) {
			v.Active = true;
			v.activationCollider.enabled = true;
		}
		
		co = null;
	}
	
	IEnumerator ReturnMove () {
		if (originPos == m_targetTransform.localPosition) {
			// If this object isn't interacting and is already at rest, skip out of this coroutine
		} else {
			moveTimer = 0;
			
			// Grab the position and rotation of the player camera
			workerVec = interactionHandler.playerController.cameraTransform.position + interactionHandler.playerController.cameraTransform.forward * distanceFromFace;
			workerQuat = Quaternion.Euler (interactionHandler.playerController.cameraTransform.eulerAngles - new Vector3(180,180,180));
			// And the current global rotation of the viewable
			currentRot = m_targetTransform.rotation;
			
			// Do the move
			while (moveTimer <= 1-Time.deltaTime) {
				moveTimer += Time.deltaTime / moveTime;
				
				if (parentViewable || transform.parent) {
					m_targetTransform.position = Vector3.Lerp(transform.parent.TransformPoint(originPos), workerVec, (interacting ? moveTimer : 1f-moveTimer));
				} else {
					m_targetTransform.position = Vector3.Lerp(workerVec, originPos, moveTimer);
				}
				m_targetTransform.localRotation = Quaternion.Lerp(workerQuat, originRot, moveTimer);
				
				yield return null;
			}
			
			moveTimer = 1;
			m_targetTransform.localPosition = originPos;
			m_targetTransform.localRotation = originRot;
			
			// Allow the parent to be interacted with after moving if returning
			if (parentViewable) {
				parentViewable.Active = true;
				parentViewable.activationCollider.enabled = true;
			}
			RecurseChildrenEnabled();
		}
		
		co = null;
	}
	
	#endregion
	
	#region ButtonCallbacks
	
	public override void IHButtonDown (InteractionHandler.InteractionParameters iParam) {
		if (iParam.axis != activationAxis || !active)
			return;
		// Disallow the parent to be interacted with while interacting
		if (parentViewable) {
			parentViewable.Active = false;
			parentViewable.activationCollider.enabled = false;
		}
		// Disallow the children from being interacted with while interacting
		foreach (ViewableObject v in childViewables) {
			v.Active = false;
			v.activationCollider.enabled = false;
		}
		onButtonDown.Invoke();
	}
	
	public override void IHButtonEnter (InteractionHandler.InteractionParameters iParam) { if (active) return; if (iParam.axis == activationAxis) onButtonEnter.Invoke(); }
	public override void IHButtonExit (InteractionHandler.InteractionParameters iParam) { if (active) return; if (iParam.axis == activationAxis) onButtonExit.Invoke(); }
	
	public override void IHButtonOriginal (InteractionHandler.InteractionParameters iParam) {
		if (!interacting || iParam.axis != activationAxis || !active)
			return;
		onButton.Invoke();
		inertia = Vector3.zero; // Kill inertia since we've grabbed the object
		Cursor.lockState = CursorLockMode.Locked; // Lock the mouse so we don't interact and can continue manipulating the viewable
		SetAngle(iParam.deltaLook);
	}
	
	public override void IHButtonUp (InteractionHandler.InteractionParameters iParam) {
		if (alternateActivationMethod || iParam.axis != activationAxis || !active)
			return;
		
		// Allow child viewables to be interacted with
		foreach (ViewableObject v in childViewables) {
			v.Active = true;
			v.activationCollider.enabled = true;
		}
		
		onButtonUp.Invoke();
		if (!interacting)
			BeginView(); // If clicking on an object that we're not interacting with, start doing so.
		if (interacting) {
			inertia = m_targetTransform.localEulerAngles - prevRot; // Set the inertia based on the delta rotation now that we've let go.
			// Allow the children from being interacted with
			foreach (ViewableObject v in childViewables) {
				v.Active = true;
				v.activationCollider.enabled = true;
			}
		}
	}
	
	#endregion
	
	#region ViewCallbacks
	
	public void BeginView () {
		if (interacting || !active)
			return;
		
		GameObject.FindWithTag("EscapeCursor").GetComponent<EscapeCursor>().IsCurrentlyEscaped = true;
		
		onViewBegin.Invoke();
		
		if (!parentViewable) {
			// Recurse through all children to set their layers
			layerIndex = 0;
			SetLayers(m_targetTransform, 5);
			m_targetTransform.gameObject.layer = 5;
		} else {
			parentViewable.Interacting = false;
		}
		
		interacting = true;
		
		if (!parentViewable) {
			// Prevent player position and rotation from changing
			interactionHandler.PlayerCanWalk = false;
			interactionHandler.PlayerCanLook = false;
			interactionHandler.PlayerIsKinematic = true;
		}
		
		co = StartCoroutine(ViewMove());
	}
	
	protected void SoftBeginView () {
		GameObject.FindWithTag("EscapeCursor").GetComponent<EscapeCursor>().IsCurrentlyEscaped = true;
		onViewBegin.Invoke();
		
		if (!parentViewable) {
			// Recurse through all children to set their layers
			layerIndex = 0;
			SetLayers(m_targetTransform, 5);
			m_targetTransform.gameObject.layer = 5;
		} else {
			parentViewable.Interacting = false;
		}
		
		interacting = true;
		
		// Disallow the parent to be interacted with before moving
		if (parentViewable) {
			parentViewable.Active = false;
			parentViewable.activationCollider.enabled = false;
		}
		
		// Grab the position and rotation of the player camera
		workerVec = interactionHandler.playerController.cameraTransform.position + interactionHandler.playerController.cameraTransform.forward * distanceFromFace;
		workerQuat = Quaternion.Euler (interactionHandler.playerController.cameraTransform.eulerAngles - new Vector3(180,180,180));
		// And the current global rotation of the viewable
		currentRot = m_targetTransform.rotation;
		
		moveTimer = 1;
		m_targetTransform.position = workerVec;
		m_targetTransform.rotation = workerQuat;
		
		foreach (ViewableObject v in childViewables) {
			v.Active = true;
			v.activationCollider.enabled = true;
		}
	}
	
	public void EndView () {
		GameObject.FindWithTag("EscapeCursor").GetComponent<EscapeCursor>().IsCurrentlyEscaped = false;
		// Allow the parent to be interacted with
		if (parentViewable) {
			parentViewable.Interacting = true;
			parentViewable.Active = true;
			parentViewable.activationCollider.enabled = true;
		} else { // Unless there is no parent in which, recurse through everything to return them back to their original layers
			layerIndex = 0;
			SetLayers(m_targetTransform);
			m_targetTransform.gameObject.layer = layeredObjects[layerIndex];
		}
		
		// Disallow the children from being interacted with
		foreach (ViewableObject v in childViewables) {
			v.Active = false;
			v.activationCollider.enabled = false;
		}
		
		onViewEnd.Invoke();
		
		interacting = false;
		
		if (!parentViewable) {
			// Allow the player to exist again
			interactionHandler.PlayerCanWalk = true;
			interactionHandler.PlayerCanLook = true;
			interactionHandler.PlayerIsKinematic = false;
		}
		
		ReturnToRest();
	}
	
	protected void SoftEndView () {
		GameObject.FindWithTag("EscapeCursor").GetComponent<EscapeCursor>().IsCurrentlyEscaped = false;
		if(co != null)
			StopCoroutine(co);
		
		// Allow the parent to be interacted with
		if (parentViewable) {
			parentViewable.Interacting = true;
			parentViewable.Active = true;
			parentViewable.activationCollider.enabled = true;
		} else { // Unless there is no parent in which, recurse through everything to return them back to their original layers
			layerIndex = 0;
			SetLayers(m_targetTransform);
			m_targetTransform.gameObject.layer = layeredObjects[layerIndex];
		}
		
		// Disallow the children from being interacted with
		foreach (ViewableObject v in childViewables) {
			v.Active = false;
			v.activationCollider.enabled = false;
		}
		
		onViewEnd.Invoke();
		
		interacting = false;
		
		// Bottom up, return all children to rest
		foreach (ViewableObject v in childViewables) {
			v.ReturnToRest();
		}
		// Then return this to rest
		interacting = false;
		
		// Grab the position and rotation of the player camera
		workerVec = interactionHandler.playerController.cameraTransform.position + interactionHandler.playerController.cameraTransform.forward * distanceFromFace;
		workerQuat = Quaternion.Euler (interactionHandler.playerController.cameraTransform.eulerAngles - new Vector3(180,180,180));
		// And the current global rotation of the viewable
		currentRot = m_targetTransform.rotation;
		
		moveTimer = 1;
		m_targetTransform.localPosition = originPos;
		m_targetTransform.localRotation = originRot;
		
		// Allow the parent to be interacted with after moving if returning
		if (parentViewable) {
			parentViewable.Active = true;
			parentViewable.activationCollider.enabled = true;
		}
		RecurseChildrenEnabled();
	}
	
	public void JumpToView () {
		EndAllChildrenViews();
		ViewDownTo(true);
	}
	
	protected void ViewDownTo(bool target = false) {
		if (parentViewable) {
			parentViewable.ViewDownTo();
		}
		if (target) {
			BeginView();
		} else {
			SoftBeginView();
		}
	}
	
	protected void EndAllChildrenViews () {
		if (parentViewable) {
			parentViewable.EndAllChildrenViews();
		} else {
			EndChildrenViews();
		}
	}
	
	protected void EndChildrenViews () {
		foreach (ViewableObject v in childViewables) {
			v.EndChildrenViews();
		}
		SoftEndView();
	}
	
	#endregion
	
	public void ReturnToRest () {
		// Bottom up, return all children to rest
		foreach (ViewableObject v in childViewables) {
			v.ReturnToRest();
		}
		// Then return this to rest
		interacting = false;
		co = StartCoroutine(ReturnMove());
	}
	
	public void SetAngle (Vector2 deltaLook) {
		deltaLook.x *= xSensitivity;
		deltaLook.y *= ySensitivity;
		
		prevRot = m_targetTransform.localEulerAngles;
		
		m_targetTransform.RotateAround(transform.position, Vector3.up, -deltaLook.x);
		m_targetTransform.RotateAround(transform.position, interactionHandler.playerController.transform.right, deltaLook.y);
	}
	
	public bool IsInteractable {
		get { return isInteractiable; }
		set { isInteractiable = value; }
	}
	
	public bool Active {
		get { return active; }
		set { active = value; }
	}
	
	public bool Interacting {
		get { return interacting; }
		set { interacting = value; }
	}
}