using UnityEngine;
using System.Collections;

public class RedZone : MonoBehaviour {
	
	public bool game;
	
	public Vector3 redZone;
	public Vector3 offSet;
	public bool useRedZone = true;
	public GameObject redZoneVisual;
	
	private GameObject redZoneVisualSpawn;
	private WarningSystem warningSystem;
	private bool gameOver;
	
	void Start () {
		warningSystem = GameObject.FindWithTag("WarningSystem").GetComponent<WarningSystem>();
		if (useRedZone) {
			redZoneVisualSpawn = (GameObject)Instantiate(redZoneVisual);
			redZoneVisualSpawn.transform.parent = transform;
			redZoneVisualSpawn.transform.localPosition = offSet - new Vector3(0,2f,0);
			redZoneVisualSpawn.transform.localScale = new Vector3(redZone.x, 0.1f, redZone.z);
		}
	}
	
	void OnTriggerEnter (Collider col) {
		if (game && gameOver)
			return;
		if (col.gameObject.tag == "Player") {
			warningSystem.Warn(1);
			if (game) {
				GameObject.FindWithTag("CrossingRailer").SendMessage("GameOver", warningSystem.warnings[1].name);
				gameOver = true;
			}
		}
	}
}
