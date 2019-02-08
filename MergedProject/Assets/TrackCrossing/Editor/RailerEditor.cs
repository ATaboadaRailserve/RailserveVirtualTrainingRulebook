using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Railer)), CanEditMultipleObjects]
class RailerEditor : Editor {
	
	public override void OnInspectorGUI() {
		Railer railer = (Railer)target;
		DrawDefaultInspector();
		if (GUILayout.Button("Generate Rails")) {
			railer.Generate();
		}
	}
}