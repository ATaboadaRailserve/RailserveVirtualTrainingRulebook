using UnityEngine;
using System.Collections;

public class RevealObject : MonoBehaviour {
	
	public bool reveal;
	public Vector2 timeFrame;
	public Revealer revealer;
	
	void OnEnable () {
		revealer.DoReveal(timeFrame, reveal);
	}
}
