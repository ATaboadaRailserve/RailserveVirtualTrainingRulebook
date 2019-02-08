using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClipboardReader : MonoBehaviour {

	public ClipboardList clipboard;
	public string source;
	public Text result;
	
	void Update () {
		result.text = source.Replace("*1", clipboard.NumCompleted.ToString()).Replace("*2", clipboard.NumElements.ToString());
	}
}
