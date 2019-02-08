using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
	
	public Vector2 angles = new Vector2(0,120);
	public float timeToOpen = 1;
	
	private float timer = 0;
	private bool opening;
	
	void ActivateUp () {
		opening = !opening;
	}
	
	void Update () {
		/*
		if (opening) {
			if (timer < 1-Time.deltaTime / timeToOpen) {
				timer += Time.deltaTime / timeToOpen;
			} else {
				timer = 1;
			}
		} else if (!opening) {
			if (timer > Time.deltaTime / timeToOpen) {
				timer -= Time.deltaTime / timeToOpen;
			} else {
				timer = 0;
			}
		}
		*/
		
		if ((!opening && timer > 0) || (opening && timer < 1))
			timer += (Time.deltaTime / timeToOpen) * (opening ? 1 : -1);
		else
			timer = (opening ? 1 : 0);
		
		transform.localEulerAngles = Vector3.Lerp(new Vector3(-90,angles.x,0), new Vector3(-90,angles.y,0), timer);
	}
}
