using UnityEngine;
using System.Collections;

public class TunnelHit : MonoBehaviour {

	//------------------------------Private----------------------------
	private Switch_GAME_CONTROL gamecontrol;

	void Start()
	{
		gamecontrol = GameObject.FindGameObjectWithTag ("SwitchGameControl").GetComponent<Switch_GAME_CONTROL> ();
	}

	void OnTriggerEnter(Collider other) {
		if (other.name == "Leaf") {
			gamecontrol.FinishCarThough ();
		} 
		else if (other.name == "Box Car") {
			gamecontrol.FinishCarThough ();
		} 
	}

}
