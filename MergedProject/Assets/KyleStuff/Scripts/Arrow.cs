using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {
	
	public Vector3 target;
	public GameObject player;
	public float fadeDistance = 10.0f;
	public GameObject[] subObjects;
	
	private float angle;
	private float scale;
	private float alpha;
	private Vector3 endVector;
	private Color color;
	
	void Start () {
		if (subObjects.Length != 0)
			color = subObjects[0].GetComponent<Renderer>().material.color;
	}
	
	void FixedUpdate () {
		endVector = target-transform.position;
		
		angle = Mathf.Atan2(endVector.x, endVector.z)*(180/Mathf.PI);
		transform.localEulerAngles = new Vector3(270, angle, 0);
		transform.position = player.transform.position - new Vector3 (0,0.25f,0);
		
		scale = Mathf.Clamp((endVector.magnitude-(fadeDistance/2))/fadeDistance, 0, 1.0f);
		transform.localScale = new Vector3(1,scale,1);
		
		alpha = Mathf.Clamp(endVector.magnitude-fadeDistance, 0, 1);
		for (int i = 0; i < subObjects.Length; i++)
			subObjects[i].GetComponent<Renderer>().material.color = color * new Color (1,1,1,alpha);
	}
	
	void Target (Vector3 newTarget) {
		target = newTarget;
	}
	
	void Off () {
		gameObject.SetActive(false);
	}
	
	void OnDrawGizmos () {
		Gizmos.color = Color.green;
		Gizmos.DrawLine (transform.position, target);
	}
}
