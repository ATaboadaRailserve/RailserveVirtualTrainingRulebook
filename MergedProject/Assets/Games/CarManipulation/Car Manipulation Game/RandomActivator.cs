using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomActivator : MonoBehaviour {
	
	public GameObject AngleCockA, AirhoseA, AirBrakeA, TruckA, PistonsA, AngleCockB, AirhoseB, AirBrakeB, TruckB, HandBrakeB;
	public GameObject AngleCockAz, AirhoseAz, AirBrakeAz, TruckAz, PistonsAz, AngleCockBz, AirhoseBz, AirBrakeBz, TruckBz, HandBrakeBz;
	public bool isA = true;
	public bool SetOne = true;
	int Num;
	// I'm sorry I know this script is shit
	//I'm not really that sorry ;/
	//it's ok
	//history will only repeat itself
	void Awake(){		}
	void Start()
	{
		//a
		AngleCockA.gameObject.SetActive (false);
		AirhoseA.gameObject.SetActive (false);
		AirBrakeA.gameObject.SetActive (false);
		TruckA.gameObject.SetActive (false);
		PistonsA.gameObject.SetActive (false);
		//b
		AngleCockB.gameObject.SetActive (false);
		AirhoseB.gameObject.SetActive (false);
		AirBrakeB.gameObject.SetActive (false);
		TruckB.gameObject.SetActive (false);
		HandBrakeB.gameObject.SetActive (false);

		//wrongs
		//a
		AngleCockAz.gameObject.SetActive (true);
		AirhoseAz.gameObject.SetActive (true);
		AirBrakeAz.gameObject.SetActive (true);
		TruckAz.gameObject.SetActive (true);
		PistonsAz.gameObject.SetActive (true);
		//b
		AngleCockBz.gameObject.SetActive (true);
		AirhoseBz.gameObject.SetActive (true);
		AirBrakeBz.gameObject.SetActive (true);
		TruckBz.gameObject.SetActive (true);
		HandBrakeBz.gameObject.SetActive (true);

		//Num = (Random.Range(1, 6));


		if (SetOne) {
			Num = (Random.Range(1, 3));
		}
		if (!SetOne) {
			Num = (Random.Range(4, 6));
		}


		Debug.Log("nums is " + Num);

		if (Num == 1) {
			if (isA) {
				AngleCockA.gameObject.SetActive (true);
				AngleCockAz.gameObject.SetActive (false);
			}
			if (!isA) {
				AngleCockB.gameObject.SetActive (true);
				AngleCockBz.gameObject.SetActive (false);
			}

		}

		if (Num == 2) {
			if (isA) {
				AirhoseA.gameObject.SetActive (true);
				AirhoseAz.gameObject.SetActive (false);
			}
			if (!isA) {
				AirhoseB.gameObject.SetActive (true);
				AirhoseBz.gameObject.SetActive (false);
			}

		}

		if (Num == 3) {
			if (isA) {
				AirBrakeA.gameObject.SetActive (true);
				AirBrakeAz.gameObject.SetActive (false);
			}
			if (!isA) {
				AirBrakeB.gameObject.SetActive (true);
				AirBrakeBz.gameObject.SetActive (false);
			}
		}

		if (Num == 4) {
			if (isA) {
				TruckA.gameObject.SetActive (true);
				TruckAz.gameObject.SetActive (false);
			}
			if (!isA) {
				TruckB.gameObject.SetActive (true);
				TruckBz.gameObject.SetActive (false);
			}
		}

		if (Num == 5) {
			if (isA) {
				PistonsA.gameObject.SetActive (true);
				PistonsAz.gameObject.SetActive (false);
			}
			if (!isA) {
				HandBrakeB.gameObject.SetActive (true);
				HandBrakeBz.gameObject.SetActive (false);
			}
		}


	}
}


