using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicLayer : MonoBehaviour {

	public void SetLayer(string layerName)
	{
		int newLayer = LayerMask.NameToLayer(layerName);
		if(newLayer != -1)
		{
			gameObject.layer = 	newLayer;
		}
	}
}
