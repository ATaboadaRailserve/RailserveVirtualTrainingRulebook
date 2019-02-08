using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureTiler : MonoBehaviour {
	
	[System.Serializable]
	public struct Map {
		public string name;
		public Vector2 tileRate;
		
		[HideInInspector]
		public Vector2 tiling;
	}
	
	public Material material;
	public Map[] maps;
	
	void Start () {
		Material tempMat = Instantiate(material) as Material;
		material = tempMat;
		GetComponent<Renderer>().material = material;
		for (int m = 0; m < maps.Length; m++) {
			maps[m].tiling = material.GetTextureOffset(maps[m].name);
		}
	}
	
	void Update () {
		for (int m = 0; m < maps.Length; m++) {
			maps[m].tiling += maps[m].tileRate;
			material.SetTextureOffset(maps[m].name, maps[m].tiling);
		}
	}
}
