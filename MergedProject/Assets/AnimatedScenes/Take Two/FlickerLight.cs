using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;
using UnityEngine;

public class FlickerLight : MonoBehaviour {

	private Material mat;
	private float bloomLow;

	public AnimationScrubber scrubber;
	public Bloom bloom;

	public float startTime, startLength;
	public AnimationCurve fCurve;
	public float bloomStartTime, bloomIntensity;

	void Start () {
		mat = GetComponent<Renderer> ().material;
		if (bloom != null) {
			bloomLow = bloom.bloomIntensity;
		}
	}

	void Update () {
		float t = scrubber.GetTime ();

		if (t < startTime) {
			mat.SetFloat ("_Glow", 0f);
		}

		else if (t >= startTime && t < startTime + startLength) {
			float intense = (fCurve.Evaluate ((t - startTime) / (startLength)))*3f;
			mat.SetFloat ("_Glow", intense);
		}

		else if (t > startTime + startLength) {
			mat.SetFloat ("_Glow", 3f);
		}


		if (bloom != null) {
			if (t < bloomStartTime) {
				bloom.bloomIntensity = bloomLow;
			} else if (t >= bloomStartTime) {
				bloom.bloomIntensity = bloomIntensity;
			}
		}

		
	}


}
