using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetrainUI : MonoBehaviour {
	
	public GameObject retrainPrompt;
	
	private EscapeCursor escaper;
	
	void Start () {
		escaper = GameObject.FindWithTag("EscapeCursor").GetComponent<EscapeCursor>();
		if (GameObject.FindWithTag("Retrain") && GameObject.FindWithTag("Retrain").GetComponent<Retrain>() && GameObject.FindWithTag("Retrain").GetComponent<Retrain>().needsRetrain) {
			retrainPrompt.SetActive(true);
			escaper.IsCurrentlyEscaped = true;
		}
	}
	
	void Update () {
		if (retrainPrompt.activeInHierarchy) {
			escaper.IsCurrentlyEscaped = true;
		}
	}
	
	public void Dismiss() {
			retrainPrompt.SetActive(false);
			escaper.IsCurrentlyEscaped = false;
	}
}
