using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// #-----------------------------------------------------------------------------
// #  Copyright (C) 2015  GaGaGames
// #-----------------------------------------------------------------------------

public class Extrood : MonoBehaviour {

	[Tooltip("Type of modifier to apply to elements.")]
	public SplineElement.SplineElementType 	ExtroodType;

	//[Tooltip("Spline to follow for repeat and extrude.")]
	public SplineInterpolator 	spline;

	// Need to know the last known state of the spline - on change we remove old updater and add new one.
	[System.NonSerialized]
	public SplineInterpolator 	lastSpline = null;

	// Used to ensure the spline has an updater attached - keeps it external to the spline object
	public SplineInterpolator 	_spline
	{
		get{
			return spline;
		}
		set{
			if(lastSpline != null) spline.UpdateChanges -= this.UpdateChanges;
			spline = value;
			if(spline != null)
				spline.UpdateChanges += UpdateChanges;
		}
	}

	[Tooltip("Object to be used for extruding or repeating.")]
	public GameObject 			obj;
	[Tooltip("Only used for Repeat Type Elements")]
	public int 					count = 1;

	[Header("Advanced Settings")]
	[Tooltip("Offset away from the spline - spline relative")]
	public Vector3 				offset;
	[Tooltip("Rotate the object before applying modifiers.")]
	public Vector3				rotate;

	[Tooltip("Randomise the Y rotation for repeat elements - when 0.0f rotations use above rotate.")]
	public float randomYRotation = 0.0f;

	[HideInInspector]
	public List<SplineElement> elements; 	

	[HideInInspector]
	public int parentID = -1;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateChanges()
	{	
		//Debug.Log ("Spline updating");
		if (spline.enableUpdate == true) {
			RebuildMeshes ();
		}
	}

	public void RebuildMeshes()
	{
		if (parentID == -1)
			parentID = this.GetInstanceID ();

		elements = new List<SplineElement> ();

		// Get parent object and iterate children with input data
		Transform [] tforms = spline.mPoints.ToArray();
		spline.Reset ();
		for(int i=0; i< tforms.Length-1; i++)
		{
			GameObject child = tforms[i].gameObject;
			List<SplineElement> roads = new List<SplineElement>(child.GetComponents<SplineElement>());

			// Find elements with this parent
			SplineElement re = roads.Find( e => e.GetOwner() == parentID);
			if(re == null) 
			{
				re = child.AddComponent<SplineElement>();

				re.SetOwner(parentID);
				re.nodeIndex = i;
			}

			re.count = count;
			re.spline = spline;
			re.obj = obj;
			re.etype = ExtroodType;
			re.segment_offset = offset;
			re.segment_rotate = rotate;
			re.rotation_random = randomYRotation;
			if(re.doNotGenerate == false)
				re.GenerateElementButton();
			elements.Add(re);
		}
	}

	public void ClearMeshes()
	{
		if (elements.Count <= 0)
			return;

		for (int i=0; i< elements.Count; i++) {
			//Debug.Log (elements[i]);
			elements [i].DeleteElements ();
		}

		elements.Clear();
	}

	public void ClearElements()
	{
		// Get the spline.. kill the elements and children attached to the spline nodes
		foreach (Transform t in spline.mPoints) {
			// Delete elements first
			SplineElement [] allse = t.gameObject.GetComponents<SplineElement>();
			foreach(SplineElement se in allse)
				se.DeleteElements();
			for(int i=0; i<allse.Length; i++)
				DestroyImmediate(allse[i]);

			foreach(Transform ct in t)
			{
				ct.parent = null;
				DestroyImmediate(ct.gameObject);
			}
		}
	}

	public void OnDestroy()
	{
		ClearElements ();
	}
}
