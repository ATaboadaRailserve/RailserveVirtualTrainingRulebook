using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Retrain : MonoBehaviour {
	
	public float secondsUntilRetrain = 31536000;
	public bool needsRetrain;
	
	public void CheckForRetrain (string completions) {
		if (completions == "" || completions.Length == 0)
			return;
		int latestTimeStamp = 31536001;
		string[] messages = completions.Split(';');
		for (int i = 0; i < messages.Length; i++) {
			System.TimeSpan diff = System.DateTime.Now - System.DateTime.Parse(messages[i].Split('|')[0]);
			if ((int)diff.TotalSeconds < latestTimeStamp)
				latestTimeStamp = (int)diff.TotalSeconds;
		}
		print(latestTimeStamp);
		if (latestTimeStamp > secondsUntilRetrain) {
			needsRetrain = true;
			print("NEED RETRAIN!");
		}
	}
}
