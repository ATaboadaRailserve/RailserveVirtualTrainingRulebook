using UnityEngine;
using System.Collections;

public class Cows : MonoBehaviour {
	
	public GameObject cow;
	public bool showSpawnPlane = true;
	public Vector2 spawnPlane = new Vector2(5,10);
	
	[Range(0,1)]
	public float cowDensity = 0.7f;
	public int cluster = 100;
	public int resolution = 10;
	
	private float seed = 0;

	// Use this for initialization
	void Start () {
		showSpawnPlane = false;
		seed = Random.Range(-999999, 999999);
		StartCoroutine("Cowordinate");
	}
	
	IEnumerator Cowordinate () {
		for (int i = (int)-spawnPlane.x/2; i < (int)spawnPlane.x/2; i += resolution) {
			for (int j = (int)-spawnPlane.y/2; j < (int)spawnPlane.y/2; j += resolution) {
				if (Mathf.PerlinNoise((spawnPlane.x+transform.position.x+seed+i)/cluster, (spawnPlane.y+transform.position.z+seed+j)/cluster) > cowDensity) {
					GameObject temp = (GameObject)Instantiate(cow, transform.position - new Vector3(i, 0, j), transform.rotation);
					temp.transform.parent = transform;
					temp.transform.localEulerAngles += new Vector3 (0, Random.Range(0,360), 0);
					temp.transform.position += new Vector3(Random.Range(-resolution/2, resolution/2), 0, Random.Range(-resolution/2, resolution/2));
				}
			}
			yield return null;
		}
	}
	
	void OnDrawGizmos () {
		if (showSpawnPlane) {
			Gizmos.color = Color.green;
			Gizmos.DrawCube(transform.position, new Vector3(spawnPlane.x, 0, spawnPlane.y));
		}
	}
}
