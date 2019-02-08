using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUI : MonoBehaviour {
	public Speedometer speedometer;
	public Text text;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		text.text = "Speed: " + speedometer.currentSpeed + "    Distance: " + speedometer.totalDistance;
	}
}
