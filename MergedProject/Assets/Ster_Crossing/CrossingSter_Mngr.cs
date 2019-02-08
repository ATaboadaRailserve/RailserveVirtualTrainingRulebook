using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossingSter_Mngr : MonoBehaviour {

	//public InteractionHandler interactionHandler;

	public GameObject CrossLook, LookL, LookR, Head, SafeIcon, WarningIcon, KILLME;

	public bool SafeToCross = false, InsideWarning = false;

	public float SafeTime = 10f;
	public float SafeOrig = 10f;
	// Use this for initialization
	void Start () {

		KILLME.SetActive (false);

		CrossLook.SetActive (false);
		SafeIcon.SetActive (false);
		WarningIcon.SetActive (false);
	}
		
	//************************************************** Collision Triggers ***************************************************************//
	void OnTriggerEnter(Collider warning) {
		if (warning.gameObject.tag == "CrossWarning") {
			
			GameObject warningTrans = warning.gameObject;

			CrossLook.SetActive (true);
			LookL.SetActive (true);
			LookR.SetActive (true);
			InsideWarning = true;
			SafeIcon.SetActive (false);
			WarningIcon.SetActive (true);
	
			CrossLook.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, warning.transform.rotation.eulerAngles.y - 90, transform.rotation.eulerAngles.z);
			Head.gameObject.GetComponent<ChildTrigger>().Reset();
		}


		if (warning.gameObject.tag == "Crossing") {
			if (!SafeToCross) {
				KillPlayer ();
			}
		}


	}


	void OnTriggerExit(Collider warningLeave) {
		if (warningLeave.gameObject.tag == "CrossWarning") {

			CrossLook.SetActive (false);
			LookL.SetActive (true);
			LookR.SetActive (true);
			SafeToCross = false;
			SafeTime = SafeOrig;
			InsideWarning = false;
			SafeIcon.SetActive (false);
			WarningIcon.SetActive (false);

		Head.gameObject.GetComponent<ChildTrigger>().Reset();
		}
	}

	//************************************************** Called Functions ***************************************************************//

	void KillPlayer (){

		Debug.Log("Player has Failed to cross correctly!", this.gameObject);
		KILLME.SetActive (true);

	}

	void CheckIfInside (){

		if (InsideWarning) {
			
			SafeIcon.SetActive (false);
			WarningIcon.SetActive (true);

			CrossLook.SetActive (true);
			LookL.SetActive (true);
			LookR.SetActive (true);

			Head.gameObject.GetComponent<ChildTrigger>().Reset();
		
		}

		return;
	}


	public void StartSafe (){
		SafeToCross = true;
	}


	void FixedUpdate() {

		if (SafeToCross) {
			SafeTime -= Time.deltaTime;
		
			SafeIcon.SetActive (true);
			WarningIcon.SetActive (false);

		if(SafeTime < 0) {
			SafeToCross = false;
			SafeTime = SafeOrig; //resets timer for next time;
			Head.gameObject.GetComponent<ChildTrigger>().Reset();

				CheckIfInside ();
			}  }
		
	}


}
	