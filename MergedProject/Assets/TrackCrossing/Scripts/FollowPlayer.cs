using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {
	
	public bool followX;
	public bool followY;
	public bool followZ;
	public Transform target;
	
	private Vector3 startPos;
	
	void Start () {
		startPos = transform.position;
	}
	
	void Update () {
		transform.position = new Vector3(startPos.x * (followX ? 0 : 1) + target.position.x * (followX ? 1 : 0),
										startPos.y * (followY ? 0 : 1) + target.position.y * (followY ? 1 : 0),
										startPos.z * (followZ ? 0 : 1) + target.position.z * (followZ ? 1 : 0));
	}
}
