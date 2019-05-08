using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MHLab.PATCH;

public class GetCurrentVersion : MonoBehaviour {
	
	public Text versionText;
	
	private string versionOverride = "1.0.1.1";
	
	void Start () {
		versionText.text = "Ver." + new LauncherManager().GetCurrentVersion();
		if (versionOverride != "")
			versionText.text = "Ver. " + versionOverride;
		Debug.Log(versionText.text);
	}
}
