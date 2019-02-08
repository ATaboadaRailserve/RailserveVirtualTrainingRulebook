using UnityEngine;
using System.Collections;

public class BlinkScript : MonoBehaviour {

	public int TimeBlinkins = 3;
	public float TimeBetweenBlinkins = 0.5f;
	private float startTime;
	public bool isActive = false;
	public Color visibleColor;
	public Color invisibleColor;
	private float timeAggregate;
	private bool isVisible;

	// Use this for initialization
	void Start () {
		StartDasBlinkins();
	}

	public void StartDasBlinkins() {
		startTime = Time.time;
		isActive = true;
		timeAggregate = 0;
		isVisible = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (isActive &&((Time.time-startTime)<TimeBlinkins)) {
			timeAggregate+=Time.deltaTime;
			if (timeAggregate > TimeBetweenBlinkins) {
				timeAggregate -= TimeBetweenBlinkins;
				isVisible = !isVisible;
				if (isVisible) {
					this.GetComponent<Renderer>().material.color = visibleColor;
				}
				else {
					this.GetComponent<Renderer>().material.color = invisibleColor;
				}
			}
		}
		else {
			isActive = false;
			Destroy(this.gameObject);
		}
	}
}
