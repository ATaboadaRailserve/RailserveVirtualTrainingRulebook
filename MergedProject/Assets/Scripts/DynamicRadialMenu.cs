using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class DynamicRadialMenu : MonoBehaviour {
	
	/*
	
	Add in support for icons and text for the radial button
	Will probably have to redo all of it.  Hop to etc.
	
	*/
	
	[System.Serializable]
	public struct RadialButton {
		public bool disabled;
		public string text;
		public Color textColor;
		public Sprite icon;
		public Color iconColor;
		public Sprite backGroundGraphic;
		public Color backGroundColor;
		public Sprite buttonGraphic;
		public Color buttonColorIdle;
		public Color buttonColorDisabled;
		public Color buttonColorMouseOver;
		public Color buttonColorMouseClick;
		public InteractionHandler.InvokableState m_OnMouseEnter;
		public InteractionHandler.InvokableState m_OnClick;
		public InteractionHandler.InvokableState m_OnClickNextFrame;
		public InteractionHandler.InvokableState m_OnMouseExit;
		[HideInInspector]
		public Image bgImage;
		[HideInInspector]
		public Image buttonImage;
		[HideInInspector]
		public bool isClicked;
	}
	
	public InteractionHandler interactionHandler;
	
	[Header("Wheel Properties")]
	public RadialButton[] radialButtons;
	[Range(0,360)]
	public float startAngle;
	[Range(0,360)]
	public float gap;
	public bool ccw;
	public int size;
	
	[Header("Icon Properties")]
	public float iconSize = 0.25f;
	public float iconOffset = 0;
	public Font font;
	public bool displayText;
	public Vector3 textOffset;
	
	public enum ActivationType { OnClick, OnHighlight, OnRelease };
	[Header("Interactions")]
	public ActivationType activationType;
	public bool overrideActivation;
	public string radioActivationAxis;
	public InteractionHandler.InvokableState onActivate;
	public InteractionHandler.InvokableState whileActivated;
	public InteractionHandler.InvokableState onDeactivate;
	public string buttonActivationAxis;
	
	[Header("Leave 0,0 for full screen")]
	public Vector2 activationRange;
	
	private GameObject holder;
	private Vector3 startAngleVec;
	private Vector3 workerVec;
	private float angle;
	private float degreeToRad = Mathf.PI/180f;
	private float radsToDegree = 180f/Mathf.PI;
	private int currentClick = -1;
	private bool activated;
	private float workerFloat;
	
	#region Rewired Stuff
	private Player player;
	void Awake() {
		player = ReInput.players.GetPlayer(0); // get the Rewired Player
	}
	#endregion
	
	void Start () {
		startAngleVec = new Vector3(Mathf.Cos((startAngle)*degreeToRad), Mathf.Sin((startAngle)*degreeToRad), 0);
		
		holder = new GameObject();
		holder.name = "ButtonHolder";
		holder.transform.parent = transform;
		holder.transform.localPosition = Vector3.zero;
		holder.transform.localEulerAngles = Vector3.zero;
		GameObject temp;
		GameObject tempGraphic;
		GameObject tempText;
		float divisions = radialButtons.Length;
		for (int i = 0; i < (int)divisions; i++) {
			workerFloat = Mathf.Clamp(1f/divisions - gap/360f, 0, 1);
			temp = new GameObject();
			temp.transform.parent = holder.transform;
			temp.transform.localPosition = Vector3.zero;
			temp.transform.localEulerAngles = new Vector3(0,0,startAngle + (360f/divisions)*(i*(ccw ? -1 : 1)) + (gap/720f)*i);
			temp.name = radialButtons[i].text;
			
			if (radialButtons[i].backGroundGraphic) {
				tempGraphic = new GameObject();
				tempGraphic.transform.parent = temp.transform;
				tempGraphic.transform.localPosition = new Vector3(0,0,1);
				tempGraphic.transform.localEulerAngles = Vector3.zero;
				tempGraphic.AddComponent<RectTransform>().sizeDelta = new Vector2(size, size);
				radialButtons[i].bgImage = tempGraphic.AddComponent<Image>();
				radialButtons[i].bgImage.sprite = radialButtons[i].backGroundGraphic;
				radialButtons[i].bgImage.color = radialButtons[i].backGroundColor;
				radialButtons[i].bgImage.type = Image.Type.Filled;
				radialButtons[i].bgImage.fillMethod = Image.FillMethod.Radial360;
				radialButtons[i].bgImage.fillAmount = workerFloat;
				tempGraphic.name = "Background";
			}
			
			if (radialButtons[i].buttonGraphic) {
				tempGraphic = new GameObject();
				tempGraphic.transform.parent = temp.transform;
				tempGraphic.transform.localPosition = Vector3.zero;
				tempGraphic.transform.localEulerAngles = Vector3.zero;
				tempGraphic.AddComponent<RectTransform>().sizeDelta = new Vector2(size, size);
				radialButtons[i].buttonImage = tempGraphic.AddComponent<Image>();
				radialButtons[i].buttonImage.sprite = radialButtons[i].buttonGraphic;
				if (radialButtons[i].disabled)
					radialButtons[i].buttonImage.color = radialButtons[i].buttonColorDisabled;
				else
					radialButtons[i].buttonImage.color = radialButtons[i].buttonColorIdle;
				radialButtons[i].buttonImage.type = Image.Type.Filled;
				radialButtons[i].buttonImage.fillMethod = Image.FillMethod.Radial360;
				radialButtons[i].buttonImage.fillAmount = workerFloat;
				tempGraphic.name = "Button";
			}
			
			if (radialButtons[i].icon) {
				tempGraphic = new GameObject();
				tempGraphic.transform.parent = temp.transform;
				tempGraphic.AddComponent<RectTransform>().sizeDelta = new Vector2(size/divisions * iconSize, size/divisions * iconSize);
				tempGraphic.transform.eulerAngles = new Vector3(0,0,180 - 360f/divisions/2f);
				
				tempGraphic.transform.localPosition = tempGraphic.transform.up * (size/2f - (size/divisions * iconSize) - iconOffset);
				tempGraphic.transform.localPosition += Vector3.forward;
				
				tempGraphic.transform.eulerAngles = Vector3.zero;
				tempGraphic.AddComponent<Image>();
				tempGraphic.GetComponent<Image>().sprite = radialButtons[i].icon;
				tempGraphic.GetComponent<Image>().color = radialButtons[i].iconColor;
				tempGraphic.name = "Icon";
				
				if (displayText) {
					tempText = new GameObject();
					tempText.transform.parent = tempGraphic.transform;
					tempText.AddComponent<RectTransform>().sizeDelta = new Vector2(size/divisions * iconSize, size/divisions * iconSize);
					tempText.transform.localPosition = textOffset;
					tempText.transform.localEulerAngles = Vector3.zero;
					tempText.AddComponent<Text>();
					tempText.GetComponent<Text>().font = font;
					tempText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
					tempText.GetComponent<Text>().text = radialButtons[i].text;
					tempText.GetComponent<Text>().color = radialButtons[i].textColor;
					tempText.name = "Text";
				}
			}
		}
		holder.SetActive(false);
	}
	
	void Update () {
		if (!player.GetButton(radioActivationAxis)) {
			if (activated) {
				activated = false;
				currentClick = -1;
				MenuVisibility(false);
				if (interactionHandler.LockInteraction)
					interactionHandler.LockInteraction = false;
				onDeactivate.Invoke();
			}
		}
		
		if (overrideActivation) {
			if (holder.activeInHierarchy)
				MenuVisibility(false);
			return;
		}
		
		if (player.GetButtonDown(radioActivationAxis)) {
			MenuVisibility(true);
			if (!interactionHandler.LockInteraction)
				interactionHandler.LockInteraction = true;
			onActivate.Invoke();
		}
		
		if (player.GetButton(radioActivationAxis)) {
			if (!activated)
				activated = true;
			if (!overrideActivation && !holder.activeInHierarchy)
				MenuVisibility(true);
			if (!interactionHandler.LockInteraction)
				interactionHandler.LockInteraction = true;
			MouseAngleAndButtonReaction();
			whileActivated.Invoke();
		}
	}
	
	private void MenuVisibility(bool visibility) {
		foreach (Interactable i in GameObject.FindObjectsOfType<Interactable>()) {
			i.OverrideInteraction = visibility;
		}
		holder.SetActive(visibility);
	}
	
	private void MouseAngleAndButtonReaction () {
		for (int i = 0; i < radialButtons.Length; i++) {
			if (radialButtons[i].isClicked || currentClick == i) {
				continue;
			}
			if (radialButtons[i].disabled)
				radialButtons[i].buttonImage.color = radialButtons[i].buttonColorDisabled;
			else
				radialButtons[i].buttonImage.color = radialButtons[i].buttonColorIdle;
		}
		workerVec = (Input.mousePosition - transform.position);
		
		if (activationRange.x != activationRange.y && (workerVec.sqrMagnitude < activationRange.x*activationRange.x || workerVec.sqrMagnitude > activationRange.y*activationRange.y)) {
			for (int i = 0; i < radialButtons.Length; i++) {
				if (currentClick == i) {
					radialButtons[currentClick].isClicked = false;
					if (radialButtons[currentClick].disabled)
						radialButtons[currentClick].buttonImage.color = radialButtons[currentClick].buttonColorDisabled;
					else
						radialButtons[currentClick].buttonImage.color = radialButtons[currentClick].buttonColorMouseOver;
					radialButtons[currentClick].m_OnMouseExit.Invoke();
					currentClick = -1;
				}
			}
			return;
		}
		
		workerVec.x /= Screen.width/2f;
		workerVec.y /= Screen.height/2f;
		workerVec = workerVec.normalized;
		int sign = Vector3.Cross(startAngleVec, workerVec).z < 0 ? -1 : 1;
		angle = sign * Vector3.Angle(startAngleVec, workerVec)/360f;
		angle += 0.25f;
		angle += 1f/radialButtons.Length;
		
		if (angle >= 1f)
			angle -= 1f;
		if (angle < 0f)
			angle += 1f;
		angle = Mathf.Clamp(angle, 0, 0.999999f);
		angle *= radialButtons.Length;
		
		angle = Mathf.Floor(angle);
		angle %= radialButtons.Length;
		
		if (currentClick == -1) {
			if (!radialButtons[(int)angle].disabled) {
				radialButtons[(int)angle].buttonImage.color = radialButtons[(int)angle].buttonColorMouseOver;
				radialButtons[(int)angle].m_OnMouseEnter.Invoke();
			} else {
				radialButtons[(int)angle].buttonImage.color = radialButtons[(int)angle].buttonColorDisabled;
			}
		} else if (currentClick != (int)angle) {
			if (radialButtons[currentClick].disabled) {
				radialButtons[currentClick].isClicked = false;
				radialButtons[currentClick].buttonImage.color = radialButtons[currentClick].buttonColorDisabled;
				
				radialButtons[(int)angle].buttonImage.color = radialButtons[(int)angle].buttonColorDisabled;
			} else {
				radialButtons[currentClick].isClicked = false;
				radialButtons[currentClick].buttonImage.color = radialButtons[currentClick].buttonColorMouseOver;
				radialButtons[currentClick].m_OnMouseExit.Invoke();
				
				radialButtons[(int)angle].buttonImage.color = radialButtons[(int)angle].buttonColorMouseOver;
				radialButtons[(int)angle].m_OnMouseEnter.Invoke();
			}
		}
		
		currentClick = (int)angle;
		
		if (radialButtons[currentClick].disabled) {
			radialButtons[currentClick].buttonImage.color = radialButtons[currentClick].buttonColorDisabled;
			return;
		}
		
		if (!radialButtons[currentClick].isClicked && activationType == ActivationType.OnHighlight) {
			radialButtons[currentClick].buttonImage.color = radialButtons[currentClick].buttonColorMouseClick;
			StartCoroutine(DoClickEvents(radialButtons[currentClick].m_OnClick, radialButtons[currentClick].m_OnClickNextFrame));
			radialButtons[currentClick].isClicked = true;
			return;
		}
		
		if (!radialButtons[currentClick].isClicked && activationType == ActivationType.OnClick && player.GetButtonDown(buttonActivationAxis)) {
			radialButtons[currentClick].buttonImage.color = radialButtons[currentClick].buttonColorMouseClick;
			StartCoroutine(DoClickEvents(radialButtons[currentClick].m_OnClick,radialButtons[currentClick].m_OnClickNextFrame));
		}
		
		if (!radialButtons[currentClick].isClicked && activationType == ActivationType.OnRelease && player.GetButtonUp(buttonActivationAxis)) {
			radialButtons[currentClick].buttonImage.color = radialButtons[currentClick].buttonColorMouseOver;
			StartCoroutine(DoClickEvents(radialButtons[currentClick].m_OnClick, radialButtons[currentClick].m_OnClickNextFrame));
		}
		
		if (player.GetButtonUp(buttonActivationAxis)) {
			radialButtons[currentClick].buttonImage.color = radialButtons[currentClick].buttonColorMouseOver;
		}
	}
	
	IEnumerator DoClickEvents (InteractionHandler.InvokableState firstEvent, InteractionHandler.InvokableState secondEvent) {
		print(gameObject.name);
		yield return null;
		firstEvent.Invoke();
		yield return null;
		yield return null;
		secondEvent.Invoke();
	}
	
	public void OverrideRadialActivation (bool state) {
		overrideActivation = state;
	}
	
	public void ToggleButtonEnabled (int index) {
		radialButtons[index].disabled = !radialButtons[index].disabled;
	}

    public void SetButtonEnabled(int index) {
        radialButtons[index].disabled = false;
    }

    public void SetButtonDisabled(int index) {
        radialButtons[index].disabled = true;
    }
}
