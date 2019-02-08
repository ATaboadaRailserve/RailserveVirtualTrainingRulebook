using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(TrackLayout))]
[CanEditMultipleObjects]
public class TrackLayoutInspector : Editor {
	
	bool viewLine;
	List<int> choiceIndex;
	List<int> prevChoiceIndex;
	List<bool> flipped;
	List<bool> prevflipped;
	
	public void OnEnable () {
		// Get the TrackLayout Script
		TrackLayout targetScript = (TrackLayout)target;
		
		// Create the previous choice holder when the script is first selected.
		prevChoiceIndex = new List<int>();
		prevflipped = new List<bool>();
		for (int i = 0; i < targetScript.lines[targetScript.activeLine].pieces.Count; i++) {
			prevChoiceIndex.Add(-1);
			prevflipped.Add(false);
		}
		prevChoiceIndex.Add(targetScript.segments.Length);
		prevflipped.Add(false);
	}
	
	public override void OnInspectorGUI() {
		
		serializedObject.Update ();
		
		// Don't bother drawing the default stuff since the custom editor is set up
		DrawDefaultInspector();
		
		// Get the TrackLayout Script
		TrackLayout targetScript = (TrackLayout)target;
		
		// Choose which line to edit
		EditorGUILayout.BeginHorizontal("Lines", GUILayout.MaxHeight(50));
		if (targetScript.activeLine > 0) {
			if (GUILayout.Button("Previous Line", GUILayout.MaxWidth(100))) {
				targetScript.Next(false);
				return;
			}
		}
		GUILayout.Label("Lines");
		if (targetScript.activeLine < targetScript.lines.Count-1) {
			if (GUILayout.Button("Next Line", GUILayout.MaxWidth(100))) {
				targetScript.Next(true);
				return;
			}
		}
		EditorGUILayout.EndHorizontal();
		
		// Each "Frame" iteration, renew the list of track pieces and their orientation
		choiceIndex = new List<int>();
		flipped = new List<bool>();
		
		// Set the lists to whatever the current pieces are
		for (int i = 0; i < targetScript.lines[targetScript.activeLine].pieces.Count; i++) {
			choiceIndex.Add(Mathf.Abs(targetScript.lines[targetScript.activeLine].pieces[i]));
			if (targetScript.lines[targetScript.activeLine].pieces[i] < 0)
				flipped.Add(true);
			else
				flipped.Add(false);
		}
		choiceIndex.Add(targetScript.segments.Length);
		flipped.Add(false);
		
		// Get the possible track piece types from the segments list
		string[] choices = new string[targetScript.segments.Length + 1];
		for (int i = 0; i < targetScript.segments.Length; i++) {
			choices[i] = targetScript.segments[i].pieceName;
		}
		choices[choices.Length-1] = "None"; // Plus 1 for adding/removing track
		
		// Ability to collapse the line for easy access to rest of UI
		viewLine = EditorGUILayout.Foldout(viewLine, targetScript.lines[targetScript.activeLine].name);
		if (viewLine) {
			// List out a... uh... list... of pieces in this line.
			for (int i = 0; i <= targetScript.lines[targetScript.activeLine].pieces.Count; i++) {
				if (i < choiceIndex.Count) {
					
					// Individual piece UI (Type and whether or not it's flipped)
					EditorGUILayout.BeginHorizontal("Piece", GUILayout.MaxHeight(50));
					choiceIndex[i] = EditorGUILayout.Popup(choiceIndex[i], choices);
					if (i < targetScript.lines[targetScript.activeLine].pieces.Count) {
						EditorGUIUtility.labelWidth = 40;
						if (targetScript.lines[targetScript.activeLine].pieces[i] > 2 || targetScript.lines[targetScript.activeLine].pieces[i] < -2) {
							flipped[i] = EditorGUILayout.Toggle("   Flip", flipped[i]);
						}
						if (targetScript.lines[targetScript.activeLine].pieces[i] == 0) {
							targetScript.lines[targetScript.activeLine].count[i] = EditorGUILayout.IntField("Count:", targetScript.lines[targetScript.activeLine].count[i]);
							
							if (targetScript.lines[targetScript.activeLine].prevCount[i] != targetScript.lines[targetScript.activeLine].count[i]) {
								targetScript.lines[targetScript.activeLine].ChangeCount(i, targetScript.lines[targetScript.activeLine].count[i]);
							}
							
							targetScript.lines[targetScript.activeLine].prevCount[i] = targetScript.lines[targetScript.activeLine].count[i];
						}
						EditorGUIUtility.labelWidth = 0;
					}
					EditorGUILayout.EndHorizontal();
					
					// If a piece is changed, update it
					if ((i < prevChoiceIndex.Count && prevChoiceIndex[i] != choiceIndex[i]) || (i < prevflipped.Count && prevflipped[i] != flipped[i])) {
						if (i < targetScript.lines[targetScript.activeLine].pieces.Count && choiceIndex[i] == choices.Length-1) {
							targetScript.lines[targetScript.activeLine].pieces.RemoveAt(i);
							targetScript.lines[targetScript.activeLine].count.RemoveAt(i);
							targetScript.lines[targetScript.activeLine].prevCount.RemoveAt(i);
							EditorUtility.SetDirty(targetScript);
							targetScript.Generate();
						} else if (choiceIndex[i] != choices.Length-1) {
							if (i < targetScript.lines[targetScript.activeLine].pieces.Count && targetScript.lines[targetScript.activeLine].pieces[i] != choiceIndex[i] * (flipped[i] ? -1 : 1)) {
								if (flipped[i])
									targetScript.lines[targetScript.activeLine].pieces[i] = -choiceIndex[i];
								else
									targetScript.lines[targetScript.activeLine].pieces[i] = choiceIndex[i];
							} else if (i == targetScript.lines[targetScript.activeLine].pieces.Count) {
								if (flipped[i]) {
									targetScript.lines[targetScript.activeLine].pieces.Add(-choiceIndex[i]);
									targetScript.lines[targetScript.activeLine].count.Add(1);
									targetScript.lines[targetScript.activeLine].prevCount.Add(1);
								} else {
									targetScript.lines[targetScript.activeLine].pieces.Add(choiceIndex[i]);
									targetScript.lines[targetScript.activeLine].count.Add(1);
									targetScript.lines[targetScript.activeLine].prevCount.Add(1);
								}
							}
							EditorUtility.SetDirty(targetScript);
							targetScript.Generate();
							Undo.RecordObject(target, "Updated track pieces");
						}
					}
				}
			}
		}
		
		prevChoiceIndex.Clear();
		prevflipped.Clear();
		for (int i = 0; i < choiceIndex.Count; i++) {
			prevChoiceIndex.Add(choiceIndex[i]);
			prevflipped.Add(flipped[i]);
		}
		
		// "Nuke all" button
		if(GUILayout.Button("Reset")) {
			targetScript.Reset();
			Undo.RecordObject(target, "Updated track pieces");
			
			// Create the previous choice holder when the script is first selected.
			prevChoiceIndex = new List<int>();
			prevflipped = new List<bool>();
			for (int i = 0; i < targetScript.lines[targetScript.activeLine].pieces.Count; i++) {
				prevChoiceIndex.Add(-1);
				prevflipped.Add(false);
			}
			prevChoiceIndex.Add(targetScript.segments.Length);
			prevflipped.Add(false);
		}
	}
}