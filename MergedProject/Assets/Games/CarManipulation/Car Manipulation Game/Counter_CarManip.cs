using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter_CarManip : MonoBehaviour {


	public int Counter = 0;
	public int ActivateNum;
	public GameObject Sitelead_Old, SiteLeadNew;
	// Use this for initialization
	void Start () {
		
	}

	public void AddNum(){
		Counter++;
	}

	// Update is called once per frame
	void Update () {

		if (Counter == ActivateNum) {
		
			Sitelead_Old.gameObject.SetActive (false);
			SiteLeadNew.gameObject.SetActive (true);
		
		}

	}
}
