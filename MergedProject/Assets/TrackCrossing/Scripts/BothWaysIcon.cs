using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BothWaysIcon : MonoBehaviour {
	
	public float rotationSpeed = 30f;
	public Image[] glowLayers;
	public Image icon;
	public PlayerAngleTracker tracker;
	public AudioSource audio;
	
	private float[] angles;
	private float percent = 1f;
	
	void OnEnable () {
		if (audio)
			audio.Play();
	}
	
	void Update () {
		if (tracker)
			percent = (float)Mathf.Min(tracker.rightLoc, tracker.leftLoc) / (float)tracker.angles.Count;
		
		for (int i = 0; i < glowLayers.Length; i++) {
			glowLayers[i].rectTransform.localEulerAngles += new Vector3(0,0,Time.deltaTime*rotationSpeed*(i % 2 == 0 ? 1 : -1));
			
			if (Mathf.Ceil(((1f-Mathf.Pow(1f-percent, 4))*20f)) % 2 == 1 || percent < 0.05f) {
				glowLayers[i].gameObject.SetActive(false);
			} else {
				glowLayers[i].gameObject.SetActive(true);
			}
		}
		icon.fillAmount = percent;
	}
}
