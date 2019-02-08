using UnityEngine;
using System.Collections;

public class TutorialNexter : MonoBehaviour {
	
	public TempTutorialize tuter;
	
	void OnTriggerEnter () {
		tuter.Next();
		Destroy(GetComponent<Collider>());
	}
}
