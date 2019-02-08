using UnityEngine;
using System.Collections;

public class Marking : MonoBehaviour {
	
	public float fadeTime = 1.0f;
	public GameObject[] markings;
	
	private float time;
	private Color color;
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime/fadeTime;
		if (time >= Mathf.PI*2) {
			time -= Mathf.PI*2;
		}
		for (int i = 0 ; i < markings.Length; i++) {
			color = markings[i].GetComponent<Renderer>().material.color;
			color.a = Mathf.Sin(time)/4f+0.5f;
			markings[i].GetComponent<Renderer>().material.color = color;
		}
	}
}
