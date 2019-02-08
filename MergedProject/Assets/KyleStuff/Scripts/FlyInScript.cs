using UnityEngine;
using System.Collections;

public class FlyInScript : MonoBehaviour {

	public Transform endPoint;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (this.transform.position.z > endPoint.transform.position.z) {
			this.transform.Translate(0,1,0);
		}
		else {
			StartCoroutine(LoadnDaLevel());
		}
	}

	public IEnumerator LoadnDaLevel() {

		yield return new WaitForSeconds(3f); 
		Application.LoadLevel("track_master"); 
	}

}
