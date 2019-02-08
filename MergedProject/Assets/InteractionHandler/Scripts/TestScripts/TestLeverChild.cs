using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLeverChild : MonoBehaviour {
	
	public void On () {
		GetComponent<Renderer>().material.color = Color.green;
	}
	
	public void Off () {
		GetComponent<Renderer>().material.color = Color.red;
	}
}
