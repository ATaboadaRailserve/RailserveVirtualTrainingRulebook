using UnityEngine;
using System.Collections;

public class Compass : MonoBehaviour {
	
	public Transform target;
	public Transform player;
	
	void Update () {
		transform.LookAt(target);
		transform.localEulerAngles += new Vector3(0,180,0);
		transform.localEulerAngles = new Vector3(-90,transform.localEulerAngles.y,0);
		transform.position = player.position + player.forward + Vector3.up/10f;
	}
}
