using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleWhenShown : MonoBehaviour {
	
	public bool allowScale = true;
	public float maxScale;
	public float scaleTime;
	
	private bool scaling;
	private bool up;
	private float timer;
	
	void Start(){
		if(!allowScale){
			allowScale = true;
			Disable();
			allowScale = false;
		}
	}
	
	public void Enable () {
		if (!allowScale)
			return;
		up = true;
		if (!scaling) {
			StartCoroutine(DoScale());
		}
	}
	
	public void Disable () {
		if (!allowScale)
			return;
		up = false;
		if (!scaling) {
			StartCoroutine(DoScale());
		}
	}
	
	IEnumerator DoScale () {
		scaling = true;
		while ((up && timer < 1) || (!up && timer > 0)) {
			timer += Time.deltaTime/scaleTime * (up ? 1 : -1);
			transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(maxScale, maxScale, maxScale), timer);
			yield return null;
		}
		transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(maxScale, maxScale, maxScale), (up ? 1 : 0));
		scaling = false;
	}
	
	public bool IsScaling {
		get { return scaling; }
	}
	
	public bool AllowScaling {
		get { return allowScale; }
		set { allowScale = value; }
	}
}