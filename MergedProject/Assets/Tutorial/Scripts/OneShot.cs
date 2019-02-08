using UnityEngine;
using System.Collections;

public class OneShot : MonoBehaviour {
	
	[System.Serializable]
	public enum ShotType { Controllable , NotControllable }
	
	public ShotType shotType;
	public GameObject player;
	
	void OnEnable () {
		print("One Shotting: " + gameObject.name);
		switch(shotType) {
			case ((ShotType)0):
				player.SendMessage("SetControllable", true);
				break;
			case ((ShotType)1):
				player.SendMessage("SetControllable", false);
				break;
		}
		gameObject.SetActive(false);
	}
}
