using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PatricleThingy : MonoBehaviour {
	private ParticleSystem ps;
	private float time;

	public Slider slide;
	// Use this for initialization
	void Start () {
		ps = this.GetComponent<ParticleSystem> ();
		ps.randomSeed = 1;
	}
	
	// Update is called once per frame
	void Update () {
		time = slide.value;
		ps.Simulate (time, true, true);
	}
}
