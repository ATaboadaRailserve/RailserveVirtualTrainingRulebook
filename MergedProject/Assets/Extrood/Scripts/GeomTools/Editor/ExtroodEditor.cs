using UnityEngine;
using System.Collections;
using UnityEditor;

// #-----------------------------------------------------------------------------
// #  Copyright (C) 2015  GaGaGames
// #-----------------------------------------------------------------------------

[CustomEditor(typeof(Extrood))]
public class ExtroodEditor : Editor
{
	private const float iconSz = 24f;
    public override void OnInspectorGUI()
    {
		Extrood owner = (Extrood)target;
		if (owner.lastSpline == null) {
			owner._spline = owner.spline;
		}

		GUILayout.BeginHorizontal ();
		Texture2D tex = (Texture2D)Resources.Load ("UI/extrood-sml");
		GUILayout.Box (tex, GUILayout.MinWidth(287), GUILayout.MinHeight(57)); 
		
		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal ();
		Texture2D cancel = (Texture2D)Resources.Load ("UI/cancel");
		if(GUILayout.Button(new GUIContent( " Delete All", cancel), GUILayout.Width(90), GUILayout.Height(iconSz)) && owner)
		{
			if(EditorUtility.DisplayDialog("Warning", "The Nodes in the spline will have all meshes and elements deleted!",
			                               "Ok", "Cancel"))
			owner.ClearElements();
		}
		Texture2D triangle = (Texture2D)Resources.Load ("UI/pyramid");
		if(GUILayout.Button(new GUIContent( " Delete Mesh", triangle), GUILayout.Width(100), GUILayout.Height(iconSz)) && owner)
		{
			owner.ClearMeshes();
		}
		Texture2D refresh = (Texture2D)Resources.Load ("UI/refresh");
		if(GUILayout.Button(new GUIContent( " Refresh", refresh), GUILayout.Width(80), GUILayout.Height(iconSz)) && owner)
        {
			owner.RebuildMeshes();
        }
		GUILayout.EndHorizontal ();


		DrawDefaultInspector();

		if(owner.lastSpline != owner._spline)
			owner._spline = owner.spline;
		owner.lastSpline = owner._spline;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
