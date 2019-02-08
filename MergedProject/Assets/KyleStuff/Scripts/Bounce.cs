using UnityEngine;
using System.Collections;

public class Bounce : MonoBehaviour {
	
	public float bounceRate = 2.0f;
	public float bounceAmount = 0.1f;
	public float twistRate = 5.0f;
	public float twistAmount = 5.0f;
	public float originScale = 1;
	
	private float bounceTime;
	private float twistTime;
	private float scale;
	private float twist;
	private Vector3 originRotation;
	
	void Start () {
		twistTime = Random.value;
		originRotation = transform.localEulerAngles;
	}
	
	void Update () {
		bounceTime += bounceRate * Time.deltaTime;
		if (bounceTime > 1)
			bounceTime -= 1.0f;
		
		twistTime += twistRate * Time.deltaTime;
		if (twistTime > 1)
			twistTime -= 1.0f;
		
		scale = 1-((Mathf.Cos(bounceTime*2*Mathf.PI))/2)*bounceAmount;
		twist = ((Mathf.Cos(twistTime*2*Mathf.PI))/2)*twistAmount;
		
		gameObject.transform.localScale = new Vector3(scale, scale, scale)*originScale;
		gameObject.transform.localEulerAngles = new Vector3(0, twist, 0) + originRotation;
	}
}
