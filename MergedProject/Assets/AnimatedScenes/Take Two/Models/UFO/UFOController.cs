using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOController : MonoBehaviour {
	
	public Transform spinner;
	public Vector3 spinnerSpeed = new Vector3(0,5,0);
	
	public GameObject ringPrefab;
	public float ringEmissionRate = 0.5f;
	public float ringLife;
	public Vector3 ringDirection;
	public bool showRingDirection;
	
	private List<GameObject> rings;
	private List<float> ringLifeCounter;
	private float timer;
	private GameObject workerGO;
	
	void Start () {
		rings = new List<GameObject>();
		ringLifeCounter = new List<float>();
		timer = ringEmissionRate;
	}
	
	void Update () {
		spinner.transform.localEulerAngles += spinnerSpeed*Time.deltaTime;
		if (timer > Time.deltaTime) {
			timer -= Time.deltaTime;
		} else {
			timer = ringEmissionRate;
			workerGO = (GameObject)Instantiate(ringPrefab);
			workerGO.transform.parent = transform;
			workerGO.transform.localPosition = ringPrefab.transform.localPosition;
			workerGO.transform.localEulerAngles = ringPrefab.transform.localEulerAngles;
			workerGO.transform.localScale = ringPrefab.transform.localScale;
			workerGO.SetActive(true);
			rings.Add(workerGO);
			ringLifeCounter.Add(ringLife);
		}
		
		while (ringLifeCounter.Count > 0 && ringLifeCounter[0] <= Time.deltaTime) {
			Destroy(rings[0]);
			rings.RemoveAt(0);
			ringLifeCounter.RemoveAt(0);
		}
		for (int i = 0; i < ringLifeCounter.Count; i++) {
			ringLifeCounter[i] -= Time.deltaTime;
			rings[i].transform.localPosition += ringDirection*Time.deltaTime;
		}
	}
	
	void OnDrawGizmos () {
		if (showRingDirection) {
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, transform.position + ringDirection);
		}
	}
}
