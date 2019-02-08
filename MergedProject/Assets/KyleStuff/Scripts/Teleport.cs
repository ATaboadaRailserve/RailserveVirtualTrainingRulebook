using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Teleport : MonoBehaviour {
	
	public RawImage fader;
	public float fadeTime;
	
	public Transform player;
	public Vector3 offSet;
	
	private bool triggered;
	
	void Start () {
		fader = GameObject.Find("Fader").GetComponent<RawImage>();
	}
	
	void OnTriggerEnter (Collider col) {
		if (!triggered)
			StartCoroutine("CrossFade");
	}
	
	IEnumerator CrossFade () {
		triggered = true;
		float time = fadeTime;
		Color color = fader.color;
		
		fader.enabled = true;
		color.a = 0;
		fader.color = color;
		
		while (time > 0) {
			color.a = (fadeTime - time)/fadeTime;
			fader.color = color;
			time -= Time.deltaTime;
			yield return null;
		}
		time = fadeTime;
		
		player.position += offSet;
		
		while (time > 0) {
			color.a = time/fadeTime;
			fader.color = color;
			time -= Time.deltaTime;
			yield return null;
		}
		fader.enabled = false;
		Destroy(gameObject);
	}
	
	void OnDrawGizmos () {
		Gizmos.color = Color.red;
		Gizmos.DrawLine (transform.position, transform.position + offSet);
	}
}
