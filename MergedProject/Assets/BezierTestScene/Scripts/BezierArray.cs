using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Point {
	public Vector3 startWeight;
	public Vector3 point;
	public Vector3 weight;
	public float resolution;
}
	
[ExecuteInEditMode]
public class BezierArray : MonoBehaviour {
	
	public float handleSize = 0.1f;
	public Point[] points;
	public bool closed;
	public bool initialized;
	
	/*
	public Transform obj;
	public float objSpeed;
	[Range (0,1)]
	public float objPos;
	*/
	
	void Update () {
		
		if (!initialized) {
			points = new Point[2];
			points[0].startWeight = new Vector3 (-3,0,-1);
			points[0].point = new Vector3 (-2,0,-1);
			points[0].weight = new Vector3 (-1,0,-1);
			points[0].resolution = 50;
			
			points[1].startWeight = new Vector3 (1,0,1);
			points[1].point = new Vector3 (2,0,1);
			points[1].weight = new Vector3 (3,0,1);
			points[1].resolution = 50;
			initialized = true;
		}
		
		/*
		if (obj && points.Length > 1) {
			if (obj.parent != transform)
				obj.parent = transform;
			float t = objPos;
			
			//Vector3 v1 = -3 * points[0].point + 9 * points[0].weight - 9 * points[1].startWeight + 3 * points[1].point;
			//Vector3 v2 = 6 * points[0].point - 12 * points[0].weight + 6 * points[1].startWeight;
			//Vector3 v3 = -3 * points[0].point + 3 * points[0].weight;
			//t = t + Time.deltaTime*objSpeed / (t * t * v1 + t * v2 + v3).magnitude;
			
			obj.position = transform.position + Mathf.Pow((1 - t), 3) * points[0].point + 3 * (float)Mathf.Pow((1 - t), 2) * t * points[0].weight + 3 * (1 - t) * (float)Mathf.Pow((t), 2) * points[1].startWeight + (float)Mathf.Pow((t), 3) * points[1].point;
		}
		*/
	}
	
	void OnDrawGizmos () {
		for (int i = 0; i < points.Length; i++) {
			Gizmos.color = new Color(1,0.5f,0,1);
			if (i < points.Length - 1) {
				for (float j = 1; j <= points[i].resolution; j++) {
					Vector3 point2 = points[i+1].point;
					Vector3 weight2 = points[i+1].startWeight;
					
					float tPrev = (j-1)/points[i].resolution;
					float t = j/points[i].resolution;
					Vector3 startPoint = transform.position + Mathf.Pow((1 - tPrev), 3) * points[i].point + 3 * (float)Mathf.Pow((1 - tPrev), 2) * tPrev * points[i].weight + 3 * (1 - tPrev) * (float)Mathf.Pow((tPrev), 2) * weight2 + (float)Mathf.Pow((tPrev), 3) * point2;
					Vector3 endPoint = transform.position + Mathf.Pow((1 - t), 3) * points[i].point + 3 * (float)Mathf.Pow((1 - t), 2) * t * points[i].weight + 3 * (1 - t) * (float)Mathf.Pow((t), 2) * weight2 + (float)Mathf.Pow((t), 3) * point2;
					
					Gizmos.DrawLine(startPoint, endPoint);
				}
			} else if (closed) {
				for (float j = 1; j <= points[i].resolution; j++) {
					Vector3 point2 = points[0].point;
					Vector3 weight2 = points[0].startWeight;
					
					float tPrev = (j-1)/points[i].resolution;
					float t = j/points[i].resolution;
					Vector3 startPoint = transform.position + Mathf.Pow((1 - tPrev), 3) * points[i].point + 3 * (float)Mathf.Pow((1 - tPrev), 2) * tPrev * points[i].weight + 3 * (1 - tPrev) * (float)Mathf.Pow((tPrev), 2) * weight2 + (float)Mathf.Pow((tPrev), 3) * point2;
					Vector3 endPoint = transform.position + Mathf.Pow((1 - t), 3) * points[i].point + 3 * (float)Mathf.Pow((1 - t), 2) * t * points[i].weight + 3 * (1 - t) * (float)Mathf.Pow((t), 2) * weight2 + (float)Mathf.Pow((t), 3) * point2;
					
					Gizmos.DrawLine(startPoint, endPoint);
				}
			}
			
			Gizmos.color = Color.green;
			Gizmos.DrawCube(transform.position + points[i].point, new Vector3(handleSize, handleSize, handleSize));
		}
	}
}