using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAngleAlign : MonoBehaviour {
	
	public enum Axis { X, Y, Z }
	public Axis axisToCheck;
	
	public Vector3 heading;
	[Range(0,1)]
	public float tolerance;
	
	[Header("Aligned")]
	public InteractionHandler.InvokableState onAlign;
	public InteractionHandler.InvokableState whileAlign;
	
	[Header("Unaligned")]
	public InteractionHandler.InvokableState onUnalign;
	public InteractionHandler.InvokableState whileUnaligned;
	
	[Header("Debug")]
	public bool printMagnitude;
	
	private bool alignment;
	private bool prevAlignment;
	private Vector3 normalizedHeading;
	
	void Start () {
		normalizedHeading = heading.normalized;
	}
	
	void Update () {
		switch (axisToCheck) {
			case Axis.X:
				if (Vector3.Dot(transform.right, normalizedHeading) >= tolerance) {
					if (alignment != prevAlignment)
						onAlign.Invoke();
					whileAlign.Invoke();
				} else {
					if (alignment != prevAlignment)
						onUnalign.Invoke();
					whileUnaligned.Invoke();
				}
				if (printMagnitude) {
					print(gameObject.name + "'s Dot Product = " + Vector3.Dot(transform.right, normalizedHeading) + " | Forward" + transform.right);
				}
				break;
			case Axis.Y:
				if (Vector3.Dot(transform.up, normalizedHeading) >= tolerance) {
					if (alignment != prevAlignment)
						onAlign.Invoke();
					whileAlign.Invoke();
				} else {
					if (alignment != prevAlignment)
						onUnalign.Invoke();
					whileUnaligned.Invoke();
				}
				if (printMagnitude) {
					print(gameObject.name + "'s Dot Product = " + Vector3.Dot(transform.up, normalizedHeading) + " | Forward" + transform.up);
				}
				break;
			case Axis.Z:
				if (Vector3.Dot(transform.forward, normalizedHeading) >= tolerance) {
					if (alignment != prevAlignment)
						onAlign.Invoke();
					whileAlign.Invoke();
				} else {
					if (alignment != prevAlignment)
						onUnalign.Invoke();
					whileUnaligned.Invoke();
				}
				if (printMagnitude) {
					print(gameObject.name + "'s Dot Product = " + Vector3.Dot(transform.forward, normalizedHeading) + " | Forward" + transform.forward);
				}
				break;
		}
		
		prevAlignment = alignment;
	}
	
	void OnDrawGizmos () {
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position, transform.position + heading.normalized);
	}
}