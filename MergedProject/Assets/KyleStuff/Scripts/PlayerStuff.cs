using UnityEngine;
using System.Collections;
using Rewired;

public class PlayerStuff : MonoBehaviour {
	
	public GameObject[] waypoints;
	public GameObject skyProject;
	public GameObject[] toggles;
	public RectTransform crosshair;
	public GameObject menu;
	public Camera cam;
	
	private bool rain;
	private bool devMode;
	private bool togglesState;
	private bool initialized;
	private bool buttonPress;
	
	private int playerId = 0;
	private Player _player; // the Rewired player
	
	private Player player {
		get {
			// Get the Rewired Player object for this player. Refresh it as needed so it will get the new reference after a script recompile in the editor.
			if(_player == null) _player = ReInput.players.GetPlayer(playerId);
			return _player;
		}
	}
	
	void Awake () {
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Start () {
		Cursor.visible = false;
		
		StartCoroutine(Rainer());
	}
	
	IEnumerator Rainer () {
		yield return new WaitForSeconds (3);
		skyProject.SendMessage("MakeItRain", false);
	}
	
	void FixedUpdate() {
		crosshair.position = Input.mousePosition;
	}

	void Update () {
		if (initialized) {
			if (!buttonPress && player.GetAxis("Menu") != 0) {
				buttonPress = true;
				devMode = !devMode;
			} else if (buttonPress && player.GetAxis("Menu") == 0) {
				buttonPress = false;
			}
			
			if (devMode) {
				menu.SetActive(true);
				Cursor.lockState = CursorLockMode.None;
				gameObject.BroadcastMessage("Toggle", true, SendMessageOptions.DontRequireReceiver);
				if (Input.GetKeyDown(KeyCode.RightControl)) {
					togglesState = !togglesState;
					for (var i = 0; i < toggles.Length; i++) {
						toggles[i].SetActive(togglesState);
					}
				}
			} else {
				menu.SetActive(false);
				if (Application.loadedLevelName == "Tutorial") 
					Cursor.lockState = CursorLockMode.Locked;
				gameObject.BroadcastMessage("Toggle", false, SendMessageOptions.DontRequireReceiver);
			}
			
			if (Input.GetKeyDown("1")) {
				if (waypoints.Length > 0) {
					transform.position = waypoints[0].transform.position;
				}
			} else if (Input.GetKeyDown("2")) {
				if (waypoints.Length > 1) {
					transform.position = waypoints[1].transform.position;
				}
			} else if (Input.GetKeyDown("3")) {
				if (waypoints.Length > 2) {
					transform.position = waypoints[2].transform.position;
				}
			} else if (Input.GetKeyDown("4")) {
				if (waypoints.Length > 3) {
					transform.position = waypoints[3].transform.position;
				}
			} else if (Input.GetKeyDown("5")) {
				if (waypoints.Length > 4) {
					transform.position = waypoints[4].transform.position;
				}
			} else if (Input.GetKeyDown("6")) {
				if (waypoints.Length > 5) {
					transform.position = waypoints[5].transform.position;
				}
			} else if (Input.GetKeyDown("7")) {
				if (waypoints.Length > 6) {
					transform.position = waypoints[6].transform.position;
				}
			} else if (Input.GetKeyDown("8")) {
				if (waypoints.Length > 7) {
					transform.position = waypoints[7].transform.position;
				}
			} else if (Input.GetKeyDown("9")) {
				if (waypoints.Length > 8) {
					transform.position = waypoints[8].transform.position;
				}
			} else if (Input.GetKeyDown("0")) {
				if (waypoints.Length > 9) {
					transform.position = waypoints[9].transform.position;
				}
			}
			
			if (player.GetAxis("Fire1") > 0) {
				crosshair.gameObject.SetActive(false);
				Ray ray = cam.ScreenPointToRay( Input.mousePosition );
				RaycastHit hit;
				Physics.Raycast(ray, out hit, 100);
				if (hit.collider && hit.collider.name == "Switch") {
					hit.collider.gameObject.SendMessage("Change");
				}
				crosshair.gameObject.SetActive(true);
			}
		}
	}

	void RainToggle () {
		rain = !rain;
		print (rain);
		skyProject.SendMessage("MakeItRain", rain);
	}

	void Initalize () {
		initialized = true;
	}
}