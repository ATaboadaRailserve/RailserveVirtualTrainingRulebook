using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AnimationScrubber))]
[CanEditMultipleObjects]
public class AnimationScrubberInspector : Editor {
	
	bool confirm;
	bool toAdd;
	
	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		
		AnimationScrubber myScript = (AnimationScrubber)target;
		GUILayout.BeginHorizontal("box");
		if(GUILayout.Button("Add")) {
			confirm = true;
			toAdd = true;
		}
		if(GUILayout.Button("Delete")) {
			confirm = true;
			toAdd = false;
		}
		GUILayout.EndHorizontal();
		
		if (confirm) {
			GUILayout.BeginHorizontal("box");
			if(GUILayout.Button("Confirm")) {
				confirm = false;
				myScript.EditArray(toAdd);
			}
			if(GUILayout.Button("NEVER MIND!")) {
				confirm = false;
			}
			GUILayout.EndHorizontal();
		}
	}
}
