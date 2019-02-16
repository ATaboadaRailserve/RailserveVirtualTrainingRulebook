using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckStateMachine : BaseLogic {
	
	//public string state;
	//public StateMachine stateMachine;
	
	public StateMachineCheck[] StateMachineList;
	
	public override bool IsTrue {
		//get { return stateMachine.CurrentState.name == state; }
		get {
			bool temp = false;
			foreach(StateMachineCheck s in StateMachineList)
			{
				if(s.stateMachine.CurrentState.name == s.state)
				{
					temp = true;
					break;
				}
			}
			return temp;
		}
	}
	
	[System.Serializable]
    public class StateMachineCheck
    {
        public string state;
		public StateMachine stateMachine;
    } 
}
