using UnityEngine;
using System.Collections;
using Rewired;

public class Flashlight : MonoBehaviour {
	
	public bool playable;
	public int night = 20;
	public int day = 7;
	public GameObject sky;
	
	private Light lightObj;
	private bool lightToggled = false;
	
	private int playerId = 0;
	private Player _player; // the Rewired player
	
	private Player player {
		get {
			// Get the Rewired Player object for this player. Refresh it as needed so it will get the new reference after a script recompile in the editor.
			if(_player == null) _player = ReInput.players.GetPlayer(playerId);
			return _player;
		}
	}
	
	void Start () {
		lightObj = GetComponent<Light>();
	}
	
	void Update () {
		if (playable) {
			if (player.GetAxis("Lantern") != 0) {
				if (!lightToggled) {
					lightObj.enabled = !lightObj.enabled;
					lightToggled = true;
				}
			} else if (lightToggled) {
				lightToggled = false;
			}
		} else {
			sky.SendMessage("GetTime", gameObject);
		}
	}
	
	void Time (float time) {
		if (time > night) {
			lightObj.enabled = true;
		} else if (time > day) {
			lightObj.enabled = false;
		}
	}
}
