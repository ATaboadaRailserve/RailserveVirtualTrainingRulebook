using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuperSeecretKeyCode : MonoBehaviour {
	
	public KeyCode key;
	public GameObject fpsCounter;
	
	private List<int> average = new List<int>();
	private int avgFPS= 0;
	private int averageCount = 40;
	
	void Update () {
		if (Input.GetKeyDown(key)) {
			fpsCounter.SetActive(!fpsCounter.activeInHierarchy);
		}
		
		average.Add((int)(1f / Time.unscaledDeltaTime));
		while (average.Count > averageCount) {
			average.RemoveAt(0);
		}
		
		foreach (int i in average) {
			avgFPS += i;
		}
		avgFPS /= average.Count;
		fpsCounter.GetComponent<Text>().text = "Current FPS: " + (int)(1f / Time.unscaledDeltaTime) + '\n' + "Average FPS: " + avgFPS;
	}
}
