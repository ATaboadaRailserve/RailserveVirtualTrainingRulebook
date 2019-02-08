using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Rewired;

public class OptionsMenu : MonoBehaviour {
	
	[Header("Allow Menu")]
	public bool allowRedZoneChange = true;
	
	[Header("Var Setup")]
	public GlobalVars globalVars;
	public Vector3 offRot;
	public Vector3 onRot;
	public GameObject player;
	public InteractionHandler.InvokableState onMenuOpen;
	public InteractionHandler.InvokableState onMenuClose;
	
	[Header("UI Setup")]
	public Text redZoneTypeText;
	
	private int totalTypeCount;
	private bool on;
	private bool pressed;
	private int index;
	private float timer = 0;
	
	private Player rewiredPlayer;
	private bool moving;
	
	void Start () {
		rewiredPlayer = ReInput.players.GetPlayer(0);
		transform.localEulerAngles = offRot;
		totalTypeCount = System.Enum.GetValues(typeof(GlobalVars.RedZoneType)).Length;
		if (redZoneTypeText)
			redZoneTypeText.text = globalVars.redZoneType.ToString();
	}
	
	void Update () {
		timer += Time.deltaTime * (on ? 1 : -1);
		
		if (timer > 1)
			timer = 1;
		else if (timer < 0)
			timer = 0;
		
		transform.localEulerAngles = Vector3.Lerp(offRot, onRot, timer);
		
		if(timer != 0 && timer != 1){
			moving = false;
		}
		else if (!moving){
			moving = true;
			if (timer == 1)
				onMenuOpen.Invoke();
			else
				onMenuClose.Invoke();
		}
		
		if (rewiredPlayer.GetButtonDown("Menu")) {
			on = !on;
			//player.SendMessage("ToggleMove", !on);
		}
		
		if (allowRedZoneChange) {
			if (on && !pressed) {
				if (rewiredPlayer.GetAxis("Horizontal") != 0) {
					globalVars.redZoneType = (GlobalVars.RedZoneType)(GlobalVars.Mod(((int)globalVars.redZoneType+((Input.GetAxis("Horizontal") > 0) ? 1 : -1 )),totalTypeCount));
					pressed = true;
					redZoneTypeText.text = globalVars.redZoneType.ToString();
				}
			} else if (pressed && rewiredPlayer.GetAxis("Horizontal") == 0) {
				pressed = false;
			}
		}
	}
	
	public void Next (bool left) {
		if (allowRedZoneChange) {
			globalVars.redZoneType = (GlobalVars.RedZoneType)(GlobalVars.Mod(((int)globalVars.redZoneType+((left) ? -1 : 1 )),totalTypeCount));
			redZoneTypeText.text = globalVars.redZoneType.ToString();
		}
	}
}
