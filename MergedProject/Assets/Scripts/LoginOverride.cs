using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoginOverride : MonoBehaviour {

	public float waitTime = 0.2f;
	public float transitionTime = 0.5f;
	public GameObject menuCurtain;
	public Color white = Color.white;
	public Color clear = Color.white; // inspector alpha = 0
	public UnityEvent OnWaitComplete;

	Image curtainImage;
	// Use this for initialization
	void Start () {
		if(menuCurtain != null)
		{
			curtainImage = menuCurtain.GetComponent<Image>();
			if(curtainImage != null)
				StartCoroutine(DoOverride());
		}
	}
	
	public IEnumerator DoOverride()
	{
		yield return new WaitForSeconds(waitTime);
		OnWaitComplete.Invoke();
		for(float t = 0.0f; t < transitionTime; t += Time.deltaTime)
		{
			curtainImage.color = Color.Lerp(white, clear, t / transitionTime);
			yield return null;
		}
		menuCurtain.SetActive(false);
	}
}
