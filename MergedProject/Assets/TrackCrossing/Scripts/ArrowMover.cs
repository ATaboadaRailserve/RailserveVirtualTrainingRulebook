using UnityEngine;
using System.Collections;

public class ArrowMover : MonoBehaviour {
	
	public Vector3 target;
	public float timeToMove;
	public bool pingPong;
	[Range(0,1)]
	public float timer = 0;
	public Gradient color;
	public Gradient emission;
	
	private Vector3 startPos;
	private bool initialized;
	
	void Start () {
		startPos = transform.localPosition;
	}
	
	void Update () {
		transform.localPosition = Vector3.Lerp(startPos, target + startPos, timer);
		GetComponent<Renderer>().material.color = color.Evaluate(timer);
		GetComponent<Renderer>().material.SetColor ("_EmissionColor", emission.Evaluate(timer));
		timer += Time.deltaTime / timeToMove;
		if (timer > 1) {
			if (pingPong) {
				timer -= (timer-1f)*2f;
				timeToMove = -timeToMove;
			} else
				timer -= 1f;
		} else if (timer < 0) {
			timer -= timer*2f;
			timeToMove = -timeToMove;
		}
	}
	
	void OnEnable () {
		if (initialized)
			return;
		startPos = transform.localPosition;
		transform.localPosition = Vector3.Lerp(startPos, target + startPos, 0);
		GetComponent<Renderer>().material.color = color.Evaluate(0);
		GetComponent<Renderer>().material.SetColor ("_EmissionColor", emission.Evaluate(timer));
		initialized = true;
	}
}