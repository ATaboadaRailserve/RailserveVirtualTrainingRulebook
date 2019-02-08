﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseLogic : MonoBehaviour {
	
	public Gate[] parentGates;
	public bool isTrue;
	public InteractionHandler.InvokableState onTrue;
	public InteractionHandler.InvokableState onFalse;
	
	public bool IsTrue {
		get { return isTrue; }
		set {
			if (isTrue == value)
				return;
			isTrue = value;
			foreach (Gate pg in parentGates) {
				pg.UpdateLogic();
			}
			if (isTrue)
				onTrue.Invoke();
			else
				onFalse.Invoke();
		}
	}
}
