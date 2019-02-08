using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {
	
	public GameObject player;
	public GameObject start;
	public GameObject guiItem;
	public float timeToDisplay = 2.5f;
	
	private bool warning = false;
	private float timeElapsed = 0.0f;
	private GameObject[] tracks;
	
	void Start () {
		tracks = GameObject.FindGameObjectsWithTag("CarSet");
		StartCoroutine("GenerateCars");
		if (guiItem)
			guiItem.SetActive(false);
	}
	
	void Trigger () {
		if (!warning) {
			StartCoroutine("WarnPlayer");
		}
	}
	
	IEnumerator WarnPlayer () {
		player.transform.position = start.transform.position;
		player.transform.rotation = start.transform.rotation;
		player.SendMessage("SetVelocity", Vector3.zero);
		timeElapsed = 0.0f;
		
		if (guiItem)
			guiItem.SetActive(true);
		warning = true;
		player.SendMessage("Toggle", true);
		player.SendMessage("SetControllable", false);
		
		while (timeElapsed < timeToDisplay) {
			timeElapsed += 0.025f;
			yield return null;
		}
		
		if (guiItem)
			guiItem.SetActive(false);
		warning = false;
		player.SendMessage("Toggle", false);
		player.SendMessage("SetControllable", true);
	}
	
	IEnumerator GenerateCars () {
		for (int i = 0; i < tracks.Length; i++) {
			tracks[i].SendMessage("Generate");
			yield return null;
		}
	}
}
