using UnityEngine;
using System.Collections;

public class LightFadeIn : MonoBehaviour {

	private Light light;
	public float startTime;
	public float lightStep = 0.001f;
	public float maxLight = 0.8f;
	public float cutToNextSceneTime = 10.0f;
	private float elapsedTime;

	// Use this for initialization
	void Start () {
		light = this.GetComponent<Light>();
		light.intensity = 0;
		elapsedTime = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		elapsedTime += Time.deltaTime;
		if (elapsedTime > startTime) {
			if (light.intensity < maxLight) {
				light.intensity += lightStep;
			}
		}
		if (elapsedTime > cutToNextSceneTime) {
			Application.LoadLevel("track_master");
		}
	}
}
