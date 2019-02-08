using UnityEngine;
using System.Collections;

public class RenderOrder : MonoBehaviour {
	
	public int renderQueue = 900;
	
	void Start () {
		GetComponent<Renderer>().material.renderQueue = renderQueue;
	}
}
