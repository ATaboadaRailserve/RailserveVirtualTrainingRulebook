using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProcedureStatusDependentEvents : MonoBehaviour {	

	public ConditionalEvent[] conditionalEvents;
	
	static DataLoader dataLoader;
	
	[System.Serializable]
	public struct ConditionalEvent
	{
		[Header("procedure -cond- target")]
		public int targetStatus;
		public Condition condition;
		public UnityEvent onCondition;
		
		public enum Condition
		{
			DISABLED,
			GREATER,
			LESS,
			GREATER_EQUAL,
			LESS_EQUAL,
			EQUAL,
			ENABLED
		}
	}
	
	void Start()
	{
		if(dataLoader == null)
		{
			dataLoader = GameObject.FindObjectOfType<DataLoader>();
			if(dataLoader == null)
			{
				Debug.LogError("No DataLoader Found!");
				this.enabled = false;
				return;
			}
		}
		for(int i = 0; i < conditionalEvents.Length; i++)
		{
			ConditionalEvent e = conditionalEvents[i];
			switch(e.condition)
			{
				case ConditionalEvent.Condition.DISABLED:
					break;
				case ConditionalEvent.Condition.GREATER:
					if(dataLoader.procedureStatus > e.targetStatus)
					{
						e.onCondition.Invoke();
					}
					break;
				case ConditionalEvent.Condition.LESS:
					if(dataLoader.procedureStatus < e.targetStatus) {e.onCondition.Invoke();}
					break;
				case ConditionalEvent.Condition.GREATER_EQUAL:
					if(dataLoader.procedureStatus >= e.targetStatus) {e.onCondition.Invoke();}
					break;
				case ConditionalEvent.Condition.LESS_EQUAL:
					if(dataLoader.procedureStatus <= e.targetStatus) {e.onCondition.Invoke();}
					break;
				case ConditionalEvent.Condition.EQUAL:
					if(dataLoader.procedureStatus == e.targetStatus) {e.onCondition.Invoke();}
					break;
				case ConditionalEvent.Condition.ENABLED:
					e.onCondition.Invoke();
					break;
			}
		}
	}
}
