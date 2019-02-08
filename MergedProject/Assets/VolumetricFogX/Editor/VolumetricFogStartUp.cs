using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class VolumetricFogStartUp {
	static VolumetricFogStartUp()
	{
		////////////creating layers ///////////////////

		SerializedObject tagManager = new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/TagManager.asset") [0]);

		SerializedProperty layerProp = tagManager.FindProperty("layers");
		for(int i = 8; i <= 29; i++)
		{
			SerializedProperty sp0 = layerProp.GetArrayElementAtIndex (i);
			SerializedProperty sp1 = layerProp.GetArrayElementAtIndex (i+1);
			if(sp0.stringValue.Equals("VolumetricFogF") && sp1.stringValue.Equals("VolumetricFogB"))
			{
				break;
			}
			if(sp0 != null && sp1 != null)
			{
				if(sp0.stringValue.Trim().Length == 0 && sp1.stringValue.Trim().Length == 0)
				{
					sp0.stringValue = "VolumetricFogF";
					sp1.stringValue = "VolumetricFogB";
					break;
				}
			}
		}

		////////////creating tag ///////////////////
		bool tagFound = false;
		SerializedProperty tagProp = tagManager.FindProperty("tags");
		for(int i = 0; i < tagProp.arraySize; i++)
		{
			SerializedProperty sp0 = tagProp.GetArrayElementAtIndex (i);
			if(sp0.stringValue.Equals("VolumetricFogArea"))
			{
				tagFound = true;
				break;
			}
		}
		if (!tagFound) 
		{
			tagProp.InsertArrayElementAtIndex(0);
			SerializedProperty sp0 = tagProp.GetArrayElementAtIndex (0);
			sp0.stringValue = "VolumetricFogArea";
		}
		tagManager.ApplyModifiedProperties ();
		Debug.Log ("tag and layers added"); 

		/////////////////////////////////////////////
	}
}