using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CommsEmergenciesGameManager))]
[CanEditMultipleObjects]
public class CommsEmergenciesGameManagerInspector : Editor {

    bool confirm;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CommsEmergenciesGameManager myScript = (CommsEmergenciesGameManager)target;
        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("Delete"))
            confirm = true;
        GUILayout.EndHorizontal();

        if (confirm)
        {
            GUILayout.BeginHorizontal("box");
            if (GUILayout.Button("Confirm"))
            {
                confirm = false;
                myScript.HospitalDeleteIndex();
            }
            if (GUILayout.Button("NEVER MIND!"))
            {
                confirm = false;
            }
            GUILayout.EndHorizontal();
        }
    }
}
