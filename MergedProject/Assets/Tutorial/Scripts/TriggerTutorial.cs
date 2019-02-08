using UnityEngine;
using System.Collections;

public class TriggerTutorial : MonoBehaviour {
	
	public Tutorial tutorial;
	
	void OnTriggerEnter () {
		tutorial.Next();
		Destroy(gameObject);
	}
}
