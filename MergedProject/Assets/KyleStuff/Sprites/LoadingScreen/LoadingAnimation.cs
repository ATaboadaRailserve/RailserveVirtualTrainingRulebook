using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingAnimation : MonoBehaviour {
	public Texture[] frames;
	public int framesPerSecond = 32;
	public float loadTime = 5.0f;
	public GameObject loadScreen;
	public GameObject player;
	public GameObject fog;
	public GameObject skyProject;
	
	void Start () {
		StartCoroutine ("Ding");
	}
	
	void Awake () {
		player.BroadcastMessage("Toggle", true);
	}
	 
	void Update() {
		int index = (int)((Time.time * framesPerSecond) % frames.Length);
		GetComponent<RawImage>().texture = frames[index];
	}
	
	IEnumerator Ding () {
		for (float i = 0; i < loadTime; i += Time.deltaTime) {
			fog.SetActive(true);
			yield return null;
		}
		loadScreen.SetActive(false);
		player.BroadcastMessage("Initalize");
		player.BroadcastMessage("Toggle", false);
		fog.SetActive(false);
		skyProject.SendMessage("SetFog");
		skyProject.SendMessage("SetFog");
		gameObject.SetActive(false);
	}
}
