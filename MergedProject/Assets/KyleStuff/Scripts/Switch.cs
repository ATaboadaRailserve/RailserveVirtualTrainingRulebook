using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour {
	
	private Animation animationThing;
	private bool activate = false;
	
	void Start () {
		animationThing = GetComponent<Animation>();
	}
	
	void Change () {
		if (!animationThing.isPlaying) {
			if (activate) {
				animationThing["ArmatureAction"].speed = 1;
				animationThing.Play();
				activate = false;
			} else {
				animationThing["ArmatureAction"].speed = -1;
				animationThing["ArmatureAction"].time = animationThing["ArmatureAction"].length;
				animationThing.Play();
				activate = true;
			}
		}
		print ("Switch is " + activate);
	}
}
