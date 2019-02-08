/// <summary>
/// Bezier script v0.1
/// Author: Jeff Chastine
/// Summary: this script takes in 4 transforms and calculates a Bezier curve
///  containing evenly-spaced points.  In addition to being stored in a 
///  line renderer, the points are stored in a points[] array of Vector4s.
///  The position is stored in (x, y, z) and the Y rotation in (w).
///  This script is the basis for TrackFollower.cs
/// 
/// Notes: the execution order is Bezier, TrackFollower, SmartTanker
/// </summary>
using UnityEngine;
using System.Collections;

public class BezierScript : MonoBehaviour {

	// Cubes for handles
	public Transform start;
	public Transform end;
	public Transform handle1;
	public Transform handle2;

	// Parent and child can be either a BezierCurve or a BezierSet
	public Transform parent;
	public Transform child;

	// Resolution is used in ApproximateLengthOfCurve() and CalculatePoints()
	public float resolution = 0.0001f;
	[Range(4,200)]
	public int numPointsInCurve = 10;
	private float distanceBetweenPoints;
	[HideInInspector]
	public float lengthOfCurve;
	


	// Things for the Bezier and Line Renderer
	private Vector3 linePoint;
	private Vector3 lastPoint; 
	private LineRenderer lineRenderer;
	private Vector4[] points;	// A mirror copy of the points for faster access
	public Material lineMaterial;
	public Color startColor = Color.yellow;
	public Color endColor = Color.yellow;

	// Other vars
	private float time = 0;
	private static float PIover2;
	public bool isActive;
	public bool printOut = false;


	public BezierScript GetParentCurve() {
		if (parent.tag == "BezierCurve") {
			return parent.GetComponent<BezierScript>();
		}
		else if (parent.tag == "BezierSet") {
			return parent.GetComponent<BezierSwitcher>().GetActiveBezierScript();
		}
		Debug.Log("BezierScript::GetParentCurve::null parent");
		return null;
	}
	public BezierScript GetChildCurve() {
		if (child.tag == "BezierCurve") {
			return child.GetComponent<BezierScript>();
		}
		else if (child.tag == "BezierSet") {
			return child.GetComponent<BezierSwitcher>().GetActiveBezierScript();
		}
		Debug.Log("BezierScript::GetChildCurve::null child");
		return null;
	}

	// Non-realtime.  Initialized the line renderer and points[] 
	// array with a size
	private void InitLineRenderer() {
		if (printOut) {
			Debug.Log(start.position);
			Debug.Log(end.position);
			Debug.Log (handle1.position);
			Debug.Log(handle2.position);
		}

		lineRenderer = gameObject.AddComponent<LineRenderer>();
		// Not sure why this was necessary, but apparently a lot of people have to
		// manually load the material to make it build correctly.  I tested and this
		// was the only way I could get the build version to visualize the line.
		if (!lineMaterial)
			lineMaterial = new Material ("Shader \"line\" { "+
		                             "Properties { "+
		                             "_Color (\"Color Tint\", Color) = (1,1,0,1) "+
		                             "_MainTex (\"SelfIllum Color (RGB) Alpha (A)\", 2D) = \"white\" "+
		                             "} Category { "+
		                             "Lighting On ZWrite Off Cull Back Blend SrcAlpha OneMinusSrcAlpha Tags {"+
		                             "Queue=Transparent} "+
		                             "SubShader { Material { Emission [_Color] } "+
		                             "Pass { SetTexture [_MainTex] { Combine Texture * Primary, Texture * Primary } } } } }");
		
		lineRenderer.material = lineMaterial; //new Material(Shader.Find("Particles/Additive"));
		lineRenderer.SetColors(startColor, endColor);
		lineRenderer.SetWidth(0.2F, 0.2F);

		// Set the lengthOfCurve variable
		ApproximateLengthOfCurve();

		// Calculate the distance between the points based on length of curve and num points
		distanceBetweenPoints = lengthOfCurve/((float)numPointsInCurve-1.0f);

		// Allocate space for the line renderer and points array
		lineRenderer.SetVertexCount(numPointsInCurve);
		points = new Vector4[numPointsInCurve];

		// Only render the Bezier if it is active
		if (!isActive) {
			lineRenderer.enabled = false;
		}
		if (printOut)
			Debug.Log("LoC is " + lengthOfCurve + "\tNumPoints is " + numPointsInCurve +"\tDBPs is "+distanceBetweenPoints);
	}


