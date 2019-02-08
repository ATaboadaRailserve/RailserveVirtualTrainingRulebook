using UnityEngine;
using System.Collections;

public class SceneTransaction : MonoBehaviour {

	public int level;
	
	private bool first = true;
	
	void Start(){
		StartCoroutine("Initialize");
	}
	void Update () {
		if(!first && Input.GetAxis("ChangeLevel") != 0) {
			Application.LoadLevel(level);
			if(gameObject.name == "Player") {
				Screen.lockCursor = false;
				gameObject.BroadcastMessage("Toggle", true, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	IEnumerator Initialize(){
		yield return new WaitForSeconds(1);
		first = false;
	}
}
