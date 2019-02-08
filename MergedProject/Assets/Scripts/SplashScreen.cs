using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour {
	
	public string scene;
	public float time;
	public Image fader;
	public Color fadeFrom;
	public Color fadeTo;
	public float fadePower = 4;
	public KeyCode skipKey;
	
	void Start () {
		StartCoroutine(ChangeScenes());
	}
	
	IEnumerator ChangeScenes () {
		float timer = time;
		
		while (timer > 0) {
			fader.color = Color.Lerp(fadeFrom, fadeTo, -Mathf.Pow((((1f-timer/time)-0.5f)*2f), fadePower)+1f);
			timer -= Time.deltaTime;
			if (Input.GetKeyDown(skipKey)) {
				fader.color = Color.black;
				break;
			}
			yield return null;
		}
		
		SceneManager.LoadScene(scene);
	}
}
