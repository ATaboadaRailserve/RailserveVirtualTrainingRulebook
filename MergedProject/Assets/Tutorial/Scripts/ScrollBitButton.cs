using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBitButton : MonoBehaviour {
	
	public Scrollbar target;
	public float amount;
	
	private bool decreasing;
	
	void Start () {
		if (amount < 0)
			decreasing = true;
	}
	
	void ActivateEveryFrame () {
		if (decreasing) {
			if (target.value > -amount*Time.deltaTime) {
				target.value += amount*Time.deltaTime;
				return;
			} else {
				target.value = 0;
				return;
			}
		}
		
		if (target.value < 1f - amount*Time.deltaTime)
			target.value += amount*Time.deltaTime;
		else
			target.value = 1;
	}
}
