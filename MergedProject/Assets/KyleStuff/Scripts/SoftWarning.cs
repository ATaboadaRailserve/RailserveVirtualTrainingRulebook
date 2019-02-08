using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SoftWarning : MonoBehaviour {
	
	public GameObject[] warnings;
	
	private LocoScript leaf;
	
	void Start () {
		leaf = GetComponent<LocoScript>();
		foreach (GameObject p in warnings) {
			p.SetActive(false);
		}
	}
	
	public void Warn (int index) {
		foreach (GameObject p in warnings) {
			p.SetActive(false);
		}
		warnings[index].SetActive(true);
	}
	
	public void Dismiss (int index) {
		foreach (GameObject p in warnings) {
			p.SetActive(false);
		}
		
		// For special actions on return, add to switch
		// index 1 is broken coupler, reset scene
		// index 2 is derailed car, reset scene
		switch (index) {
			case 1:
				Application.LoadLevel(1);
				break;
			case 2:
				Application.LoadLevel(1);
				break;
		}
	}
}
