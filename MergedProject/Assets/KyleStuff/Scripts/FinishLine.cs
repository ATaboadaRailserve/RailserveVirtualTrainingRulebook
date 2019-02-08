using UnityEngine;
using System.Collections;

public class FinishLine : MonoBehaviour {
	
	public LocoScript leaf;
	
	void OnTriggerEnter (Collider col) {
		if (col.tag == "Leaf"){
			leaf.velocity = 0;
			leaf.CheckCars();
		}
	}
}
