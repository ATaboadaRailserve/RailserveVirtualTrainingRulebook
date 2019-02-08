using UnityEngine;
using System.Collections;

public class OrthoFollowScript : MonoBehaviour {

	public Camera cameraObj;
	public Transform thingToFollow;
	private Vector3 offset;
	private Vector3 goal;
	public TextToTexture carPlaque;
	[HideInInspector]
	public Camera carNumCamera;

	// Use this for initialization
	void Start () {
		goal = thingToFollow.transform.position + offset;
		carNumCamera = GameObject.FindGameObjectWithTag("CarNumberCamera").GetComponent<Camera>();
	}

	public void SetOffset(float xOffset, float yOffset, float zOffset) {
		offset.x = xOffset;
		offset.y = yOffset;
		offset.z = zOffset;
	}
	public void SetOffset(Vector3 offy) {
		offset = offy;
	}

	public void SetLookAt(Transform t) {
		this.transform.LookAt(t.position);
	}

	// Kyle/Ray - this MUST be a late update to avoid a shaking camera.
	// See LateUpdate effects on camera http://docs.unity3d.com/Manual/ExecutionOrder.html
	void LateUpdate () {

		// Check if the player is clicking on a car
		if (Input.GetMouseButtonDown(0)) {
			Ray ray = cameraObj.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				if (hit.transform.tag == "SmartTanker") {
					// Get the car number
					SmartTankerScript s = hit.transform.gameObject.GetComponent<SmartTankerScript>();
					if(s.plate){
					int carNum = s.plate.number;
					// Set the text on the plane
					carPlaque.SetText(carNum);
					}else{
						carPlaque.SetText(888888);
					}
					

					// Note: I know this seems screwy, but you can't find game objects unless they are enabled.
					// So, just change the depth of the camera.
					carNumCamera.depth = 1;
				}
			}
		}
		goal = thingToFollow.transform.position + offset;

		if (Vector3.Distance(goal, this.transform.position) > 1.0f) {
			this.transform.position = Vector3.Lerp(transform.position, goal, Time.deltaTime/3.0f);
			this.transform.LookAt(thingToFollow);
		}

	}
}
