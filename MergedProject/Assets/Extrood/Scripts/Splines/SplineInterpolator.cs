using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// #-----------------------------------------------------------------------------
// #  Copyright (C) 2015  GaGaGames
// #-----------------------------------------------------------------------------

public enum eEndPointsMode { AUTO, AUTOCLOSED, EXPLICIT }
public enum eWrapMode { ONCE, LOOP }
public delegate void OnEndCallback();

public class SplineInterpolator : MonoBehaviour
{
	//eEndPointsMode mEndPointsMode = eEndPointsMode.AUTO;
    public float stepSize = 5.0f;

	// If the parent node is set then use the child nodes as the node positions of the spline nodes
	public GameObject mParent;
	public List<Transform> mPoints;
	public bool snapToGround = false;

	bool mRotations;
	
	public float maxDistance = 0.0f;
	public float avDistance = 0.0f;
	public int splineNodeCount
	{
		get { return mPoints.Count; }
	}

	public bool enableUpdate = true;
	[HideInInspector]
	public delegate void UpdateChangesDlg();
	[HideInInspector]
	public event UpdateChangesDlg UpdateChanges;

	void Awake()
	{
		Reset();
	}

	public void Reset()
	{
		if (mParent == null && this.transform.childCount > 0)
			mParent = this.gameObject;

		if (mParent) {
			mPoints = new List<Transform> ();
			NodesFromParent ();
		}

		Recalc();
	}

	public void Recalc()
	{
		if (mPoints.Count > 1) {
			maxDistance = iTween.PathLength (mPoints.ToArray ());
			avDistance = maxDistance / (float)mPoints.Count;
		}
	}

	/// <summary>
	/// Calls GameObject.Destroy on all children of transform. and immediately detaches the children
	/// from transform so after this call tranform.childCount is zero.
	/// </summary>
	public static void DestroyChildren(Transform transform) {
		for (int i = transform.childCount - 1; i >= 0; --i) {
			GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
		}
		transform.DetachChildren();
	}

	public float GetLength(int index, int startIndex = 0)
	{
		if (index <= startIndex)
			return 0.0f;
		Transform [] path;
		if (index >= mPoints.Count) 
			path = mPoints.ToArray ();
		else
			path = mPoints.GetRange(startIndex, index+1).ToArray();
		//Debug.Log ("path length: " + path.Length);
		return iTween.PathLength(path);
	}

	public void NodesFromParent()
	{
		if (mParent.transform.childCount == 0)
			return;

		List<Transform> nodes = new List<Transform> ();
		foreach (Transform child in mParent.transform) {
			// Raycast to ground if requested
			if(snapToGround == true)
			{
				Vector3 down = transform.TransformDirection(-Vector3.up);
				RaycastHit hit;
				if(Physics.Raycast(child.transform.position, down, out hit))
				{
					child.transform.position = hit.point;
				}
			}
			child.gameObject.transform.hasChanged = false;
			nodes.Add (child.gameObject.transform);
		}

		for (int i=0; i<nodes.Count; i++) {

			AddPoint (nodes[i].gameObject);
			//AddGizmo(gizmo, node, gizmoParent);
		}
	}

	public void AddPoint(GameObject node)
	{
		node.transform.hasChanged = false;
		mPoints.Add (node.transform);

		Recalc ();
	}

    public void SetPoint(int index, Vector3 p)
    {
        mPoints[index].position = p;
    }

	public Vector3 PointOnPath(float t)
	{
		return iTween.PointOnPath(mPoints.ToArray (), t);
	}

	private bool lastChange = false;
	void OnDrawGizmos()
	{
		if (mPoints == null)
			return;

		if (maxDistance > 0.0f) {

			if (mPoints.Count > 1) {
				iTween.DrawPathGizmos (mPoints.ToArray (), Color.blue);
			}
		}

		bool anyChanges = false;
		foreach (Transform t in mPoints) {
			if (t.hasChanged == true) {
				anyChanges = true;
			}
			t.hasChanged = false;
		}
		if (lastChange == true && anyChanges == false)
			if(UpdateChanges != null) UpdateChanges();
		lastChange = anyChanges;
	}
}