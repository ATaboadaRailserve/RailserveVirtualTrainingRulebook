using UnityEngine;
using System.Collections;
using UnityEditor;

// #-----------------------------------------------------------------------------
// #  Copyright (C) 2015  GaGaGames
// #-----------------------------------------------------------------------------

[CustomEditor(typeof(SplineInterpolator))]
public class SplineInterpolatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

		SplineInterpolator spline = (SplineInterpolator)target;
		if(GUILayout.Button("ReBuild Spline") && spline)
        {
			spline.Reset();
        }
    }

	// Use this for initialization
	void Start () {
	
	}
}
