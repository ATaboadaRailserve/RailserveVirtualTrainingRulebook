using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ParticleScrubber : MonoBehaviour {
	public AnimationScrubber scrubber;
	public float startTime;
	[Header("ie: .5x, 2x, ect")]
	public float time_modifier = 1;
	public uint seed = 1;
	private ParticleSystem ps;
	// Use this for initialization
	void Start () {
		ps = this.GetComponent<ParticleSystem> ();
		ps.Pause ();
		ps.randomSeed = seed;
		ps.Play ();
	}

	// Update is called once per frame
	void Update () {
		float time = Mathf.Clamp(scrubber.GetTime () - startTime, 0, ps.duration*(1f/time_modifier));
		time *= time_modifier;
		ps.Simulate (time, true ,true);
	}
}