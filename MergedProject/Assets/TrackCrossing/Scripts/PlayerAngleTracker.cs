using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAngleTracker : MonoBehaviour {
	
	public float interval = 0.5f;
	public int maxAngleCount = 20;
	public float comparisonAngle;
	public float angleLeniency = 70;
	public GameObject icon;
	public Transform player;
	
	[HideInInspector]
	public List<float> angles;
	
	[HideInInspector]
	public bool goodToCross;
	
	[HideInInspector]
	public int leftLoc;
	[HideInInspector]
	public int rightLoc;
	
	private float timer;
	private bool left;
	private bool right;
	private float workerAngle = 0;
	
	void Start () {
		if (player == null)
			player = transform;
		angles = new List<float>();
		for (int i = 0; i < maxAngleCount; i++) {
			angles.Add(0);
		}
	}
	
	void Update () {
		timer += Time.deltaTime;
		if (timer >= interval) {
			timer -= interval;
			angles.Add(player.localEulerAngles.y);
			if (angles.Count > maxAngleCount) {
				angles.RemoveAt(0);
			}
			
			left = false;
			right = false;
			for (int i = 0; i < angles.Count; i++) {
				workerAngle = comparisonAngle - angles[i];
				if (workerAngle < 0f)
					workerAngle += 360f;
				else if (workerAngle > 360f)
					workerAngle -= 360f;
				if (workerAngle < angleLeniency/2f && workerAngle > -angleLeniency/2f) {
					left = true;
					leftLoc = i;
				} else if (workerAngle < (180+angleLeniency/2f) && workerAngle > (180-angleLeniency/2f)) {
					right = true;
					rightLoc = i;
				}
			}
			
			if (right && left) {
				icon.SetActive(true);
				goodToCross = true;
			} else {
				icon.SetActive(false);
				goodToCross = false;
			}
		}
	}
	
	void OnDrawGizmos () {
		if (player) {
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(player.position, player.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * comparisonAngle), 0, Mathf.Sin(Mathf.Deg2Rad * comparisonAngle)));
		}
	}
	
	public void ResetAngles () {
		angles.Clear();
		for (int i = 0; i < maxAngleCount; i++) {
			angles.Add(0);
		}
		right = false;
		left = false;
		goodToCross = false;
	}
}
