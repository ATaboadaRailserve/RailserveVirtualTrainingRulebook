using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour {
	
	public Image image;
	public Color fadedColor;
	public bool startBlack = true;
	public bool fadeOnStart = true;
	
	public InteractionHandler.InvokableState onFadeToClear;
	public InteractionHandler.InvokableState onIsClear;
	
	public InteractionHandler.InvokableState onFadeToBlack;
	public InteractionHandler.InvokableState onIsBlack;
	
	private bool toBlack;
	private float timer;
	private IEnumerator fade;
	
	void Start () {
		if (startBlack) {
			timer = 1;
			image.color = fadedColor;
		} else {
			timer = 0;
			image.color = Color.clear;
		}
		
		if (fadeOnStart) {
			if (startBlack)
				FadeToClear();
			else
				FadeToBlack();
		}
	}
	
	public void FadeToBlack (float time = 1) {
		onFadeToBlack.Invoke();
		toBlack = true;
		if (fade != null) {
			StopCoroutine(fade);
		}
		fade = Fade(time);
		StartCoroutine(fade);
	}
	
	public void FadeToClear (float time = 1) {
		onFadeToClear.Invoke();
		toBlack = false;
		if (fade != null) {
			StopCoroutine(fade);
		}
		fade = Fade(time);
		StartCoroutine(fade);
	}
	
	IEnumerator Fade (float time) {
		while (timer >= 0 && timer <= 1) {
			timer += Time.deltaTime / time * (toBlack ? 1 : -1);
			image.color = Color.Lerp(Color.clear, fadedColor, Mathf.Clamp(timer, 0, 1));
			yield return null;
		}
		timer = (toBlack ? 1 : 0);
		image.color = Color.Lerp(Color.clear, fadedColor, timer);
		fade = null;
		
		if (toBlack)
			onIsBlack.Invoke();
		else
			onIsClear.Invoke();
	}
}