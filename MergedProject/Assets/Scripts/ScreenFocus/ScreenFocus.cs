using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFocus : MonoBehaviour {
	
	public Image focus;
	public Image background;
	public GameObject button;
	
	public float timeToFade = 0.5f;
	
	private float timer;
	private bool fading;
	private IEnumerator coroutine;
	private GameObject currentCallback;
	private Transform focusParent;
	
	void Start () {
		background.material.color = Color.clear;
		focusParent = focus.transform.parent;
	}
	
	public void FocusScreen (Vector3 focalPoint, Vector2 focusSize, Color fadeColor, bool displayButton, GameObject callback = null) {
		if (callback)
			currentCallback = callback;
		if (displayButton)
			button.SetActive(true);
		
		focus.rectTransform.position = focalPoint;
		focus.rectTransform.sizeDelta = focusSize;
		
		if (fading)
			StopCoroutine(coroutine);
			
		coroutine = Fade(fadeColor);
		StartCoroutine(coroutine);
	}
	
	public void FocusScreen (Transform focalPoint, Vector2 focusSize, Color fadeColor, bool displayButton, GameObject callback = null) {
		if (callback)
			currentCallback = callback;
		if (displayButton)
			button.SetActive(true);
		
		//focus.rectTransform.position = focalPoint.position;
		focus.transform.parent = focalPoint;
		focus.rectTransform.localPosition = Vector3.zero;
		focus.transform.parent = focusParent;
		focus.rectTransform.sizeDelta = focusSize;
		
		if (fading)
			StopCoroutine(coroutine);
			
		coroutine = Fade(fadeColor);
		StartCoroutine(coroutine);
	}
	
	public void DefocusScreen () {
		if (currentCallback)
			currentCallback.SendMessage("Defocusing");
		currentCallback = null;
		button.SetActive(false);
		
		if (fading)
			StopCoroutine(coroutine);
			
		coroutine = Fade(Color.clear);
		StartCoroutine(coroutine);
	}
	
	IEnumerator Fade (Color toColor) {
		fading = true;
		timer = 0;
		Color startColor = background.material.color;
		while (timer < 1) {
			timer += Time.deltaTime / timeToFade;
			background.material.color = Color.Lerp(startColor, toColor, timer);
			yield return null;
		}
		fading = false;
	}
}
