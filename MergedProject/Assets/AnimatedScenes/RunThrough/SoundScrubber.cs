using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundScrubber : MonoBehaviour {

	public AnimationScrubber scrubber;
	private float lasttime;
	public List<AmbientSound> ambientList = new List<AmbientSound> ();
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (scrubber.GetTime () != lasttime) //Scrubber Moving
		{
			foreach (AmbientSound s in ambientList) 
			{
				if (!s.audio.isPlaying && scrubber.GetTime () > s.timeFrame.x && scrubber.GetTime () < s.timeFrame.y) //Audio is not Playing and in time frame
				{
					s.audio.Play ();
				} else if (s.audio.isPlaying && (scrubber.GetTime () <= s.timeFrame.x || scrubber.GetTime () >= s.timeFrame.y)){
					s.audio.Pause ();
				}
					
			}
		} 
		else //Scrubber Not Moving
		{
			foreach (AmbientSound s in ambientList) 
			{
				if (s.audio.isPlaying)
				s.audio.Pause ();
			}
		}
		lasttime = scrubber.GetTime ();
	}
}

[System.Serializable]
public class AmbientSound
{
	public Vector2 timeFrame;
	public AudioSource audio;
}
