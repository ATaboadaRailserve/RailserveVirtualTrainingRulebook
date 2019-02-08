using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DebugEvents : MonoBehaviour {

	public DebugEvent[] events;

	[System.Serializable]
	public struct DebugEvent
	{
		public string name;
		public KeyCode key;
		public UnityEvent result;
	}
	
	void Start()
	{
		if(Application.isEditor)
			print("Debug Events Are Enabled");
	}
	
	void Update () {
		if(Application.isEditor)
		{
			foreach(DebugEvent e in events)
			{
				if(Input.GetKeyDown(e.key))
					e.result.Invoke();
			}
		}
	}
}
