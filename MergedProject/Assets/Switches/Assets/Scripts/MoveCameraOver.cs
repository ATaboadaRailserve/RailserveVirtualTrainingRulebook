using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoveCameraOver : MonoBehaviour {
	public bool left;
	bool moveRight = false;
	bool moveLeft = false;
	bool moveMid = false;
	bool checkSwitch = false;
	bool start = false;
	public float speed;
	public Button checkRail;
	public Button moveOver;
	Vector3 tempRot , tempLoc;

	float tempPosx;
	// Use this for initialization
	public void MoveLeftorRight()
	{
		print (left);
		if (left) {moveRight = true;moveLeft = false;start = true;}
		else {moveLeft = true;moveRight = false;start= true;}
	}
	public void CheckSwitch()
	{
		moveRight = false;
		moveLeft = false;
		moveMid = true;
		start = true;
	}
	void Update()
	{
		if (moveLeft) 
		{
			if (start) 
			{
				tempPosx = transform.position.x;
				start = false;
			} 
			else if (transform.position.x > tempPosx - .6f) 
			{
				transform.position = Vector3.Lerp (transform.position, new Vector3 (transform.position.x - .6f, transform.position.y, transform.position.z), speed);
			} 
			else 
			{
				moveLeft = false;
				left = true;
			}
		} 
		else if (moveRight) 
		{
			if (start) 
			{
				tempPosx = transform.position.x;
				start = false;
			}
			else if (transform.position.x < tempPosx + .6f)
			{
				transform.position = Vector3.Lerp (transform.position, new Vector3 (transform.position.x + .6f, transform.position.y, transform.position.z), speed);
			}
			else 
			{
				moveRight = false;
				left = false;
			}
		} 
		else if (moveMid) 
		{
			if(start)
			{
				tempPosx = transform.localEulerAngles.x;
				start = false;
				checkRail.interactable = false;
				if(!checkSwitch)
				{
					tempLoc = transform.position;
					tempRot = transform.localEulerAngles;
					moveOver.enabled = false;
				}
			}
			else if(transform.localEulerAngles.x < tempPosx + 62f && !checkSwitch)
			{
				transform.position = Vector3.Lerp (transform.position, new Vector3 (transform.position.x , transform.position.y+1.5f, transform.position.z+2.9f), speed);
				transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3 (transform.localEulerAngles.x + 62f, transform.localEulerAngles.y, transform.localEulerAngles.z), speed);
			}
			else if(transform.localEulerAngles.x > tempPosx - 62f && checkSwitch)
			{
				transform.position = Vector3.Lerp (transform.position, new Vector3 (transform.position.x , transform.position.y-1.5f, transform.position.z-2.9f), speed);
				transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3 (transform.localEulerAngles.x - 62f, transform.localEulerAngles.y, transform.localEulerAngles.z), speed);
			}
			else
			{
				moveMid = false;
				if(checkSwitch)
				{
					transform.position = tempLoc;
					transform.localEulerAngles = tempRot;
					moveOver.enabled = true;
				}
				checkSwitch = !checkSwitch;
				checkRail.interactable = true;
			}
		}
	}
}
