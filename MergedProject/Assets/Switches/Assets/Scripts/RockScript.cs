using UnityEngine;
using System.Collections;

public class RockScript : MonoBehaviour {
	Camera cam;
	public bool hideArrow;
	public bool hideRockTexture;
	public bool near;
	GameObject arrowGO;
	Renderer mytexture;
	GrabandRotate garScript;
	// Use this for initialization
	void Start () {
		cam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
		arrowGO = transform.FindChild ("Arrow").gameObject;
		garScript = this.transform.parent.FindChild("Lever_1").FindChild("Lever_0").GetComponent<GrabandRotate>();
		mytexture = gameObject.GetComponent<Renderer> ();
		if (hideArrow) 
		{
			arrowGO.SetActive (false);
		}
		if (hideRockTexture)
		{
			mytexture.material = null;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0) ) 
		{
			RaycastHit hit;
			Ray ray = cam.ScreenPointToRay (Input.mousePosition);
			Physics.Raycast (ray, out hit, 2f);
			if (hit.collider == gameObject.GetComponent<Collider> ()) {//if raycast on mouse hits hitbox 
				if (near) {
					garScript.nearState = -1;
				} else {
					garScript.farState = -1;
				}
				gameObject.SetActive (false);

				//print ("Rock Removed");
			}//
		}
		if (!hideRockTexture) {
			float scrollspeed = .2f * Time.time;
			mytexture.material.mainTextureOffset = new Vector2 (scrollspeed, -scrollspeed);
		}
		if (!hideArrow) {
			arrowGO.SetActive (true);
		}
	}
}
