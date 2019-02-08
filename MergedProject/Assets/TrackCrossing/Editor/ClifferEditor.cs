using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Cliffer)), CanEditMultipleObjects]
class ClifferEditor : Editor {
	
	public override void OnInspectorGUI() {
		Cliffer cliffer = (Cliffer)target;
		DrawDefaultInspector();
		if (GUILayout.Button("Generate Island")) {
			cliffer.Generate();
		}
	}
}