using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LockToPoint : MonoBehaviour {
	
	public Transform target;
	public Vector3 offset;
	
	void Update () {
		if (target)
			transform.position = target.position + offset;
	}
}
