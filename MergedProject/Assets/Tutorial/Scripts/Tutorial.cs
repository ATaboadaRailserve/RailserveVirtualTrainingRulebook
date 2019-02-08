using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {
	
	[System.Serializable]
	public struct Step {
		public string name;
		public GameObject[] turnOn;
		public GameObject[] turnOff;
	}
	
	public GameObject[] turnOnInitial;
	public GameObject[] turnOffInitial;
	public Step[] steps;
	public GameObject nukeTutorial;
	public GameObject controllable;
	public DatabaseMessageHolder messageHolder;
	
	private int index;
	
	void Start () {
		StartCoroutine("Initialize");
	}
	
	IEnumerator Initialize () {
		//yield return null;
		do {
			print("Waiting");
			yield return null;
		} while (!GameObject.FindWithTag("DataLoader"));
		print(GameObject.FindWithTag("DataLoader").GetComponent<DataLoader>().CurrentUser.trainingPorcedure);
		//if (GameObject.FindWithTag("DataLoader") && GameObject.FindWithTag("DataLoader").GetComponent<DataLoader>().CurrentUser.trainingPorcedure.Length > 0 && GameObject.FindWithTag("DataLoader").GetComponent<DataLoader>().CurrentUser.trainingPorcedure[0] == 'A') {
		if (true) { // Seriously bad
			print("Nuking Tutorial");
			nukeTutorial.SetActive(true);
		} else {
			for (int i = 0; i < turnOnInitial.Length; i++) {
				turnOnInitial[i].SetActive(true);
			}
			for (int i = 0; i < turnOffInitial.Length; i++) {
				turnOffInitial[i].SetActive(false);
			}
		}
	}
	
	public void Next () {
		if (index < steps.Length-2) {
			index++;
			for (int i = 0; i < steps[index].turnOn.Length; i++) {
				steps[index].turnOn[i].SetActive(true);
			}
			for (int i = 0; i < steps[index].turnOff.Length; i++) {
				steps[index].turnOff[i].SetActive(false);
			}
		} else if (index == steps.Length-2) {
			index++;
			for (int i = 0; i < steps[index].turnOn.Length; i++) {
				steps[index].turnOn[i].SetActive(true);
			}
			for (int i = 0; i < steps[index].turnOff.Length; i++) {
				steps[index].turnOff[i].SetActive(false);
			}
			messageHolder.moduleFinished = true;
			messageHolder.WriteMessage("100", 4);
			messageHolder.PushingMessages();
		} else {
			messageHolder.moduleFinished = true;
			messageHolder.WriteMessage("100", 4);
			messageHolder.PushingMessages();
		}
	}
}
