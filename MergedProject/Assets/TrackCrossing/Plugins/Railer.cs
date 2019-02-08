using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Railer : MonoBehaviour {
	
	public bool game;
	
	public GameObject railSection;
	public GameObject endRailSection;
	public float sectionLengthInMeters;
	public int arrayLength;
	public bool doEndSection;
	
	[Range(0,90)]
	public float angleTolerance = 30;
	
	public Vector3 collisionNormal;
	public Vector3 inPosition;
	public Vector3 outPosition;
	
	public List<Transform> segments;
	
	private float radsToDegrees = 180f/Mathf.PI;
	
	private WarningSystem warningSystem;
	
	void Start () {
		if (GameObject.FindWithTag("WarningSystem"))
			warningSystem = GameObject.FindWithTag("WarningSystem").GetComponent<WarningSystem>();
	}
	
	// Generate a length of Rail
	public void Generate () {
		
		// Script giving a piece of its mind to give you peace of mind.  (Let's you know it's working.)
		print("Generating Rail");
		
		#region Kill The Old Rail
		List<Transform> children = new List<Transform>();
		// Get all children pieces
		foreach (Transform t in transform) {
			children.Add(t);
		}
		for (int i = children.Count-1; i >= 0; i--) {
			if (children[i].name.Length >= 4 && children[i].name.Substring(0,4) == "Rail") {
				// Murder them immediately
				DestroyImmediate(children[i].gameObject);
				children.RemoveAt(i);
			}
		}
		BoxCollider box;
		box = gameObject.GetComponent<BoxCollider>();
		if (box) {
			DestroyImmediate(box);
		}
		#endregion
		
		// Village GameObject
		GameObject temp;
		segments = new List<Transform>();
		
		#region Generate The New Rail
		for (int i = 0; i < arrayLength; i++) {
			if (doEndSection && i == 0)
				temp = (GameObject)Instantiate(endRailSection, Vector3.zero, Quaternion.identity);
			else if (doEndSection && i == arrayLength - 1)
				temp = (GameObject)Instantiate(endRailSection, Vector3.zero, Quaternion.identity);
			else {
				temp = (GameObject)Instantiate(railSection, new Vector3(0,-90,0), Quaternion.identity);
				temp.name = "Rail";
				segments.Add(temp.transform);
			}
			temp.transform.parent = transform;
			if (i == arrayLength - 1) {
				if (temp.name == "Rail")
					temp.transform.localEulerAngles = new Vector3(0,-90,0);
				else
					temp.transform.localEulerAngles = new Vector3(-90,180,0);
				temp.transform.localPosition = new Vector3(0,0,i*sectionLengthInMeters - sectionLengthInMeters);
			} else {
				if (temp.name == "Rail")
					temp.transform.localEulerAngles = new Vector3(0,-90,0);
				else
					temp.transform.localEulerAngles = new Vector3(-90,0,0);
				temp.transform.localPosition = new Vector3(0,0,i*sectionLengthInMeters);
			}
		}
		box = gameObject.AddComponent<BoxCollider>();
		box.size = new Vector3(1.6f, 10, arrayLength*sectionLengthInMeters);
		box.center = new Vector3(0, 5, arrayLength*sectionLengthInMeters/2f - sectionLengthInMeters);
		box.isTrigger = true;
		#endregion
		
	}
	
	void OnTriggerEnter (Collider col) {
		if (segments.Count != 0 && col.gameObject.tag == "Player") {
			int segment = 0;
			for (int i = 1; i < segments.Count; i++) {
				if ((col.transform.position - segments[i].position).magnitude < (col.transform.position - segments[segment].position).magnitude)
					segment = i;
			}
			if (GetAngle(segments[segment].right, col.transform.position - segments[segment].position) <= 0)
				collisionNormal = segments[segment].transform.forward;
			else
				collisionNormal = -segments[segment].transform.forward;
			inPosition = col.transform.position;
			
			#region Look Both Ways
			/*
			List<float> angles = col.GetComponent<PlayerAngleTracker>().angles;
			bool left = false;
			bool right = false;
			
			foreach (float f in angles) {
				if (segments[segment].localEulerAngles.y - f < 30 && segments[segment].localEulerAngles.y - f > -30) {
					left = true;
				} else if (segments[segment].localEulerAngles.y - f < 210 && segments[segment].localEulerAngles.y - f > 150) {
					right = true;
				}
			}
			
			if (!(right && left)) {
				warningSystem.Warn(2);
				if (game)
					GameObject.FindWithTag("CrossingRailer").SendMessage("GameOver", warningSystem.warnings[2].name);
			}
			*/
			if (!GameObject.FindWithTag("AngleTracker").GetComponent<PlayerAngleTracker>().goodToCross) {
				warningSystem.Warn(2);
				if (game)
					GameObject.FindWithTag("CrossingRailer").SendMessage("GameOver", warningSystem.warnings[2].name);
			}
			#endregion
		}
	}
	
	public void RemoteTrigger (Collider col) {
		if (segments.Count != 0 && col.gameObject.tag == "Player") {
			print("Remote Triggering");
			int segment = 0;
			for (int i = 1; i < segments.Count; i++) {
				if ((col.transform.position - segments[i].position).magnitude < (col.transform.position - segments[segment].position).magnitude)
					segment = i;
			}
			if (GetAngle(segments[segment].right, col.transform.position - segments[segment].position) <= 0)
				collisionNormal = segments[segment].transform.forward;
			else
				collisionNormal = -segments[segment].transform.forward;
			inPosition = col.transform.position;
			
			#region Look Both Ways
			List<float> angles = GameObject.FindWithTag("AngleTracker").GetComponent<PlayerAngleTracker>().angles;
			bool left = false;
			bool right = false;
			
			foreach (float f in angles) {
				if (segments[segment].localEulerAngles.y - f < 30 && segments[segment].localEulerAngles.y - f > -30) {
					left = true;
				} else if (segments[segment].localEulerAngles.y - f < 210 && segments[segment].localEulerAngles.y - f > 150) {
					right = true;
				}
			}
			
			if (!(right && left)) {
				warningSystem.Warn(2);
				if (game)
					GameObject.FindWithTag("CrossingRailer").SendMessage("GameOver", warningSystem.warnings[2].name);
			}
			#endregion
		}
	}
	
	void OnTriggerExit (Collider col) {
		if (col.gameObject.tag == "Player") {
			outPosition = col.transform.position;
			CrossingReport();
			GameObject.FindWithTag("AngleTracker").GetComponent<PlayerAngleTracker>().ResetAngles();
		}
	}
	
	public void CrossingReport() {
		if (-Vector3.Dot(collisionNormal.normalized, (outPosition - inPosition).normalized) < Mathf.Cos(angleTolerance/radsToDegrees)) {
			warningSystem.Warn(0);
			if (game)
				GameObject.FindWithTag("CrossingRailer").SendMessage("GameOver", warningSystem.warnings[0].name);
		} else if (game) {
			GameObject.FindWithTag("CrossingRailer").SendMessage("ScoreUp", true);
			game = false;
		}
	}
	
	float GetAngle (Vector3 vectorA, Vector3 vectorB) {
		float angle = Vector3.Angle(vectorA, vectorB);
		Vector3 cross = Vector3.Cross(vectorA, vectorB);
		return (cross.y < 0) ? -angle : angle;
	}
}