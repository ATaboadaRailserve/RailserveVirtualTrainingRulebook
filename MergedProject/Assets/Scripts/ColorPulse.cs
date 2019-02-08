using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPulse : MonoBehaviour {
	
	public Color upColor;
	public Color downColor;
	
	public float phaseLength;
	public AnimationCurve halfPhaseCurve;
	
	public bool emissive;
	
	private Material material;
	private float timer;
	private float twoPI = Mathf.PI*2f;
	
	void Start () {
		material = gameObject.GetComponent<Renderer>().material;
		material.color = downColor;
	}
	
	void Update () {
		timer += Time.deltaTime/phaseLength;
		while (timer > twoPI)
			timer -= twoPI;
		material.color = Color.Lerp(downColor, upColor, halfPhaseCurve.Evaluate((Mathf.Sin(timer)+1f)/2f));
		if (emissive)
			material.SetColor ("_EmissionColor", Color.Lerp(downColor, upColor, halfPhaseCurve.Evaluate((Mathf.Sin(timer)+1f)/2f)));
	}
}
