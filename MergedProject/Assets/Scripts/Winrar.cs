using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Winrar : MonoBehaviour {
	
	public Text guideText;
	public string winText = "Congratulations!  You've completed all available training courses!";
	public InteractionHandler.InvokableState OnWin;
	
	void OnEnable () {
		guideText.text = winText;
		OnWin.Invoke();
	}
}
