using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraAutoSwitcher : MonoBehaviour {

	public AnimationScrubber scrubber;
	public List<OnandOff> OnOffList;
	public List<ChangeCameraOverTime> camTimeList;

	void Start () {
	}

	void Update()
	{
		float f = scrubber.GetTime ();
		if (scrubber.IsPlaying()) {
			for (int i = 0; i < camTimeList.Count; i++) {
				if (f > camTimeList [i].time && f <= camTimeList [i].time + 1) {
					SwitchCam (camTimeList [i].cam);
				}
			}
		}
	}
	public void SwitchCam(int i)
	{
		foreach (GameObject g in OnOffList[i].offList) {
			g.SetActive (false);
		}
		foreach (GameObject g in OnOffList[i].onList) {
			g.SetActive (true);
		}
	}
}
[System.Serializable]
public class OnandOff
{
	public List<GameObject> offList;
	public List<GameObject> onList;
}
[System.Serializable]
public class ChangeCameraOverTime
{
	public int time;
	public int cam;
}



	

