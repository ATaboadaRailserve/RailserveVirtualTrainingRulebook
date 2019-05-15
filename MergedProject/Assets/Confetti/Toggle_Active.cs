using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle_Active : MonoBehaviour {

	public List<GameObject> gameObjectList = new List<GameObject>();
	
	public void ToggleGameObjects()
	{
		foreach(GameObject g in gameObjectList)
		{
			g.SetActive(!g.activeSelf);
		}
	}
}
