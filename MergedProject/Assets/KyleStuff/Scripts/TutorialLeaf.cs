using UnityEngine;
using System.Collections;

public class TutorialLeaf : MonoBehaviour {
	
	public AudioSource sound;
	
	private float volume;
	
	void Start () {
		volume = sound.volume;
	}
	
	public void LowerVolume () {
		sound.volume = volume/3f;
	}
	
	public void TutorialPing () {
		sound.volume = volume;
	}
}
