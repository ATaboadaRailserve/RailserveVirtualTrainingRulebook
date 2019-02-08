using UnityEngine;
using System.Collections;

public class Balloons : MonoBehaviour {
	
	public GameObject balloonPrefab;
	public float delay = 2.5f;
	public Camera cameraObj;
	[HideInInspector]
	public bool spawnDemThings = true;
	
	private Vector2 screenSize;
	
	void Start () {
		screenSize = new Vector2(Screen.width, Screen.height);
	}
	
	public void Win () {
		//StartCoroutine("BALLOONS");
	}
	
	IEnumerator BALLOONS () {
		float timer = delay;
		while(spawnDemThings) {
			timer = delay;
			GameObject temp = (GameObject)Instantiate(balloonPrefab, cameraObj.ScreenToWorldPoint(new Vector3(Random.value*Screen.width, 0, 1)), Quaternion.identity);
			temp.transform.parent = cameraObj.transform;
			temp.transform.localEulerAngles = Vector3.zero;
			while (timer > 0) {
				timer -= Time.deltaTime;
				yield return null;
			}
		}
	}
}
