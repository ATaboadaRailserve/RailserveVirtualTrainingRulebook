using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Retical_Changer : MonoBehaviour {
	private Camera cam;
	public Sprite openHand, closedHand, regular, unlock, locked;
	public Image myImage;
	// Use this for initialization
	void Start () 
	{
		cam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		RaycastHit hit;
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast (ray, out hit, 1.5f)) 
		{
			if (hit.collider.tag == "Grabable") 
			{
				if (Input.GetMouseButton (0)) 
				{
					myImage.sprite = closedHand;
				} 
				else 
				{
					myImage.sprite = openHand;
				}
			} 
			else if (hit.collider.tag == "Unlockable") 
			{
				SwitchMasterControl smc = hit.collider.transform.parent.GetComponent<SwitchMasterControl> ();
				bool islocked = smc.locked;
				if (islocked) 
				{
					myImage.sprite = locked;
				} 
				else 
				{
					myImage.sprite = unlock;
				}
			} 
			else 
			{
				myImage.sprite = regular;
			}
		} 
		else 
		{
			myImage.sprite = regular;
		}
	}
}
