using UnityEngine;
using System.Collections;
using UnityEditor;

// #-----------------------------------------------------------------------------
// #  Copyright (C) 2015  GaGaGames
// #-----------------------------------------------------------------------------

[CustomEditor(typeof(SplineElement))]
public class SplineElementEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

		SplineElement myScript = (SplineElement)target;
        if(GUILayout.Button("Regenerate Elements"))
        {
            myScript.GenerateElementButton();
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
