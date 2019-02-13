using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckStateMachine : BaseLogic {
	
	public string state;
	public StateMachine stateMachine;
	
	public override bool IsTrue {
		get { return stateMachine.CurrentState.name == state; }
	}
}
