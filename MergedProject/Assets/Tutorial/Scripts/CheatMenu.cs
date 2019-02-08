using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CheatMenu : MonoBehaviour {
	private Text text;
	private GameObject panel;
	private bool act = false;
	public List<GameObject> onList = new List<GameObject> ();
	public List<GameObject> offList = new List<GameObject> ();
	// Use this for initialization
	void Start () {
		panel = transform.FindChild ("Panel").gameObject;
		text = panel.GetComponentInChildren<Text> ();
		panel.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Tab)) {
			act = !act;
			panel.SetActive (act);
		}

		if (act) {
			text.text += Input.inputString;
			if (Input.GetKeyDown (KeyCode.Backspace)) {
				text.text = "/ ";
			}
			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				if (text.text == "/ unlock") {
					Unlock ();
				}
				else if (text.text == "/ relock") {
					Relock();
				}
				text.text = "/ ";
			}
		}
	}

	public void Unlock()
	{
		//GameObject.FindWithTag ("MessageHolder").GetComponent<DatabaseMessageHolder> ().procedureStatus = "ABC111";
		foreach (GameObject g in onList) {
			g.SetActive (true);
		}
		foreach (GameObject g in offList) {
			g.SetActive (false);
		}
	}

	public void Relock()
	{
		//GameObject.FindWithTag ("MessageHolder").GetComponent<DatabaseMessageHolder> ().procedureStatus = "ABC111";
		foreach (GameObject g in onList) {
			g.SetActive (false);
		}
		foreach (GameObject g in offList) {
			g.SetActive (true);
		}
	}
}
