using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Skyboxer : MonoBehaviour {
	
	public Material skybox;
	
	void Start () {
		SceneManager.sceneLoaded += OnSceneLoaded;
		OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
	}
	
	void OnSceneLoaded (Scene scene, LoadSceneMode mode) {
		RenderSettings.skybox = skybox;
		DynamicGI.UpdateEnvironment();
	}
}
