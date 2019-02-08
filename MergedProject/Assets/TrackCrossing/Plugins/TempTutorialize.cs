using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TempTutorialize : MonoBehaviour {
	
	[System.Serializable]
	public struct Step {
		public string instructions;
		public GameObject thing;
	}
	
	public Step[] steps;
	public Text currentInstructions;
	public Compass compass;
	
	private int index = -1;
	
	void Start () {
		Next();
	}
	
	public void Next () {
		for (int i = 0; i < steps.Length; i++) {
			if (steps[i].thing)
				steps[i].thing.SetActive(false);
		}
		if (index < steps.Length-1) {
			index++;
			if (steps[0].thing) {
				compass.gameObject.SetActive(true);
				steps[index].thing.SetActive(true);
				compass.target = steps[index].thing.transform;
			} else {
				compass.gameObject.SetActive(false);
			}
			currentInstructions.text = steps[index].instructions;
		} else {
			compass.gameObject.SetActive(false);
			currentInstructions.gameObject.SetActive(false);
		}
	}
}
