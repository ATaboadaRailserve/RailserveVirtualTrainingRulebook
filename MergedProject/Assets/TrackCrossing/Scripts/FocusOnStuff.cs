using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FocusOnStuff : MonoBehaviour {
	
	public GameObject[] blinkers;
	public float blinkRate = 1f;
	public GameObject[] objectsOn;
	public GameObject[] objectsOff;
	public Transform focusObject;
	
	private IEnumerator blink;
	private bool blunk = true;
	
	void Start () {
		GameObject.FindWithTag("Player").SendMessage("ToggleMove", false);
		GameObject.FindWithTag("ScreenFocus").GetComponent<ScreenFocus>().FocusScreen(focusObject, new Vector3(128,128), new Color(0,0,0,0.75f), true, gameObject);
		blink = Blinking();
		StartCoroutine(blink);
		foreach (GameObject g in objectsOn) {
			g.SetActive(true);
		}
		foreach (GameObject g in objectsOff) {
			g.SetActive(false);
		}
	}
	
	void Defocusing () {
		StopCoroutine(blink);
		foreach (GameObject g in objectsOn) {
			g.SetActive(false);
		}
		foreach (GameObject g in objectsOff) {
			g.SetActive(true);
		}
		GameObject.FindWithTag("Player").SendMessage("ToggleMove", true);
	}
	
	IEnumerator Blinking () {
		float timer = 0;
		while(true) {
			timer += Time.deltaTime/blinkRate;
			if (timer >= 1) {
				timer = 0;
				blunk = !blunk;
				foreach (GameObject g in blinkers) {
					g.SetActive(blunk);
				}
			}
			yield return null;
		}
	}
}
