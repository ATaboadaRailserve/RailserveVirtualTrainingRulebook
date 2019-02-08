using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Fog : MonoBehaviour {

	//Day, Night, Sunrise
	public GameObject[] images;
	public GameObject skyProject;
	public int night = 20;
	public int day = 7;
	public ParticleAnimator fog;
	public Slider fogSlider;
	
	private float density;
	
	void Update () {
		skyProject.SendMessage("GetTime", gameObject);
		density = fogSlider.value;
		FogDensity();
	}
	
	void Time (float time) {
		if (time > night) {
			images[0].SetActive(false);
			images[1].SetActive(true);
		} else if (time > day) {
			images[0].SetActive(true);
			images[1].SetActive(false);
		}
	}
	
	void FogDensity () {
		Color[] modifiedColors = fog.colorAnimation;
		modifiedColors[1].a = (density+0.015f)*2f;
		modifiedColors[2].a = (density+0.015f)*4f;
		modifiedColors[3].a = (density+0.015f)*2f;
        fog.colorAnimation = modifiedColors;
		skyProject.SendMessage("SetDensity", density);
	}
}
