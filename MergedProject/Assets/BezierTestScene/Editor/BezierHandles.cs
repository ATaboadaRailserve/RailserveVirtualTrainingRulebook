using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierArray))]
public class BezierHandles : Editor {
	
	private void OnSceneGUI () {
		BezierArray bezier = target as BezierArray;
		
		Transform bezierTransform = bezier.transform;
		for (int i = 0; i < bezier.points.Length; i++) {
			
			Handles.color = Color.white;
			Handles.DrawLine(bezier.points[i].point + bezierTransform.position, bezier.points[i].weight + bezierTransform.position);
			Handles.DrawLine(bezier.points[i].point + bezierTransform.position, bezier.points[i].startWeight + bezierTransform.position);
			
			EditorGUI.BeginChangeCheck();
			bezier.points[i].weight -= bezier.points[i].point;
			bezier.points[i].startWeight -= bezier.points[i].point;
			bezier.points[i].point += bezierTransform.position;
			
			bezier.points[i].point = Handles.DoPositionHandle(bezier.points[i].point, Quaternion.identity);
			
			bezier.points[i].point -= bezierTransform.position;
			bezier.points[i].weight += bezier.points[i].point;
			bezier.points[i].startWeight += bezier.points[i].point;
			
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(bezier, "Move Point");
				EditorUtility.SetDirty(bezier);
			}
			
			EditorGUI.BeginChangeCheck();
			bezier.points[i].weight += bezierTransform.position;
			bezier.points[i].weight = Handles.DoPositionHandle(bezier.points[i].weight, Quaternion.identity);
			bezier.points[i].weight -= bezierTransform.position;
			
			if (EditorGUI.EndChangeCheck()) {
				float mag = (bezier.points[i].startWeight - bezier.points[i].point).magnitude;
				bezier.points[i].startWeight = Handles.DoPositionHandle(-Vector3.Normalize(bezier.points[i].weight - bezier.points[i].point)*mag + bezier.points[i].point, Quaternion.identity);
				Undo.RecordObject(bezier, "Move Point");
				EditorUtility.SetDirty(bezier);
			}
			
			EditorGUI.BeginChangeCheck();
			bezier.points[i].startWeight += bezierTransform.position;
			bezier.points[i].startWeight = Handles.DoPositionHandle(bezier.points[i].startWeight, Quaternion.identity);
			bezier.points[i].startWeight -= bezierTransform.position;
			
			if (EditorGUI.EndChangeCheck()) {
				float mag = (bezier.points[i].weight - bezier.points[i].point).magnitude;
				bezier.points[i].weight = Handles.DoPositionHandle(-Vector3.Normalize(bezier.points[i].startWeight - bezier.points[i].point)*mag + bezier.points[i].point, Quaternion.identity);
				Undo.RecordObject(bezier, "Move Point");
				EditorUtility.SetDirty(bezier);
			}
			
			Handles.color = Color.yellow;
			Handles.CubeCap(0, bezierTransform.position + bezier.points[i].weight, Quaternion.identity, bezier.handleSize);
			Handles.CubeCap(0, bezierTransform.position + bezier.points[i].startWeight, Quaternion.identity, bezier.handleSize);
		}
	}
}