using UnityEngine;
using System.Collections;

public class BoxCarnivore : MonoBehaviour {
	
	public float colorOverridePercent = 0.75f;
	public Color[] colors;
	public GameObject[] lodsToColor;
	
	// Use this for initialization
	void Awake () {
		if (Random.value < colorOverridePercent) {
			int chosenOne = Random.Range(0, colors.Length);
			foreach (GameObject box in lodsToColor) {
				box.GetComponent<Renderer>().material.color = colors[chosenOne];
			}
		}
	}
}
