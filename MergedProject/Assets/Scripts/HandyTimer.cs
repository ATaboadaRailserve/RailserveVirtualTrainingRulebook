using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandyTimer : MonoBehaviour {
	
	public bool printEveryFrame;
	
	private float timer;
	private bool timing;
	
	public void StartTimer () {
		timing = true;
	}
	
	public void StopTimer () {
		timing = false;
		print("Timer stopped at " + timer + " s");
	}
	
	public void ResetTimer () {
		timer = 0;
	}
	
	public void PrintTimer () {
		print("Timer is at " + timer + " s");
	}
	
	void Update () {
		if (timing) {
			timer += Time.deltaTime;
		}
		if (printEveryFrame) {
			PrintTimer();
		}
	}
	
	public bool PrintEveryFrame {
		get { return printEveryFrame; }
		set { printEveryFrame = value; }
	}
}
