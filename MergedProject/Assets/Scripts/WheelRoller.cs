using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRoller : MonoBehaviour {
	
	public enum Axis { X, Y, Z };
	public Axis axis;
	public float diameter;
	
	private Vector3 prevPos;
	private float rotation;
	private bool sign;
	private Transform parent;
	
	void Start () {
		prevPos = transform.position;
		parent = transform.parent;
	}
	
	void LateUpdate () {
		sign = false;
		if (Vector3.Dot((transform.position - prevPos), parent.forward) < 0)
			sign = true;
		rotation = (transform.position - prevPos).magnitude / (Mathf.PI * diameter) * 360f * (sign ? -1 : 1);
		switch (axis) {
			case Axis.X:
				transform.localEulerAngles += new Vector3(rotation, 0, 0);
				break;
			case Axis.Y:
				transform.localEulerAngles += new Vector3(0, rotation, 0);
				break;
			case Axis.Z:
				transform.localEulerAngles += new Vector3(0, 0, rotation);
				break;
		}
		prevPos = transform.position;
	}
}
