using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookPoints : MonoBehaviour {
	
	[System.Serializable]
	public struct Point {
		public Transform targetTransform;
		public bool useVectorInstead;
		public Vector3 targetVector;
		public float lookTime;
		public AnimationCurve lookCurve;
		public bool lockPlayerDuring;
		public bool lockPlayerAfter;
	}
	
	public PlayerController playerController;
	public Point[] points;
	
	private int index = 0;
	
	public void LookAtPoint (bool moveToNextIndex = false, int indexOverride = -1) {
		if (indexOverride >= 0)
			index = indexOverride;
		if (index > points.Length)
			return;
		if (points[index].useVectorInstead)
			playerController.ForceCamera(points[index].targetVector, points[index].lookCurve, points[index].lookTime, points[index].lockPlayerDuring, points[index].lockPlayerAfter);
		else
			playerController.ForceCamera(points[index].targetTransform, points[index].lookCurve, points[index].lookTime, points[index].lockPlayerDuring, points[index].lockPlayerAfter);
		if (moveToNextIndex)
			index++;
	}
	
	public int Index {
		get { return index; }
		set { index = value; }
	}
}
