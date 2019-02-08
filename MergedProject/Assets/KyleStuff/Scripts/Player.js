import UnityEngine.UI;

var waypoints : GameObject[];
var skyProject : SkyProject;
var toggles : GameObject[];
var crosshair : RectTransform;
var menu : GameObject;
var cam : Camera;

private var rain : boolean;
private var devMode : boolean;
private var togglesState = true;
private var initialized = false;
private var buttonPress : boolean;

function Awake () {
	Cursor.lockState = CursorLockMode.Locked;
}

function Start () {
	Cursor.visible = false;
	
	yield WaitForSeconds (3);
	skyProject.SendMessage("MakeItRain", false);
}

function FixedUpdate() {
	crosshair.position = Input.mousePosition;
}

function Update () {
	if (initialized) {
		if (!buttonPress && Input.GetAxis("Start") != 0) {
			buttonPress = true;
			devMode = !devMode;
		} else if (buttonPress && Input.GetAxis("Start") == 0) {
			buttonPress = false;
		}
		
		if (devMode) {
			menu.SetActive(true);
			Cursor.lockState = CursorLockMode.None;
			gameObject.BroadcastMessage("Toggle", true, SendMessageOptions.DontRequireReceiver);
			if (Input.GetKeyDown(KeyCode.RightControl)) {
				togglesState = !togglesState;
				for (var i = 0; i < toggles.length; i++) {
					toggles[i].SetActive(togglesState);
				}
			}
		} else {
			menu.SetActive(false);
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
		
		if (Input.GetAxis("Fire1")) {
			crosshair.gameObject.SetActive(false);
			var ray : Ray = cam.ScreenPointToRay( Input.mousePosition );
			var hit : RaycastHit;
			Physics.Raycast(ray, hit, 100);
			if (hit.collider && hit.collider.name == "Switch") {
				hit.collider.gameObject.SendMessage("Change");
			}
			crosshair.gameObject.SetActive(true);
		}
	}
}

function RainToggle () {
	rain = !rain;
	print (rain);
	skyProject.SendMessage("MakeItRain", rain);
}

function Initalize () {
	initialized = true;
}