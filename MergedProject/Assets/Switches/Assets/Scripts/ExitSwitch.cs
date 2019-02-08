using UnityEngine;
using System.Collections;

public class ExitSwitch : MonoBehaviour {
	GrabandRotate garScript;
	// Use this for initialization
	void Start () {
		garScript = this.transform.parent.FindChild("Lever_1").FindChild("Lever_0").GetComponent<GrabandRotate>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerExit(Collider collisionInfo)
	{
		garScript.inSwitchRange = false;
	}

	void OnTriggerEnter(Collider collisionInfo)
	{
		garScript.inSwitchRange = true;
	}
}
