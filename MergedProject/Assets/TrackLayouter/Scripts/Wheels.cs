using UnityEngine;
using System.Collections;

public class Wheels : MonoBehaviour {
	
	public BezierSetup currentBezierHandler;
	public SplineInterpolator currentSpline;
	public float t = 0;
	public bool diverging;
	public bool inverted;
	public float currentLength;
	public float currentT;
	public float marginOfError = 0.01f;
	
	public ParticleSystem particles;
	
	[Header("Wheel Object Setup")]
	public float distance;
	public Transform otherSet;
	public WheelFollower parentFollower;
	
	public float acceleration;
	public float maxSpeed;
	public float friction;
	
	public bool keyboardControl;
	[HideInInspector]
	public bool accelerating;
	[HideInInspector]
	public bool gameend = false;
	
	[HideInInspector]
	public bool freeze;
	private bool derailing;
	public float velocity;
	private bool initialized;
	
	void Start () {
		StartCoroutine(Initialize());
	}
	
	IEnumerator Initialize () {
		yield return null;
		currentSpline = currentBezierHandler.GetSpline();
		currentLength = currentBezierHandler.currentDistance;
		currentT = currentBezierHandler.currentT;
		iTween.MoveUpdate(gameObject, iTween.Hash ("position", currentSpline.PointOnPath(t), "orienttopath", true, "looktime", 0, "time", 0));
		t = distance/currentLength;
		initialized = true;
	}

	void Update () {
		if (!initialized || freeze)
			return;
		
		if (velocity >= friction)
			velocity -= friction;
		else if (velocity <= -friction)
			velocity += friction;
		else
			velocity = 0;
		
		if ((currentLength > 0 && keyboardControl))
			velocity += Input.GetAxis ("Vertical") * acceleration * (inverted ? -1 : 1);
		else if (accelerating)
			velocity += maxSpeed;
		
		if (velocity > maxSpeed)
			velocity = maxSpeed;
		else if (velocity < -maxSpeed)
			velocity = -maxSpeed;
		
		float magnitude = 0;
		int whileTimer = 0;
		if (velocity > marginOfError) {
			while (magnitude < velocity * Time.deltaTime) {
				magnitude = (currentSpline.PointOnPath(Mathf.Clamp(t + marginOfError, 0, 1)) - transform.position).magnitude;
				//print("Magnitude: " + magnitude + " | Velocity * Time.deltaTime: " + velocity * Time.deltaTime);
				t = t + marginOfError;
				whileTimer++;
				if (whileTimer > 100)
					break;
			}
		} else if (velocity < -marginOfError) {
			while (magnitude < -velocity * Time.deltaTime) {
				magnitude = (currentSpline.PointOnPath(Mathf.Clamp(t - marginOfError, 0, 1)) - transform.position).magnitude;
				t = t - marginOfError;
				whileTimer++;
				if (whileTimer > 100)
					break;
			}
		}
		if (gameObject.tag == "BackSet" && (transform.position - otherSet.position).magnitude < distance) {
			if (velocity > marginOfError) {
				magnitude = (transform.position - otherSet.position).magnitude;
				while (magnitude < velocity * Time.deltaTime) {
					t = t - marginOfError;
					magnitude = (currentSpline.PointOnPath(Mathf.Clamp(t - marginOfError, 0, 1)) - otherSet.position).magnitude;
					whileTimer++;
					if (whileTimer > 100)
						break;
				}
			} else if (velocity < -marginOfError) {
				while (magnitude < -velocity * Time.deltaTime) {
					t = t - marginOfError;
					magnitude = (currentSpline.PointOnPath(Mathf.Clamp(t + marginOfError, 0, 1)) - otherSet.position).magnitude;
					whileTimer++;
					if (whileTimer > 100)
						break;
				}
			}
		}
		
		if (t > currentT) {
			if (derailing) {
				parentFollower.LockVelocity();
				gameend = true;//SWITCH GAME (ANDREW)
			} else if (currentBezierHandler.nextBezier || (currentBezierHandler.divergeNextBezier && currentBezierHandler.diverging)) {
				BezierSetup.BezierPacket packet = currentBezierHandler.GetBezier(false);
				currentBezierHandler = packet.controller;
				currentSpline = packet.spline;
				currentLength = currentBezierHandler.currentDistance;
				currentT = currentBezierHandler.currentT;
				derailing = packet.derailing;
				if (derailing) {
					parentFollower.SetFriction(3f);
					if (particles)
						particles.Play();
					CameraShake[] shakers = FindObjectsOfType(typeof(CameraShake)) as CameraShake[];
					for (int i = 0; i < shakers.Length; i++) {
						shakers[i].Shake(7,new Vector3(0.1f,0.5f,0), 50);
					}
				}
				if (currentBezierHandler.invertPositionOnNextBezierChange) {
					t = currentT;
					inverted = true;
				} else {
					t = 0f;
					inverted = false;
				}
			} else {
				t = currentT;
			}
		} else if (t < 0f) {
			if (currentBezierHandler.previousBezier) {
				BezierSetup.BezierPacket packet = currentBezierHandler.GetBezier(true);
				currentBezierHandler = packet.controller;
				currentSpline = packet.spline;
				currentLength = currentBezierHandler.currentDistance;
				currentT = currentBezierHandler.currentT;
				if (currentBezierHandler.invertPositionOnPreviousBezierChange) {
					t = 0f;
					inverted = true;
				} else {
					t = currentT;
					inverted = false;
				}
			} else {
				t = 0;
			}
		}
		
		t = Mathf.Clamp(t, 0, 1);
		iTween.MoveUpdate(gameObject, iTween.Hash ("position", currentSpline.PointOnPath(t), "orienttopath", true, "looktime", 0, "time", 0));
	}
}
