using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheatcodeExecuter : MonoBehaviour {

	private static bool created = false;
	private static string inputLog = "";
	private static bool cheatsEnabled = false;
	
	//[Header("Scripted")]
	public string enableCheatcodes = "$cheats";
	
	//[Header("Generic")]
	public List<Cheatcode> cheatcodes;
	
	public static bool CheatsEnabled
	{
		get {return cheatsEnabled;}
	}
	
	[System.Serializable]
	public class Cheatcode
	{
		public string cheatcode;
		public UnityEvent resultEvent;
	}

	void Awake()
	{
		if(!created)
		{
			DontDestroyOnLoad(this.gameObject);
			created = true;
		}
		else
		{
			Debug.Log("Duplicate Cheatcode Executers. Deleting Self");
			Destroy(this.gameObject);
		}
	}
	
	void Update()
	{
		inputLog += Input.inputString;
		
		if(inputLog.Length > 20)
		{
			inputLog = inputLog.Substring(inputLog.Length - 20, 20);
		}
		
		if(inputLog.IndexOf(enableCheatcodes) != -1)
		{
			inputLog = "";
			cheatsEnabled = !cheatsEnabled;
			Debug.Log("cheats set to: " + cheatsEnabled);
		}
		
		if(!cheatsEnabled)
			return;
		
		foreach (Cheatcode c in cheatcodes)
		{
			if(inputLog.IndexOf(c.cheatcode) != -1)
			{
				inputLog = "";
				c.resultEvent.Invoke();
				break;
			}
		}
	}
	
	public void PrintString(string s)
	{
		Debug.Log(s);
	}
}
