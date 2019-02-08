using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryState : MonoBehaviour {

	public bool startAtZero = true;
	public State stateZero;
	public State stateOne;
	
	public int currentState {get; private set;}
	
	[System.Serializable]
	public class State
	{
		public string name;
		public InteractionHandler.InvokableState OnSet;
		public InteractionHandler.InvokableState OnUnset;
	}
	
	void Start()
	{
		if(startAtZero)
		{
			currentState = 0;
			stateZero.OnSet.Invoke();
		}
		else
		{
			currentState = 1;
			stateOne.OnSet.Invoke();
		}
	}
	
	public void Toggle()
	{
		if(currentState == 0)
		{
			currentState = 1;
			stateZero.OnUnset.Invoke();
			stateOne.OnSet.Invoke();
		}
		else
		{
			currentState = 0;
			stateOne.OnUnset.Invoke();
			stateZero.OnSet.Invoke();
		}
	}
	
	public void SetState(bool one)
	{
		if(one)
		{
			currentState = 1;
			stateZero.OnUnset.Invoke();
			stateOne.OnSet.Invoke();
		}
		else
		{
			currentState = 0;
			stateOne.OnUnset.Invoke();
			stateZero.OnSet.Invoke();
		}
	}
}
