using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnableEvent : MonoBehaviour {
	public InteractionHandler.InvokableState onTriggered;
	// Use this for initialization
	void Start () {
		
	}


	void Awake(){
		
		onTriggered.Invoke();
			}
	}

