using UnityEngine;
using System.Collections;

public class Navigation : MonoBehaviour {
	
	public Vector3 corner1;
	public Vector3 corner2;
	
	private Vector3 temp;
	
	void Update () {
		transform.position += Input.GetAxis("Vertical") * transform.forward;
		transform.position += Input.GetAxis("Horizontal") * transform.right;
		transform.position += Input.GetAxis("TranslateVerical") * transform.up;
		
		
		temp = transform.position;
		temp.x = Mathf.Clamp(temp.x, corner1.x, corner2.x);
		temp.y = Mathf.Clamp(temp.y, corner1.y, corner2.y);
		temp.z = Mathf.Clamp(temp.z, corner1.z, corner2.z);
		transform.position = temp;
	}
	
	void OnDrawGizmos () {
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube((corner1+corner2)/2f, new Vector3(Mathf.Abs(corner2.x - corner1.x), Mathf.Abs(corner2.y - corner1.y), Mathf.Abs(corner2.z - corner1.z)));
	}
}