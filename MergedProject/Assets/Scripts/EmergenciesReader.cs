using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmergenciesReader : MonoBehaviour {

	public CommsEmergenciesGameManager manager;
	public string source;
	public Text result;
	
	void Update () {
		result.text = source.Replace("*1", manager.FoundEmergencies.ToString()).Replace("*2", manager.NumEmergencies.ToString());
	}
}
