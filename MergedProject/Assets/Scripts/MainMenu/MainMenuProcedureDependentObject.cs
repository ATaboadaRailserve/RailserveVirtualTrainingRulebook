using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuProcedureDependentObject : MonoBehaviour {

	public MainMenuBooklet booklet;
	public int targetStatus = -1;
	[Header("procedure -cond- target")]
	public Condition enableCondition = Condition.GREATER;
	
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
	
	int lastProcedureStatus = -1;
	
	void Start()
	{
		if(!booklet)
		{
			Debug.LogWarning(gameObject.name + " not linked with booklet. Disabled.");
			gameObject.SetActive(false);
		}
	}
	
	void Update () {
		if(lastProcedureStatus != booklet.ProcedureStatus)
		{
			GameObject child = gameObject.transform.GetChild(0).gameObject;
			lastProcedureStatus = booklet.ProcedureStatus;
			bool wasActive = child.activeSelf;
			
			switch(enableCondition)
			{
				case Condition.DISABLED:
					child.SetActive(false);
					break;
				case Condition.GREATER:
					Debug.Log(targetStatus+":"+lastProcedureStatus);
					child.SetActive(lastProcedureStatus > targetStatus);
					break;
				case Condition.LESS:
					child.SetActive(lastProcedureStatus < targetStatus);
					break;
				case Condition.GREATER_EQUAL:
					child.SetActive(lastProcedureStatus >= targetStatus);
					break;
				case Condition.LESS_EQUAL:
					child.SetActive(lastProcedureStatus <= targetStatus);
					break;
				case Condition.EQUAL:
					child.SetActive(lastProcedureStatus == targetStatus);
					break;
				case Condition.ENABLED:
					child.SetActive(true);
					break;
			}
			
			if (wasActive != child.activeSelf)
			{
				Debug.Log(gameObject.name + " procedure active state set to " + child.activeSelf);
			}
		}
	}
}
