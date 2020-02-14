using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Options : MonoBehaviour {
	
	public GameObject options;
	public Slider sensitivitySlider;
	
	void Start () {
		SceneManager.sceneLoaded += OnSceneLoaded;
		
		OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
	}
	
	void OnSceneLoaded (Scene scene, LoadSceneMode mode) {
		if (!GameObject.FindWithTag("Player"))
			return;
		//GameObject.FindWithTag("Player").GetComponent<PlayerController>().SetSensitivity((float)sensitivitySlider.value/10f);
		sensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity",30) * 10f;
	}
	
	public void UpdateSensitivity () {
		if (!GameObject.FindWithTag("Player"))
			return;
		GameObject.FindWithTag("Player").GetComponent<PlayerController>().SetSensitivity((float)sensitivitySlider.value/10f);
		PlayerPrefs.SetFloat("MouseSensitivity",(float)sensitivitySlider.value/10f);
		PlayerPrefs.Save();
	}
	
	public void ToggleOptionsMenu () {
		options.SetActive(!options.activeInHierarchy);
	}
}
