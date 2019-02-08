using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildTrigger: MonoBehaviour{

	public int LookCount = 0;

	void Start () {

	}



	void OnTriggerEnter(Collider c){
		if (c.gameObject.name == "LookR") {
			LookCount += 1;
			c.gameObject.SetActive (false);
			//gameObject.GetComponentInParent<CrossingSter_Mngr>().StartSafe(c);
		}
	

		if (c.gameObject.name == "LookL") {
			LookCount += 1;
			c.gameObject.SetActive (false);
			//gameObject.GetComponentInParent<CrossingSter_Mngr>().StartSafe(c);
		}


	}
		void Update() {

		if (LookCount == 2) {
		
			gameObject.GetComponentInParent<CrossingSter_Mngr>().StartSafe();
			LookCount = 0;
		}


		}

	public void Reset (){
		LookCount = 0;
	}


}

