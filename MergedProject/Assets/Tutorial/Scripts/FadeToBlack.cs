using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeToBlack : MonoBehaviour {
	
	public Image black;
	public float timeToFade;
	
	private bool fade;
	private float timer;
	
	void Activate () {
		fade = true;
	}
	
	void Update () {
		if (fade) {
			timer += Time.deltaTime / timeToFade;
			black.color = new Color(0,0,0,Mathf.Lerp(0, 1, timer));
		}
		if (black.color.a >= 1) {
			SceneManager.LoadScene("Menu");
		}
	}
}