	/// <summary>
	/// Non-realtime. Calculate a series of points ALONG the Bezier curve, but with "even"
	/// distances between them.  Used to keep constant velocity for cars.
	/// End result is that points[] is full of evenly-spaced points along
	/// the curve
	/// </summary>
	private void CalculatePoints() {
		int i = 0;
		time = 0.0f;

		// Set first point
		lastPoint = CalculateBezier(time, start.position, handle1.position, handle2.position, end.position);
		lineRenderer.SetPosition(i, lastPoint);	// Set the point in the line renderer
		points[i] = lastPoint;					// Set the point in the array
		// Calculate initial tangent of first point
		points[i].w = Mathf.Atan2(start.position.x-handle1.position.x, start.position.z-handle1.position.z)+PIover2; // Set the starting rotation

		if (printOut) {
			Debug.Log("i:"+i+"\tVec:"+lastPoint+"\tDBPs:"+distanceBetweenPoints);
		}
		i++;

		// Step time by a teeny-tiny amount each time through the loop (non-realtime)
		for (time = resolution; time <= 1.0f; time += resolution) {
			linePoint = CalculateBezier(time, start.position, handle1.position, handle2.position, end.position);
			// If you exceed the distance between points, time to write a new point
			if (Vector3.Distance(lastPoint, linePoint) >= distanceBetweenPoints) {
				if (printOut) {
					Debug.Log("i:"+i+"\tVec:"+linePoint+"\tDBPs:"+distanceBetweenPoints);
				}
				lineRenderer.SetPosition(i, linePoint);	// Set the point in the line renderer
				points[i] = linePoint;					// Set the point in the array
				points[i].w = Mathf.Atan2(lastPoint.x-linePoint.x, lastPoint.z-linePoint.z)+PIover2;
				lastPoint = linePoint;
				i++;
			}
		}
		
		// Dump in the last point in the curve, which should be the end point
		points[i] = end.transform.position;
		lineRenderer.SetPosition(i, end.transform.position);
		
		if(child && child.GetComponent<BezierScript>())
			StartCoroutine("SetParentW");
		else if(child && child.GetComponent<BezierSwitcher>())
			StartCoroutine("SetParentChildW");
		
		if (printOut) {
			Debug.Log("**i:"+i+"\tVec:"+points[i]+"\tDBPs:"+distanceBetweenPoints);
		}
	}
	
	IEnumerator SetParentW () {
		yield return null;
		
		BezierScript temp = child.GetComponent<BezierScript>();
		if (!temp.isActive) {
			int index = temp.points.Length-1;
			points[points.Length-1].w = temp.points[0].w;
		}
	}
	
	IEnumerator SetParentChildW () {
		yield return null;
		
		BezierScript temp = null;
		if (child.GetComponent<BezierSwitcher>().bezier1.GetComponent<BezierScript>().isActive)
			temp = child.GetComponent<BezierSwitcher>().bezier1.GetComponent<BezierScript>();
		else
			temp = child.GetComponent<BezierSwitcher>().bezier2.GetComponent<BezierScript>();
		
		int index = temp.points.Length-1;
		points[points.Length-1].w = temp.points[0].w;
	}

	// Critical, realtime method.  Takes in a distance along the curve and returns
	// a linearly interpolated Vector4 which includes pos (x, y, z) and Y rotation (w)
	public Vector4 FindPosFromDistance (float dist) {
			
		
		// Figure out which pair of index points we are between
		float percentDistance = dist/lengthOfCurve;
		int index1 = (int)(percentDistance*(numPointsInCurve-1));
		int index2 = index1 + 1;
		if (index2 >= numPointsInCurve) {
			index2 = index1;
		}
			
		// Figure out how far between index1 and index2 we are.
		// This value should be < distanceBetweenPoints
		float leftOver = (dist - index1*distanceBetweenPoints);
		float weight = leftOver/distanceBetweenPoints;
		Vector4 returnVal = (((1-weight)*points[index1])+((weight)*points[index2]));
		
		// Return a weighted average
		return returnVal;
		
	}
	

	// Non-realtime
	// Standard Bezier calculator that takes in t={0,1}, points and handles, and returns 
	// where you are along the curve
	Vector3 CalculateBezier (float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
		float tp = 1-t;
		float tp2 = tp*tp;
		float tp3 = tp2*tp;
		float tt = t*t;
		float ttt = tt*t;
		return ((tp3*p0)+(3*tp2*t*p1)+(3*tp*tt*p2)+(ttt*p3));
	}

	// Non-realtime
	// We need to know how long the curve is to do many of the calculations.  This is called only
	// once per curve.  It updates the variable lengthOfCurve.  Resolution should be small (0.0001)
	private void ApproximateLengthOfCurve() {
		Vector3 A, B;
		A = CalculateBezier(time, start.position, handle1.position, handle2.position, end.position);
		for (time = 0.0f; time <= 1.0f; time += resolution) {
			B = CalculateBezier(time, start.position, handle1.position, handle2.position, end.position);
			lengthOfCurve += Vector3.Distance(A, B);
			A = B;
		}
	}

	// Use this for initialization
	void Awake () {
		
		PIover2 = Mathf.PI/2.0f;
		HideHandles();
		InitLineRenderer();
		CalculatePoints();
	}

	#region SHOW_HIDE_FUNCTIONS
	// Activate and show the curve - when switching
	public void Show() {
		isActive = true;
		lineRenderer.enabled = true;
	}

	// Deactivate and hide the curve - when switching
	public void Hide() {
		isActive = false;
		lineRenderer.enabled = false;
	}

	// Hide all the little cubes that are visible while editing.  Called from Start()
	private void HideHandles() {
		start.GetComponent<Renderer>().enabled = false;
		end.GetComponent<Renderer>().enabled = false;
		handle1.GetComponent<Renderer>().enabled = false;
		handle2.GetComponent<Renderer>().enabled = false;
	}
	#endregion SHOW_HIDE_FUNCTIONS

	#region UNUSED_CODE

	/*public Vector3 FindPosFromTime (float t) {
		return CalculateBezier(t, start.position, handle1.position, handle2.position, end.position);
	}*/

	// Calculating all points on the Bezier when t={0,1}
	/*private void CalculatePoints() {
		int i = 0;
		lastPoint = CalculateBezier(time, start.position, handle1.position, handle2.position, end.position);
		for (time = 0.0f; time <= 1.0f; time += resolution) {
			linePoint = CalculateBezier(time, start.position, handle1.position, handle2.position, end.position);
			lineRenderer.SetPosition(i, linePoint);
			lastPoint = linePoint;
			i++;
		}
	}*/
	#endregion
}
