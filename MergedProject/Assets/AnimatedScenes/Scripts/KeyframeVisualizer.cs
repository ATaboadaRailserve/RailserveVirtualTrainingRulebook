using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class KeyframeVisualizer : MonoBehaviour {
	
	[System.Serializable]
	public struct Keyframe {
		public Vector3 position;
		public Vector3 rotation;
		public Vector3 scale;
	}
	
	public enum WrapMode { Clamp, Loop, PingPong }
	
	public AnimationScrubber scrubber;
	public WrapMode wrapMode;
	public Keyframe[] keyframes;
	public Transform anchor;
	public int previewResolution = 10;
	
	private Vector3 workerVec;
	private float x1;
	private float y1;
	private float x2;
	private float y2;
	private Vector3 workerAngle1;
	private Vector3 workerAngle2;
	private float deg2Rad = Mathf.PI/180f;
	
	void OnDrawGizmos () {
		if (keyframes == null || keyframes.Length < 2)
			return;
		if (keyframes.Length == 2) {
			Gizmos.color = Color.red;
			Gizmos.DrawLine(anchor.position + keyframes[0].position, anchor.position + keyframes[1].position);
			Gizmos.color = Color.green;
			x1 = keyframes[0].rotation.x;
			y1 = keyframes[0].rotation.y;
			x2 = keyframes[1].rotation.x;
			y2 = keyframes[1].rotation.y;
			
			workerAngle1.x = Mathf.Sin(x1*deg2Rad)*Mathf.Cos(y1*deg2Rad);
			workerAngle1.y = Mathf.Cos(x1*deg2Rad)*Mathf.Cos(y1*deg2Rad);
			workerAngle1.z = Mathf.Cos(x1*deg2Rad);
			
			workerAngle2.x = Mathf.Sin(x2*deg2Rad)*Mathf.Cos(y2*deg2Rad);
			workerAngle2.y = Mathf.Cos(x2*deg2Rad)*Mathf.Cos(y2*deg2Rad);
			workerAngle2.z = Mathf.Cos(x2*deg2Rad);
			
			for (float i = 0; i <= 1; i += 1f/previewResolution) {
				workerVec = Vector3.Lerp(anchor.position + keyframes[0].position,
											anchor.position + keyframes[1].position,
											i);
				Gizmos.DrawLine(workerVec,
								workerVec + Vector3.Lerp(workerAngle1.normalized, workerAngle2.normalized, i));
			}
		}
		if (anchor) {
			for (int i = 0; i < keyframes.Length-1; i++) {
				Gizmos.color = Color.red;
				//Gizmos.DrawLine(anchor.position + keyframes[i].position);
			}
		} else {
			
		}
	}
	
	public Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;
		return
			oneMinusT * oneMinusT * p0 +
			2f * oneMinusT * t * p1 +
			t * t * p2;
	}
}
