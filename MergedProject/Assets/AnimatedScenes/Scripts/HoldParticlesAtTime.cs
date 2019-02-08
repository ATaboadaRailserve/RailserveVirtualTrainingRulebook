using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldParticlesAtTime : MonoBehaviour {
	
	[System.Serializable]
	public struct Particles {
		public ParticleSystem system;
		public float time;
	}
	
	public Particles[] particleSystems;
	
	void Start () {
		for (int i = 0; i < particleSystems.Length; i++) {
			particleSystems[i].system.Pause();
			particleSystems[i].system.Simulate(particleSystems[i].time);
		}
	}
}
