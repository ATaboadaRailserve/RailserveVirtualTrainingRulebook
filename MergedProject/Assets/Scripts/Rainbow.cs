using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rainbow : MonoBehaviour {

	private Material mat;
	private int r1;
	private int r2;
	private float t;

	static Color[] rainbow =
	{
		Color.red,
		Color.magenta,
		Color.blue,
		Color.cyan,
		Color.green,
		Color.yellow
	};
	
	// Use this for initialization
	void Start () {
		Renderer rend = GetComponent<Renderer>();
		mat = rend.material;
		r1 = Random.Range(0, rainbow.Length);
		r2 = (r1 + 1) % rainbow.Length;
		t = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		t += Time.deltaTime;
		if( t > 1.0f)
		{
			t -= 1.0f;
			r1 = (r1 + 1) % rainbow.Length;
			r2 = (r1 + 1) % rainbow.Length;
		}
		
		Color lerpStart = rainbow[r1];
		Color lerpEnd = rainbow[r2];
		
		mat.color = Color.Lerp(lerpStart, lerpEnd, t);
	}
}
