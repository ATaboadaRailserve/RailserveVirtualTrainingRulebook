using UnityEngine;
using System.Collections;

public class WheelFollower : MonoBehaviour {
	
	public Transform frontSet;
	public Transform rearSet;
	
	void LateUpdate () {
		transform.position = frontSet.position + (rearSet.position - frontSet.position) / 2f;
		transform.rotation = Quaternion.LookRotation(frontSet.position - rearSet.position);
	}
	
	public void LockVelocity () {
		frontSet.GetComponent<Wheels>().freeze = true;
		rearSet.GetComponent<Wheels>().freeze = true;
	}
	
	public void SetFriction (float friction) {
		frontSet.GetComponent<Wheels>().friction = friction;
		rearSet.GetComponent<Wheels>().friction = friction;
	}
}
