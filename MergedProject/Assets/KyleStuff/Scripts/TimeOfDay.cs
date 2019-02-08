using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeOfDay : MonoBehaviour {

	//Day, Night, Sunrise
	public GameObject[] images;
	public int[] times;
	public GameObject skyProject;
	
	private int index = 0;
	private int hour;
	private bool changeTime;
	private int targetHour;
	
	void Update () {
		if (changeTime) {
			skyProject.SendMessage("GetTime", gameObject);
			if (hour == targetHour) {
				skyProject.SendMessage("SetTime");
				changeTime = false;
			}
		}
	}
	
	public void Clicked () {
		skyProject.SendMessage("SetTime");
		changeTime = true;
		targetHour = times[index];
		index++;
		if (index >= images.Length)
			index = 0;
		for (int i = 0; i < images.Length; i++) {
			if (i == index)
				images[i].SetActive(true);
			else
				images[i].SetActive(false);
		}
	}
	
	void Time (int time) {
		hour = time;
	}
}
