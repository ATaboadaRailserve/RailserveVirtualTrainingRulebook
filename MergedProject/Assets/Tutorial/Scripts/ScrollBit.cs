using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBit : MonoBehaviour {
	
	public Scrollbar scrollbar;
	
	public float amount = 0.1f;
	
	public void Up () {
		if (scrollbar.value <= 1f-amount) {
			scrollbar.value += amount;
		} else {
			scrollbar.value = 1;
		}
	}
	
	public void Down () {
		if (scrollbar.value >= amount) {
			scrollbar.value -= amount;
		} else {
			scrollbar.value = 0;
		}
	}
}
