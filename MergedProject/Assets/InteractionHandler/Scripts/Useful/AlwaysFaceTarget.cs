using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysFaceTarget : MonoBehaviour {
	
	public Transform target;
	public bool lockX, lockY, lockZ;

	private Vector3 tempAngle;
	
	void Start()
	{
		if (lockX || lockY || lockZ)
			tempAngle = transform.eulerAngles;
		if (target == null)
			Destroy(this);
	}
	
	void LateUpdate()
	{
		transform.forward = (transform.position - target.position).normalized;
		
		if (lockX) {
			transform.eulerAngles = new Vector3 (tempAngle.x, transform.eulerAngles.y, transform.eulerAngles.z);
		}
		if (lockY) {
			transform.eulerAngles = new Vector3 (transform.eulerAngles.x, tempAngle.y, transform.eulerAngles.z);
		}
		if (lockZ) {
			transform.eulerAngles = new Vector3 (transform.eulerAngles.x, transform.eulerAngles.y, tempAngle.z);
		}
	}
}