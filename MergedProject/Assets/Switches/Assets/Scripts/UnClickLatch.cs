using UnityEngine;
using System.Collections;

public class UnClickLatch : MonoBehaviour
{
	private Camera cam; //MainCamera
	private GameObject switchGameobject; //Lever Gameobject
	private bool isMouseDown; //is mouse down
	public bool left; //is latch Left or Right side
	private GrabandRotate grabandrotatescript; //Lever's Script
	private SwitchTutorial tutorialscript;
	private AudioSource unclick;
	private bool down = false;

	// Use this for initialization
	void Start ()
	{
		switchGameobject = transform.parent.FindChild("Lever_1").FindChild("Lever_0").gameObject; //Finds the Lever GameObject
		grabandrotatescript = switchGameobject.GetComponent<GrabandRotate>(); //Finds the script for the Lever GameObject
		cam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>(); //Finds the MainCamera
		unclick = transform.parent.FindChild("Clamps").GetComponent<AudioSource>();
		if (grabandrotatescript.type == GrabandRotate.SwitchType.Tutorial) {
			tutorialscript = GameObject.FindGameObjectWithTag ("TutorialControl").GetComponent<SwitchTutorial> ();
		}
	}


	void Update ()
	{
		if (isMouseDown) //If mouse is down
		{
			RaycastHit hit; 
			Ray ray = cam.ScreenPointToRay (Input.mousePosition); //set raycast
			if (Physics.Raycast (ray, out hit)) //If raycast hits the latch
			{
				if (switchGameobject.transform.parent.transform.localEulerAngles.z <= grabandrotatescript.minRotation+3f && !left) //If the latch is on the right side and the switch is in the bounds of the right side
				{
					grabandrotatescript.locked = false;//unlock latch
					if(!down)
					{
						unclick.Play();
						down = true;
					}
				} 
				else if (switchGameobject.transform.parent.transform.localEulerAngles.z >= grabandrotatescript.maxRotation-3f && left)  //If the latch is on the left side and the switch is in the bounds of the left side
				{
					grabandrotatescript.locked = false;//unlock latch
					if(!down)
					{
						unclick.Play();
						down = true;
					}
				}
				if (grabandrotatescript.type == GrabandRotate.SwitchType.Tutorial && tutorialscript.current == 1) 
				{
					tutorialscript.NextStep (1);
				}
			}
		}
		else
		{
			down = false;
		}
	}

	public void OnMouseDown ()
	{
		isMouseDown = true; //mouse is down
	}

	public void OnMouseUp ()
	{
		isMouseDown = false; //mouse is not down
	}
}
