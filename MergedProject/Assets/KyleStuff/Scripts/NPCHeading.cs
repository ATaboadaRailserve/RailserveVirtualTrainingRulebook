using UnityEngine;
using System.Collections;

public class NPCHeading : MonoBehaviour {
	
	public Vector3 startRotation;
	
	void Awake () {
		StartCoroutine("SetRot");
	}
	
	IEnumerator SetRot () {
		yield return null;
		yield return null;
		yield return null;
		transform.localEulerAngles = startRotation;
	}
}
