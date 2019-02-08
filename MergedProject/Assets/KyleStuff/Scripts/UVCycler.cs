using UnityEngine;
using System.Collections;

public class UVCycler : MonoBehaviour {

	public float scrollSpeed = 0.5f;
	Vector2 offVec;
	// Use this for initialization
	void Start () {
		offVec = new Vector2(0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		offVec.x = (Time.time * scrollSpeed);
		offVec.y = offVec.x;
		GetComponent<Renderer>().material.SetTextureOffset("_MainTex",offVec);
	}
}
