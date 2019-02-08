using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {
	
	public void MouseEnter (string text) {
		print("Button " + text + " Highlighted");
	}
	
	public void Clicked (string text) {
		print("Button " + text + " Clicked");
	}
	
	public void MouseExit (string text) {
		print("Button " + text + " Unhighlighted");
	}
}
