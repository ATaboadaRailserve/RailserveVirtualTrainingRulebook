using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMenu : MonoBehaviour {
	
	public Transform objectToBeMoved;
	public Transform TargPos;

	// Use this for initialization
	void Start () {
		
	}

	void Awake()
	{
		//Make ObjectA's position match objectB
		objectToBeMoved.position = TargPos.position;
	}


	// Update is called once per frame
	void Update () {
		
	}
}
	