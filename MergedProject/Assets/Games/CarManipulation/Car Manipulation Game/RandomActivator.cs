using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomActivator : MonoBehaviour {
	
	public GameObject AngleCockA, AirhoseA, AirBrakeA, TruckA, PistonsA, AngleCockB, AirhoseB, AirBrakeB, TruckB, HandBrakeB;
	public GameObject AngleCockAz, AirhoseAz, AirBrakeAz, TruckAz, PistonsAz, AngleCockBz, AirhoseBz, AirBrakeBz, TruckBz, HandBrakeBz;
	
	public int num;
	
	// I'm sorry I know this script is shit
	//I'm not really that sorry ;/
	//it's ok
	//history will only repeat itself
	
	// You're right, it is shit.
	// History is also repeating itself because I'm fixing this stuff yet again :T
	//                </3 Kyle
	
	void Start () {
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
		
		switch (num) {
			case 0:
				AngleCockA.gameObject.SetActive (true);
				AngleCockAz.gameObject.SetActive (false);
				break;
			case 1:
				AngleCockB.gameObject.SetActive (true);
				AngleCockBz.gameObject.SetActive (false);
				break;
			case 2:
				AirhoseA.gameObject.SetActive (true);
				AirhoseAz.gameObject.SetActive (false);
				break;
			case 3:
				AirhoseB.gameObject.SetActive (true);
				AirhoseBz.gameObject.SetActive (false);
				break;
			case 4:
				AirBrakeA.gameObject.SetActive (true);
				AirBrakeAz.gameObject.SetActive (false);
				break;
			case 5:
				AirBrakeB.gameObject.SetActive (true);
				AirBrakeBz.gameObject.SetActive (false);
				break;
			case 6:
				TruckA.gameObject.SetActive (true);
				TruckAz.gameObject.SetActive (false);
				break;
			case 7:
				TruckB.gameObject.SetActive (true);
				TruckBz.gameObject.SetActive (false);
				break;
			case 8:
				PistonsA.gameObject.SetActive (true);
				PistonsAz.gameObject.SetActive (false);
				break;
			case 9:
				HandBrakeB.gameObject.SetActive (true);
				HandBrakeBz.gameObject.SetActive (false);
				break;
		}
	}
}


