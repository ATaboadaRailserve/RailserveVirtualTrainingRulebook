using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Retrain : MonoBehaviour {
	
	public float secondsUntilRetrain = 31536000;
	public bool needsRetrain;
	
	public void CheckForRetrain (string completions) {
		if (completions == "" || completions.Length == 0) {
			Debug.Log("They've not completed it yet so no need to check");
			return;
		}
		int latestTimeStamp = 31536001;
		string[] messages = completions.Split(';');
		for (int i = 0; i < messages.Length; i++) {
			System.TimeSpan diff = System.DateTime.Now - System.DateTime.Parse(messages[i].Split('|')[0]);
			if ((int)diff.TotalSeconds < latestTimeStamp)
				latestTimeStamp = (int)diff.TotalSeconds;
		}
		if (latestTimeStamp > secondsUntilRetrain) {
			needsRetrain = true;
			Debug.Log("It's time to retake the comprehensive final test");
			return;
		}
		Debug.Log("No need to retake the comprehensive final test");
	}
}
