using UnityEngine;
using System.Collections;
using Rewired;

public class ExtraButtons : MonoBehaviour {
	
	public Camera cameraObj;
	
	private int playerId = 0;
	private Player player;  // the Rewired player
	private RaycastHit[] hits;
	
	void Awake () {
		// Get the Player for a particular playerId
		player = ReInput.players.GetPlayer(playerId);
	}
	
	void Update () {
		if (player.GetButtonDown("Fire1")) {
			hits = Physics.RaycastAll(cameraObj.transform.position, cameraObj.transform.forward, 3f);
			foreach (RaycastHit h in hits) {
				h.collider.gameObject.SendMessage("Activate", SendMessageOptions.DontRequireReceiver);
			}
		}
		
		if (player.GetButton("Fire1")) {
			hits = Physics.RaycastAll(cameraObj.transform.position, cameraObj.transform.forward, 3f);
			foreach (RaycastHit h in hits) {
				h.collider.gameObject.SendMessage("ActivateEveryFrame", SendMessageOptions.DontRequireReceiver);
			}
		}
		
		if (player.GetButtonUp("Fire1")) {
			hits = Physics.RaycastAll(cameraObj.transform.position, cameraObj.transform.forward, 3f);
			foreach (RaycastHit h in hits) {
				h.collider.gameObject.SendMessage("ActivateUp", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
