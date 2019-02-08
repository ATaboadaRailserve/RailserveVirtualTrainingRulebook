using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WarningSystem : MonoBehaviour {
	
	[System.Serializable]
	public struct Credit {
		public string name;
		[HideInInspector]
		public int numberOfCredits;
		[Range(0,100)]
		public int points;
	}
	
	[System.Serializable]
	public struct Warning {
		public string name;
		[HideInInspector]
		public int numberOfInfractions;
		[Range(0,100)]
		public int points;
		public int creditParent;
	}
	
	[Header("Module")]
	public DatabaseMessageHolder.Module moduleName;
	
	[Header("Gameplay")]
	public Credit[] credits;
	public Warning[] warnings;
	public AudioSource warnAudio;
	public AudioSource creditAudio;
	public bool game;
	
	[Header("Display")]
	public GameObject warningDisplay;
	public Text warningText;
	public float warningTime;
	
	private DatabaseMessageHolder messageHolder;
	private float timer;
	
	void Start () {
		gameObject.tag = "WarningSystem";

        int count = 0;
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("MessageHolder")) {
			if (go.GetComponent<DataLoader>())
				messageHolder = go.GetComponent<DatabaseMessageHolder>();
            Debug.Log("MessageHolder Counter: " + count);
            count++;
            break;
		}
		
		if (!messageHolder && GameObject.FindWithTag("MessageHolder"))
			messageHolder = GameObject.FindWithTag("MessageHolder").GetComponent<DatabaseMessageHolder>();
		
		messageHolder.WriteMessage("Training Started");
        Debug.Log("Write Training Started");
		PushMessages();
	}
	
	public void Comment (string message) {
		messageHolder.WriteMessage(message);
	}
	
	public void Pass (int index) {
        if (creditAudio)
            creditAudio.Play();
		credits[index].numberOfCredits++;
		if (warningDisplay) {
			timer = warningTime;
			warningText.text = credits[index].name;
			StartCoroutine(WarnTimer());
		}
		messageHolder.WriteMessage(credits[index].name, 3);
		//PushMessages();
	}
	
	public void Warn (int index) {
        if (warnAudio)
		    warnAudio.Play();
		warnings[index].numberOfInfractions++;
		if (warningDisplay) {
			timer = warningTime;
            warningText.text = warnings[index].name;
			if (!game)
				warningText.text += ": " + warnings[index].numberOfInfractions;
			StartCoroutine(WarnTimer());
		}
		messageHolder.WriteMessage(warnings[index].name, 2);
		//PushMessages();
	}
	
	public int SumTotal () {
		List<int> sum = new List<int>();
		for (int i = 0; i < credits.Length; i++) {
			int temp = credits[i].points * credits[i].numberOfCredits;
			foreach (Warning w in warnings) {
				if (w.creditParent == i) {
					temp -= w.points * w.numberOfInfractions;
				}
			}
			sum.Add(temp);
		}
		int totalSum = 0;
		for (int i = 0; i < sum.Count; i++) {
			totalSum += sum[i];
		}
		totalSum /= credits.Length;
		return (totalSum > 0) ? totalSum : 0;
	}
	
	public int SumSegment (int[] creditIndices, int[] warningIndices) {
		int sum = 0;
		for (int i = 0; i < creditIndices.Length; i++) {
			if (creditIndices[i] >= 0 && creditIndices[i] < credits.Length)
				sum += credits[creditIndices[i]].points * credits[creditIndices[i]].numberOfCredits;
		}
		for (int i = 0; i < warningIndices.Length; i++) {
			if (warningIndices[i] >= 0 && warningIndices[i] < warnings.Length)
				sum -= warnings[warningIndices[i]].points * warnings[warningIndices[i]].numberOfInfractions;
		}
		return (sum > 0) ? sum : 0;
	}
	
	public void PushMessages() {
		messageHolder.PushingMessages();
	}
	
	public void EndGame () {
		if (game && SumTotal() != 100) {
			return;
		}
		messageHolder.WriteMessage(SumTotal().ToString(), 4);
		PushMessages();
	}
	
	IEnumerator WarnTimer () {
		warningDisplay.SetActive(true);
		while (timer > 0) {
			timer -= Time.deltaTime;
			yield return null;
		}
		warningDisplay.SetActive(false);
	}
}