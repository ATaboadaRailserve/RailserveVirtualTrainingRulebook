using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour {
	
	public GameObject[] skyObjects;	//Sunrise, rise-Day, Day, day-Night, night, night-Rise
	public GameObject[] images; 	//Day, Night, Sunrise
	public int[] times;
	public int currentIndex;
	
	private GameObject currentSky;
	private int index = 0;
	private int timeIndex = 0;
	private bool rain;
	private int hour;
	
	void Start () {
		currentSky = (GameObject)Instantiate(skyObjects[currentIndex], Vector3.zero, Quaternion.identity);
		currentSky.transform.parent = transform.parent;
	}
	
	void Awake () {
		StartCoroutine("Initialize");
	}
	
	IEnumerator Initialize () {
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		rain = false;
		currentSky.SendMessage("SunOn");
	}
	
	void Update () {
		currentSky.SendMessage("GetTime", gameObject);
		if (timeIndex != currentIndex) {
			currentIndex = timeIndex;
			Destroy(currentSky);
			currentSky = (GameObject)Instantiate(skyObjects[currentIndex], Vector3.zero, Quaternion.identity);
			currentSky.transform.parent = transform.parent;
		}
		if (currentIndex%2 == 1) {
			if ((currentIndex > skyObjects.Length-1 && hour == times[(currentIndex+1)/2]) || (currentIndex == skyObjects.Length-1 && hour == times[0])) {
				timeIndex++;
				if (timeIndex >= skyObjects.Length)
					timeIndex = 0;
			}
		}
	}
	
	public void RainToggle () {
		rain = !rain;
		if (rain) {
			currentSky.SendMessage("SunOff");
		} else {
			currentSky.SendMessage("SunOn");
		}
	}
	
	public void Clicked () {
		if (timeIndex%2 == 0) {
			index++;
			if (index >= images.Length)
				index = 0;
			for (int i = 0; i < images.Length; i++) {
				if (i == index)
					images[i].SetActive(true);
				else
					images[i].SetActive(false);
			}
			timeIndex++;
			if (timeIndex >= skyObjects.Length)
				timeIndex = 0;
		}
	}
	
	void Time (int time) {
		hour = time;
	}
	
	void GetTime (GameObject target) {
		target.SendMessage("Time", hour);
	}
}
