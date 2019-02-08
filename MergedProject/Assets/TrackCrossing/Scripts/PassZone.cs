using UnityEngine;
using System.Collections;

public class PassZone : MonoBehaviour {
	
	public WarningSystem warnSystem;
	public int indexOfThing;
	
	private bool thingDone;
	
	void OnTriggerEnter (Collider col) {
		if (!thingDone) {
			warnSystem.Pass(indexOfThing);
			thingDone = true;
		}
	}
}
