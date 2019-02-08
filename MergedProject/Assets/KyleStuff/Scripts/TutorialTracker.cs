using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TutorialTracker : MonoBehaviour {
	
	[System.Serializable]
	public struct objectReference {
		public int textIndex;
		public int objectIndex;
		public bool turnOn;
		public bool ignoreFader;
	}
	
	public GameObject[] interactiveObjects;
	public objectReference[] interactiveObjectReference;
	public RawImage fader;
	public float fadeTime = 0.5f;
	public GameObject[] text;
	public AudioSource[] audioSources;
	public GameObject startButton;
	public Image background;
	public GameObject panel;
	public GameObject player;
	public GameObject questArrow;
	public GameObject targetObject;
	public GameObject[] turnOffOnStart;
	public GameObject[] turnOnOnEnd;
	public GameObject[] circles;
	public int scaleIterations = 10;
	public GameObject nextTutObj;
	
	private List<objectReference> objectsToToggle;
	private int index = 0;
	private bool first = true;
	private bool zoneActive;
	private Projector[] projectors;
	private bool tutorialing;
	private bool firstPlay = true;
	private float timeLeft;
	
	void Start () {
		fader = GameObject.Find("Fader").GetComponent<RawImage>();
		objectsToToggle = new List<objectReference>();
		projectors = new Projector[circles.Length];
		float temp;
		for (int i = 0; i < circles.Length; i++) {
			circles[i].SetActive(true);
			projectors[i] = circles[i].transform.GetChild(0).gameObject.GetComponent<Projector>();
			Material newMaterial = new Material(projectors[i].material);
			projectors[i].material = newMaterial;
			temp = projectors[i].orthographicSize;
			projectors[i].material.color = new Color (temp/2f, temp/2f, 0, temp/2f);
			circles[i].SetActive(false);
		}
	}
	
	public void StartTutorial () {
		player.SendMessage("SetControllable", false);
		zoneActive = false;
		tutorialing = true;
		startButton.SetActive(false);
		background.enabled = true;
	}
	
	void Update () {
		if (tutorialing) {
			if (index < audioSources.Length) {
				timeLeft = audioSources[index].clip.length - audioSources[index].time;
				if (!audioSources[index].isPlaying) {
					if (firstPlay) {
						objectsToToggle.Clear();
						foreach (objectReference g in interactiveObjectReference) {
							if (index == g.textIndex) {
								objectsToToggle.Add(g);
							}
						}
						if (objectsToToggle.Count != 0)
							StartCoroutine("CrossFade");
						if (index < text.Length)
							text[index].SetActive(true);
						audioSources[index].Play();
						firstPlay = false;
					} else {
						if (index < text.Length)
							text[index].SetActive(false);
						index++;
						firstPlay = true;
					}
				}
				if (timeLeft <= fadeTime + 0.0001f) {
					objectsToToggle.Clear();
					foreach (objectReference g in interactiveObjectReference) {
						if (index == g.textIndex) {
							objectReference temp = new objectReference();
							temp.textIndex = g.textIndex;
							temp.objectIndex = g.objectIndex;
							temp.turnOn = !g.turnOn;
							temp.ignoreFader = g.ignoreFader;
							objectsToToggle.Add(temp);
						}
					}
					if (objectsToToggle.Count != 0)
						StartCoroutine("CrossFade");
				}
			} else {
				tutorialing = false;
				player.SendMessage("SetControllable", true);
				if (nextTutObj)
					nextTutObj.SendMessage("ThisZone");
				background.enabled = false;
				for (int i = 0; i < turnOnOnEnd.Length; i++) {
					turnOnOnEnd[i].SetActive(true);
					turnOnOnEnd[i].SendMessage("TutorialPing", SendMessageOptions.DontRequireReceiver);
				}
				NextZone();
				panel.SetActive(false);
			}
		}
	}
	
	IEnumerator CrossFade () {
		bool ignoreFader = false;
		float time = 0;
		Color color = fader.color;
		foreach (objectReference o in objectsToToggle) {
			if (o.ignoreFader)
				ignoreFader = true;
		}
		if (!ignoreFader) {
			fader.enabled = true;
			color.a = 0;
			fader.color = color;
			while (time > 0) {
				color.a = (fadeTime - time)/fadeTime;
				fader.color = color;
				time -= Time.deltaTime;
				yield return null;
			}
			time = fadeTime;
		}
		foreach (objectReference o in objectsToToggle) {
			interactiveObjects[o.objectIndex].SendMessage("Ding", o.turnOn);
		}
		if (!ignoreFader) {
			while (time > 0) {
				color.a = time/fadeTime;
				fader.color = color;
				time -= Time.deltaTime;
				yield return null;
			}
			fader.enabled = false;
		}
	}
	
	public void NextText () {
		if (index < text.Length-1) {
			if (first) {
				for (int i = 0; i < turnOffOnStart.Length; i++) {
					turnOffOnStart[i].SetActive(false);
				}
				first = false;
			}
			player.SendMessage("SetControllable", false);
			zoneActive = false;
			text[index].SetActive(false);
			index++;
			text[index].SetActive(true);
		} else {
			player.SendMessage("SetControllable", true);
			if (nextTutObj)
				nextTutObj.SendMessage("ThisZone");
			text[index].SetActive(false);
			NextZone();
			panel.SetActive(false);
			for (int i = 0; i < turnOnOnEnd.Length; i++) {
				turnOnOnEnd[i].SetActive(true);
			}
		}
	}
	
	public void PreviousText () {
		if (index > 0) {
			if (first) {
				for (int i = 0; i < turnOffOnStart.Length; i++) {
					turnOffOnStart[i].SetActive(false);
				}
				first = false;
			}
			player.SendMessage("SetControllable", false);
			zoneActive = false;
			text[index].SetActive(false);
			index--;
			text[index].SetActive(true);
		}
	}
	
	void ThisZone () {
		zoneActive = true;
		StartCoroutine("CircleLight");
	}
	
	IEnumerator CircleLight () {
		float temp;
		for (int i = 0; i < circles.Length; i++) {
			circles[i].SetActive(true);
			temp = projectors[i].orthographicSize;
			projectors[i].material.color = new Color (temp/2f, temp/2f, 0, temp/2f);
		}
		while (zoneActive) {
			for (int i = 0; i < circles.Length; i++) {
				projectors[i].material.color = Color.yellow;
				projectors[i].material.color *= -Mathf.Pow((((projectors[i].orthographicSize/2f)-0.5f)*2),2)+1;
				projectors[i].orthographicSize += 2.0f/(float)scaleIterations*Time.deltaTime;
				
				if (projectors[i].orthographicSize >= 2.0f) {
					projectors[i].orthographicSize = 0;
				}
			}
			yield return null;
		}
		for (int i = 0; i < circles.Length; i++) {
			circles[i].SetActive(false);
		}
	}
	
	void NextZone () {
		if (targetObject)
			questArrow.SendMessage("Target", targetObject.transform.position);
		else
			questArrow.SendMessage("Off");
	}
	
	void OnDrawGizmos () {
		Gizmos.color = Color.red;
		if (targetObject)
			Gizmos.DrawLine (transform.position, targetObject.transform.position);
	}
}
