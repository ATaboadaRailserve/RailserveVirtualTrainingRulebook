using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(VolumetricFogX))]
[CanEditMultipleObjects]
public class VolumetricFogXEditor : Editor 
{
	private static bool FogProperties = true;
	private static bool WindEffect = true;

	private SerializedObject m_Object;
	private SerializedProperty m_Position;
	private SerializedProperty m_Rotation;
	private SerializedProperty m_Radius;
	private SerializedProperty m_Height;
	private SerializedProperty m_FallOff;

	private SerializedProperty m_Scale;
	private SerializedProperty m_NoiseStrength;
	private SerializedProperty m_Density;
	private SerializedProperty m_Color;

	private SerializedProperty m_WindDir;
	private SerializedProperty m_WindSpeed;

	private SerializedProperty m_PointLights;
	//private SerializedProperty m_SpotLights;

	private SerializedProperty m_fadeSpeed; //Added by Andrew

	void OnEnable () 
	{
		// Setup the SerializedProperties.
		m_Object = new SerializedObject(target);
		m_Position = m_Object.FindProperty ("Position");
		m_Rotation = m_Object.FindProperty ("Rotation");
		m_Radius = m_Object.FindProperty ("Radius");
		m_Height = m_Object.FindProperty ("Height");
		m_Scale = m_Object.FindProperty ("NoiseScale");
		m_Density = m_Object.FindProperty ("FogDensity");
		m_Color = m_Object.FindProperty ("FogColor");

		m_WindDir = m_Object.FindProperty("WindDirection");
		m_WindSpeed = m_Object.FindProperty("WindSpeed");

		m_PointLights = m_Object.FindProperty ("PointLights");
		//m_SpotLights = m_Object.FindProperty ("SpotLights");

		m_fadeSpeed = m_Object.FindProperty ("fadeSpeed"); //Added by Andrew
	}

	public override void OnInspectorGUI() {
		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		m_Object.Update ();

		EditorGUI.indentLevel++;
		FogProperties = EditorGUILayout.Foldout (FogProperties, "PhysicalProperties");
		if (FogProperties) 
		{
			EditorGUILayout.PropertyField (m_Position);
			EditorGUILayout.PropertyField (m_Rotation);
			EditorGUILayout.Slider (m_Radius, 0.0f, 1.0f);
			EditorGUILayout.PropertyField (m_Height);
			EditorGUILayout.Slider (m_Scale, 0.0f, 1.0f);
			EditorGUILayout.Slider (m_Density, 0.0f, 1.0f);
			EditorGUILayout.PropertyField (m_Color);
		}

		WindEffect = EditorGUILayout.Foldout (WindEffect, "WindProperties");
		if (WindEffect) 
		{
			EditorGUILayout.Slider (m_WindSpeed, 0.0f, 1.0f);
			EditorGUILayout.PropertyField (m_WindDir);
		}

		EditorGUILayout.PropertyField (m_PointLights, new GUIContent ("PointLights"), true);

		//EditorGUILayout.Slider (m_fadeSpeed, 0.0f, .1f);//Added by Andrew
		EditorGUILayout.PropertyField (m_fadeSpeed ,new GUIContent("Time for Fade In"), false);//Added by Andrew

		//EditorGUILayout.PropertyField (m_SpotLights, new GUIContent ("SpotLights"), true);

		// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
		m_Object.ApplyModifiedProperties ();
	}

}
