using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UberModule_PPE : UberModule {

	public ClipboardList clipboard;
	public bool hasListener;

	public override void StartModule()
	{
		base.StartModule();
		List<string> clipboardElements = new List<string>{"Glasses", "Boots", "Vest", "Hardhat", "React", "Gloves"};
		clipboard.NewList(clipboardElements);
		if(!hasListener)
		{
			clipboard.OnAllCompleted.AddInitialListener(CompleteModule);
			hasListener = true;
		}
	}
	
	public override void CompleteModule()
	{
		if(hasListener)
		{
			clipboard.OnAllCompleted.RemoveInitialListener(CompleteModule);
			hasListener = false;
		}
		//clipboard.NewList(new List<string>());
		base.CompleteModule();
	}
}
