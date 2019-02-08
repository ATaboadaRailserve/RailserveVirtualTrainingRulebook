using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {
	
	private Vector3 origin;
	
	void Start () {
		origin = transform.position;
	}
	
	public void Shake (float time, Vector3 intensity, float speed) {
		StartCoroutine(DoShake(time, intensity, speed));
	}
	
	IEnumerator DoShake (float time, Vector3 intensity, float speed) {
		float timer = 0;
		float randomX = Random.Range(0,9999);
		float randomY = Random.Range(0,9999);
		float randomZ = Random.Range(0,9999);
		while (timer < time) {
			timer += Time.deltaTime;
			transform.position = new Vector3(origin.x + Mathf.PerlinNoise(timer*speed + randomX, 0)*intensity.x, origin.y + Mathf.PerlinNoise(timer*speed + randomY, 0)*intensity.y, origin.z + Mathf.PerlinNoise(timer*speed + randomZ, 0)*intensity.z);
			yield return null;
		}
	}
}
