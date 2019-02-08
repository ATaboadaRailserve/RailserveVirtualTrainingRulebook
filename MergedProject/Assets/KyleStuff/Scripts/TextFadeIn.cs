using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextFadeIn : MonoBehaviour {


	private Color currentColor;
	public float startTime;
	public float alphaStep = 0.001f;
	public float maxAlpha = 1.0f;

	private float elapsedTime;

	// Use this for initialization
	void Start () {
		currentColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		GetComponentInChildren<Text>().color = currentColor;
		elapsedTime = 0.0f;

	}
	
	// Update is called once per frame
	void Update () {
		elapsedTime += Time.deltaTime;
		if (elapsedTime > startTime) {
			if (currentColor.a < maxAlpha) {
				currentColor.a += alphaStep;
				GetComponentInChildren<Text>().color = currentColor;
			}
		}

	}
}
