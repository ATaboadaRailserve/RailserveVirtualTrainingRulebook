using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicBasedOnActive : BaseLogic {
	
	public GameObject objectToMonitor;
	
	public override bool IsTrue {
		get { return objectToMonitor.activeInHierarchy; }
	}
}
