using UnityEngine;
using System.Collections;

public class WarnTrigger : MonoBehaviour {
	
	public int warningIndex = 1;
	public bool game;
	
	private WarningSystem warningSystem;
	private bool gameOver;
	
	void Start () {
		warningSystem = GameObject.FindWithTag("WarningSystem").GetComponent<WarningSystem>();
	}
	
	void OnTriggerEnter (Collider col) {
		if (gameOver)
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