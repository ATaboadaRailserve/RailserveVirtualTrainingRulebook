using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class ModuleActivator : MonoBehaviour {

	//public int moduleID;
	//public char subModuleID;
	//public GameObject[] childModules;
	//public bool checkOnStart;
	
	//private bool[] activeChildren;
	
	[System.Serializable]
	public class Module {
		public int moduleID;
		public string subModule;
		public int[] childModules; // The index in 'this.modules' that turns on AFTER this module is completed
		public GameObject button;
	}
	
	public Module[] modules;
	
	private string status;
	
	void Start () {
		StartCoroutine("CheckModules");
	}
	
	public void CheckModule () {
		StartCoroutine("CheckModules");
	}
	
	// 1 - Not Started
	// a - Started First Part
	// A - Finished First Part not Started Second Part
	// b - Started Second Part
	// B - Finished Second Part not Started Third Part
	// c - Started Second Part
	// C - Finished Third Part not Started Second Part
	IEnumerator CheckModules () {
		//yield return null;
		//status = GameObject.FindWithTag("MessageHolder").GetComponent<DatabaseMessageHolder>().procedureStatus;
		status = GameObject.FindWithTag("DataLoader").GetComponent<DataLoader>().CurrentUser.trainingPorcedure.ToString();
		print("Status: " + status);
		// if (modules[0].button)
		// modules[0].button.SetActive(true);
		if (status[0] > '1') {
			print("Module 0 Complete");
			if (modules[0].button)
				modules[0].button.SetActive(true);
			foreach (int i in modules[0].childModules) {
				CheckModule(i);
			}
		}
		
		yield return null;
	}
	
	void CheckModule (int i) {
		print ("Checking Module " + i);
		if (modules[i].button)
			modules[i].button.SetActive(true);
		if (!(status[modules[i].moduleID] == modules[i].subModule.ToLower()[0])
			&& status[modules[i].moduleID].ToString().ToLower()[0] >= modules[i].subModule.ToLower()[0]) {
			print("Module " + i + " Complete");
			foreach (int j in modules[i].childModules) {
				CheckModule(j);
			}
		}
	}
	
	/*
	void Start () {
		
		activeChildren = new bool[childModules.Length];
		
		if (checkOnStart);
			StartCoroutine("CheckModules");
	}
	
	void UpdateCheck () {
		print(gameObject.name + " is checking");
		StartCoroutine("CheckModules");
	}
	
	IEnumerator CheckModules () {
		//yield return null;

		print(GameObject.FindWithTag("MessageHolder").GetComponent<DatabaseMessageHolder>().procedureStatus);
		if (GameObject.FindWithTag("MessageHolder").GetComponent<DatabaseMessageHolder>().procedureStatus.Length > moduleID && Char.IsLetter(GameObject.FindWithTag("MessageHolder").GetComponent<DatabaseMessageHolder>().procedureStatus[moduleID])) {
			if (subModuleID == GameObject.FindWithTag("MessageHolder").GetComponent<DatabaseMessageHolder>().procedureStatus[moduleID] || subModuleID < Char.ToUpper(GameObject.FindWithTag("MessageHolder").GetComponent<DatabaseMessageHolder>().procedureStatus[moduleID])) {
				print("Module " + moduleID + " has been cleared as stated by object " + gameObject.name);
				for (int i = 0; i < childModules.Length; i++) {
					childModules[i].SetActive(true);
					if (childModules[i].activeInHierarchy) {
						activeChildren[i] = true;
						childModules[i].GetComponent<ModuleActivator>().UpdateCheck();
					}
				}
			}
		}
		
		bool allDone = false;
		while (!allDone) {
			allDone = true;
			for (int i = 0; i < activeChildren.Length; i++) {
				if (!activeChildren[i]) {
					allDone = false;
					childModules[i].SetActive(true);
					if (childModules[i].activeInHierarchy) {
						activeChildren[i] = true;
						childModules[i].GetComponent<ModuleActivator>().UpdateCheck();
						allDone = true;
					}
				}
			}
			yield return null;
		}
	}
	*/
	
	/*IEnumerator CheckModules () {
		yield return null;
		print(GameObject.FindWithTag("MessageHolder").GetComponent<DatabaseMessageHolder>().procedureStatus);
		if (GameObject.FindWithTag("MessageHolder").GetComponent<DatabaseMessageHolder>().procedureStatus.Length > moduleID && Char.IsLetter(GameObject.FindWithTag("MessageHolder").GetComponent<DatabaseMessageHolder>().procedureStatus[moduleID])) {
			if (subModuleID == GameObject.FindWithTag("MessageHolder").GetComponent<DatabaseMessageHolder>().procedureStatus[moduleID] || subModuleID < Char.ToUpper(GameObject.FindWithTag("MessageHolder").GetComponent<DatabaseMessageHolder>().procedureStatus[moduleID])) {
				print("Module " + moduleID + " has been cleared as stated by object " + gameObject.name);
				for (int i = 0; i < childModules.Length; i++) {
					if (childModules [i].transform.parent.gameObject.activeInHierarchy) {
						print ("ON : " + childModules [i].name);
						childModules [i].SetActive (true);
						childModules [i].GetComponent<ModuleActivator> ().UpdateCheck ();
						childModules [i].transform.parent.gameObject.SetActive (false);
					} else {
						print ("OFF : " + childModules [i].name);
						childModules [i].transform.parent.gameObject.SetActive (true);
						yield return 0;
						childModules [i].SetActive (true);
						childModules [i].GetComponent<ModuleActivator> ().UpdateCheck ();
						StartCoroutine (ParentOff (childModules [i].transform.parent.gameObject));
						//StartCoroutine("CheckModules");
					}
				}
			}
		}
	}*/
}
	
