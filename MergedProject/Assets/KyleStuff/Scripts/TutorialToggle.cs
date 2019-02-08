using UnityEngine;
using System.Collections;

public class TutorialToggle : MonoBehaviour {
	
	public Renderer[] renderers;
	public bool timed;
	public float timeBetweenEach;
	public float startDelay;
	
	void Ding (bool state) {
		if (timed)
			StartCoroutine(Timer(state));
		else {
			foreach (Renderer r in renderers) {
				r.enabled = state;
			}
		}
	}
	
	IEnumerator Timer (bool state) {
		if (state) {
			yield return new WaitForSeconds(startDelay);
			for (int i = 0; i < renderers.Length; i++) {
				renderers[i].enabled = true;
				yield return new WaitForSeconds(timeBetweenEach);
				renderers[i].enabled = false;
			}
		}
	}
}
